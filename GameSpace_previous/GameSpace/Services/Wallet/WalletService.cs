using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GameSpace.Services.Wallet
{
    public class WalletService : IWalletService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<WalletService> _logger;

        public WalletService(GameSpacedatabaseContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WalletResult> GetWalletAsync(int userId)
        {
            try
            {
                var wallet = await _context.UserWallets
                    .Include(uw => uw.User)
                    .FirstOrDefaultAsync(uw => uw.UserId == userId);

                if (wallet == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Wallet not found" };
                }

                return new WalletResult { Success = true, Wallet = wallet };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet for user {UserId}", userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to get wallet" };
            }
        }

        public async Task<WalletResult> AddPointsAsync(int userId, int points, string description)
        {
            if (points <= 0)
            {
                return new WalletResult { Success = false, ErrorMessage = "Points must be positive" };
            }

            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var wallet = await _context.UserWallets.FindAsync(userId);
                if (wallet == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Wallet not found" };
                }

                wallet.UserPoint += points;

                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "Point",
                    PointsChanged = points,
                    Description = description,
                    ChangeTime = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, Wallet = wallet };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding points for user {UserId}", userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to add points" };
            }
        }

        public async Task<WalletResult> DeductPointsAsync(int userId, int points, string description)
        {
            if (points <= 0)
            {
                return new WalletResult { Success = false, ErrorMessage = "Points must be positive" };
            }

            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var wallet = await _context.UserWallets.FindAsync(userId);
                if (wallet == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Wallet not found" };
                }

                if (wallet.UserPoint < points)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Insufficient points" };
                }

                wallet.UserPoint -= points;

                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "Point",
                    PointsChanged = -points,
                    Description = description,
                    ChangeTime = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, Wallet = wallet };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deducting points for user {UserId}", userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to deduct points" };
            }
        }

        public async Task<WalletResult> TransferPointsAsync(int fromUserId, int toUserId, int points, string description)
        {
            if (points <= 0)
            {
                return new WalletResult { Success = false, ErrorMessage = "Points must be positive" };
            }

            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var fromWallet = await _context.UserWallets.FindAsync(fromUserId);
                var toWallet = await _context.UserWallets.FindAsync(toUserId);

                if (fromWallet == null || toWallet == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Wallet not found" };
                }

                if (fromWallet.UserPoint < points)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Insufficient points" };
                }

                fromWallet.UserPoint -= points;
                toWallet.UserPoint += points;

                var fromHistory = new WalletHistory
                {
                    UserId = fromUserId,
                    ChangeType = "Transfer",
                    PointsChanged = -points,
                    Description = $"{description} (To User {toUserId})",
                    ChangeTime = DateTime.UtcNow
                };

                var toHistory = new WalletHistory
                {
                    UserId = toUserId,
                    ChangeType = "Transfer",
                    PointsChanged = points,
                    Description = $"{description} (From User {fromUserId})",
                    ChangeTime = DateTime.UtcNow
                };

                _context.WalletHistories.AddRange(fromHistory, toHistory);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, Wallet = fromWallet };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring points from user {FromUserId} to user {ToUserId}", fromUserId, toUserId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to transfer points" };
            }
        }

        public async Task<List<WalletHistory>> GetWalletHistoryAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.WalletHistories
                    .Where(wh => wh.UserId == userId)
                    .OrderByDescending(wh => wh.ChangeTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet history for user {UserId}", userId);
                return new List<WalletHistory>();
            }
        }

        public async Task<WalletResult> RedeemCouponAsync(int userId, int couponTypeId)
        {
            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var couponType = await _context.CouponTypes.FindAsync(couponTypeId);
                if (couponType == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Coupon type not found" };
                }

                if (couponType.ValidTo < DateTime.UtcNow)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Coupon type has expired" };
                }

                var wallet = await _context.UserWallets.FindAsync(userId);
                if (wallet == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Wallet not found" };
                }

                if (wallet.UserPoint < couponType.PointsCost)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Insufficient points" };
                }

                // Deduct points
                wallet.UserPoint -= couponType.PointsCost;

                // Create coupon
                var coupon = new Coupon
                {
                    CouponCode = GenerateCouponCode(),
                    CouponTypeId = couponTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = DateTime.UtcNow
                };

                _context.Coupons.Add(coupon);

                // Add history
                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "Coupon",
                    PointsChanged = -couponType.PointsCost,
                    ItemCode = coupon.CouponCode,
                    Description = $"Redeemed coupon: {couponType.Name}",
                    ChangeTime = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, Wallet = wallet, Coupon = coupon };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redeeming coupon for user {UserId}", userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to redeem coupon" };
            }
        }

        public async Task<WalletResult> RedeemEVoucherAsync(int userId, int evoucherTypeId)
        {
            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var evoucherType = await _context.EvoucherTypes.FindAsync(evoucherTypeId);
                if (evoucherType == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "EVoucher type not found" };
                }

                if (evoucherType.ValidTo < DateTime.UtcNow)
                {
                    return new WalletResult { Success = false, ErrorMessage = "EVoucher type has expired" };
                }

                var wallet = await _context.UserWallets.FindAsync(userId);
                if (wallet == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Wallet not found" };
                }

                if (wallet.UserPoint < evoucherType.PointsCost)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Insufficient points" };
                }

                // Deduct points
                wallet.UserPoint -= evoucherType.PointsCost;

                // Create evoucher
                var evoucher = new Evoucher
                {
                    EvoucherCode = GenerateEVoucherCode(),
                    EvoucherTypeId = evoucherTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = DateTime.UtcNow
                };

                _context.Evouchers.Add(evoucher);

                // Add history
                var history = new WalletHistory
                {
                    UserId = userId,
                    ChangeType = "EVoucher",
                    PointsChanged = -evoucherType.PointsCost,
                    ItemCode = evoucher.EvoucherCode,
                    Description = $"Redeemed EVoucher: {evoucherType.Name}",
                    ChangeTime = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, Wallet = wallet, EVoucher = evoucher };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redeeming EVoucher for user {UserId}", userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to redeem EVoucher" };
            }
        }

        public async Task<List<Coupon>> GetUserCouponsAsync(int userId, bool includeUsed = false)
        {
            try
            {
                var query = _context.Coupons
                    .Include(c => c.CouponType)
                    .Where(c => c.UserId == userId);

                if (!includeUsed)
                {
                    query = query.Where(c => !c.IsUsed);
                }

                return await query.OrderByDescending(c => c.AcquiredTime).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coupons for user {UserId}", userId);
                return new List<Coupon>();
            }
        }

        public async Task<List<Evoucher>> GetUserEVouchersAsync(int userId, bool includeUsed = false)
        {
            try
            {
                var query = _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .Where(e => e.UserId == userId);

                if (!includeUsed)
                {
                    query = query.Where(e => !e.IsUsed);
                }

                return await query.OrderByDescending(e => e.AcquiredTime).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EVouchers for user {UserId}", userId);
                return new List<Evoucher>();
            }
        }

        public async Task<WalletResult> UseCouponAsync(int userId, string couponCode, int orderId)
        {
            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var coupon = await _context.Coupons
                    .Include(c => c.CouponType)
                    .FirstOrDefaultAsync(c => c.CouponCode == couponCode && c.UserId == userId);

                if (coupon == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Coupon not found" };
                }

                if (coupon.IsUsed)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Coupon already used" };
                }

                if (coupon.CouponType.ValidTo < DateTime.UtcNow)
                {
                    return new WalletResult { Success = false, ErrorMessage = "Coupon has expired" };
                }

                coupon.IsUsed = true;
                coupon.UsedTime = DateTime.UtcNow;
                coupon.UsedInOrderId = orderId;

                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, Coupon = coupon };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error using coupon {CouponCode} for user {UserId}", couponCode, userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to use coupon" };
            }
        }

        public async Task<WalletResult> UseEVoucherAsync(int userId, string evoucherCode)
        {
            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                var evoucher = await _context.Evouchers
                    .Include(e => e.EvoucherType)
                    .FirstOrDefaultAsync(e => e.EvoucherCode == evoucherCode && e.UserId == userId);

                if (evoucher == null)
                {
                    return new WalletResult { Success = false, ErrorMessage = "EVoucher not found" };
                }

                if (evoucher.IsUsed)
                {
                    return new WalletResult { Success = false, ErrorMessage = "EVoucher already used" };
                }

                if (evoucher.EvoucherType.ValidTo < DateTime.UtcNow)
                {
                    return new WalletResult { Success = false, ErrorMessage = "EVoucher has expired" };
                }

                evoucher.IsUsed = true;
                evoucher.UsedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                transaction.Complete();

                return new WalletResult { Success = true, EVoucher = evoucher };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error using EVoucher {EVoucherCode} for user {UserId}", evoucherCode, userId);
                return new WalletResult { Success = false, ErrorMessage = "Failed to use EVoucher" };
            }
        }

        private string GenerateCouponCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateEVoucherCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}