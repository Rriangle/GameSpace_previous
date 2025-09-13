using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Services
{
    /// <summary>
    /// 錢包服務 - 實現錢包業務邏輯和約束
    /// </summary>
    public class WalletService
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<WalletService> _logger;

        public WalletService(GameSpaceDbContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 安全地增加點數（帶約束檢查）
        /// </summary>
        public async Task<WalletOperationResult> AddPointsAsync(int userId, decimal amount, string description, string? referenceId = null)
        {
            if (amount <= 0)
            {
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "充值金額必須大於0"
                };
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                var wallet = await GetOrCreateWalletAsync(userId);
                var balanceBefore = wallet.UserPoint;
                var balanceAfter = balanceBefore + amount;

                // 檢查餘額是否會超出合理範圍（防止溢出）
                if (balanceAfter > 999999999.99m)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "餘額超出最大限制"
                    };
                }

                // 更新錢包餘額
                wallet.UserPoint = balanceAfter;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄交易歷史
                var history = new WalletHistory
                {
                    UserId = userId,
                    TransactionType = "充值",
                    Amount = amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter,
                    Description = description ?? $"充值 {amount} 點",
                    ReferenceId = referenceId ?? Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("用戶 {UserId} 充值 {Amount} 點成功，餘額: {Balance}", userId, amount, balanceAfter);

                return new WalletOperationResult
                {
                    Success = true,
                    Message = $"充值成功！當前餘額: {balanceAfter} 點",
                    NewBalance = balanceAfter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "充值過程中發生錯誤，用戶ID: {UserId}, 金額: {Amount}", userId, amount);
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "充值失敗，請稍後再試"
                };
            }
        }

        /// <summary>
        /// 安全地扣除點數（帶約束檢查）
        /// </summary>
        public async Task<WalletOperationResult> SpendPointsAsync(int userId, decimal amount, string description, string? referenceId = null)
        {
            if (amount <= 0)
            {
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "消費金額必須大於0"
                };
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                var wallet = await GetOrCreateWalletAsync(userId);
                var balanceBefore = wallet.UserPoint;

                // 檢查餘額是否足夠
                if (balanceBefore < amount)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "餘額不足",
                        CurrentBalance = balanceBefore
                    };
                }

                var balanceAfter = balanceBefore - amount;

                // 確保餘額不會為負數（雙重檢查）
                if (balanceAfter < 0)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "操作會導致餘額為負數，交易已取消",
                        CurrentBalance = balanceBefore
                    };
                }

                // 更新錢包餘額
                wallet.UserPoint = balanceAfter;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄交易歷史
                var history = new WalletHistory
                {
                    UserId = userId,
                    TransactionType = "消費",
                    Amount = -amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter,
                    Description = description ?? $"消費 {amount} 點",
                    ReferenceId = referenceId ?? Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("用戶 {UserId} 消費 {Amount} 點成功，餘額: {Balance}", userId, amount, balanceAfter);

                return new WalletOperationResult
                {
                    Success = true,
                    Message = $"消費成功！當前餘額: {balanceAfter} 點",
                    NewBalance = balanceAfter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "消費過程中發生錯誤，用戶ID: {UserId}, 金額: {Amount}", userId, amount);
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "消費失敗，請稍後再試"
                };
            }
        }

        /// <summary>
        /// 安全地轉移點數（用戶間轉帳）
        /// </summary>
        public async Task<WalletOperationResult> TransferPointsAsync(int fromUserId, int toUserId, decimal amount, string description)
        {
            if (amount <= 0)
            {
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "轉帳金額必須大於0"
                };
            }

            if (fromUserId == toUserId)
            {
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "不能轉帳給自己"
                };
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                var fromWallet = await GetOrCreateWalletAsync(fromUserId);
                var toWallet = await GetOrCreateWalletAsync(toUserId);

                var fromBalanceBefore = fromWallet.UserPoint;
                var toBalanceBefore = toWallet.UserPoint;

                // 檢查轉出方餘額是否足夠
                if (fromBalanceBefore < amount)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "餘額不足",
                        CurrentBalance = fromBalanceBefore
                    };
                }

                var fromBalanceAfter = fromBalanceBefore - amount;
                var toBalanceAfter = toBalanceBefore + amount;

                // 確保轉出方餘額不會為負數
                if (fromBalanceAfter < 0)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "操作會導致餘額為負數，交易已取消",
                        CurrentBalance = fromBalanceBefore
                    };
                }

                // 檢查轉入方餘額是否會超出合理範圍
                if (toBalanceAfter > 999999999.99m)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "轉入方餘額超出最大限制"
                    };
                }

                // 更新轉出方錢包
                fromWallet.UserPoint = fromBalanceAfter;
                fromWallet.UpdatedAt = DateTime.UtcNow;

                // 更新轉入方錢包
                toWallet.UserPoint = toBalanceAfter;
                toWallet.UpdatedAt = DateTime.UtcNow;

                // 記錄轉出方交易歷史
                var fromHistory = new WalletHistory
                {
                    UserId = fromUserId,
                    TransactionType = "轉出",
                    Amount = -amount,
                    BalanceBefore = fromBalanceBefore,
                    BalanceAfter = fromBalanceAfter,
                    Description = description ?? $"轉出給用戶 {toUserId}",
                    ReferenceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                // 記錄轉入方交易歷史
                var toHistory = new WalletHistory
                {
                    UserId = toUserId,
                    TransactionType = "轉入",
                    Amount = amount,
                    BalanceBefore = toBalanceBefore,
                    BalanceAfter = toBalanceAfter,
                    Description = description ?? $"從用戶 {fromUserId} 轉入",
                    ReferenceId = fromHistory.ReferenceId, // 使用相同的參考ID
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.AddRange(fromHistory, toHistory);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("用戶 {FromUserId} 轉帳 {Amount} 點給用戶 {ToUserId} 成功", fromUserId, amount, toUserId);

                return new WalletOperationResult
                {
                    Success = true,
                    Message = $"轉帳成功！當前餘額: {fromBalanceAfter} 點",
                    NewBalance = fromBalanceAfter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "轉帳過程中發生錯誤，從用戶 {FromUserId} 到用戶 {ToUserId}, 金額: {Amount}", fromUserId, toUserId, amount);
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "轉帳失敗，請稍後再試"
                };
            }
        }

        /// <summary>
        /// 獲取或創建用戶錢包
        /// </summary>
        private async Task<UserWallet> GetOrCreateWalletAsync(int userId)
        {
            var wallet = await _context.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                wallet = new UserWallet
                {
                    UserId = userId,
                    UserPoint = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.UserWallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            return wallet;
        }

        /// <summary>
        /// 獲取用戶錢包餘額
        /// </summary>
        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var wallet = await _context.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            return wallet?.UserPoint ?? 0;
        }

        /// <summary>
        /// 獲取用戶交易歷史
        /// </summary>
        public async Task<List<WalletHistory>> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await _context.WalletHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// 管理員手動調整點數
        /// </summary>
        public async Task<WalletOperationResult> AdminAdjustPointsAsync(int userId, decimal amount, string reason, int adminUserId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                var wallet = await GetOrCreateWalletAsync(userId);
                var balanceBefore = wallet.UserPoint;
                var balanceAfter = balanceBefore + amount;

                // 檢查調整後的餘額是否合理
                if (balanceAfter < 0)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "調整後餘額不能為負數"
                    };
                }

                if (balanceAfter > 999999999.99m)
                {
                    await transaction.RollbackAsync();
                    return new WalletOperationResult
                    {
                        Success = false,
                        Message = "調整後餘額超出最大限制"
                    };
                }

                // 更新錢包餘額
                wallet.UserPoint = balanceAfter;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄管理員調整歷史
                var history = new WalletHistory
                {
                    UserId = userId,
                    TransactionType = amount > 0 ? "管理員增加" : "管理員扣除",
                    Amount = amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter,
                    Description = $"管理員調整: {reason} (操作員ID: {adminUserId})",
                    ReferenceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("管理員 {AdminUserId} 調整用戶 {UserId} 點數 {Amount}，原因: {Reason}", adminUserId, userId, amount, reason);

                return new WalletOperationResult
                {
                    Success = true,
                    Message = $"調整成功！當前餘額: {balanceAfter} 點",
                    NewBalance = balanceAfter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "管理員調整點數時發生錯誤，用戶ID: {UserId}, 金額: {Amount}", userId, amount);
                return new WalletOperationResult
                {
                    Success = false,
                    Message = "調整失敗，請稍後再試"
                };
            }
        }
    }

    /// <summary>
    /// 錢包操作結果
    /// </summary>
    public class WalletOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal? NewBalance { get; set; }
        public decimal? CurrentBalance { get; set; }
    }
}