using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 錢包控制器
    /// </summary>
    public class WalletController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<WalletController> _logger;

        public WalletController(GameSpaceDbContext context, ILogger<WalletController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示我的錢包
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var wallet = await _context.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                // 創建新錢包
                wallet = await CreateNewWalletAsync(userId.Value);
            }

            var history = await _context.WalletHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .Take(20)
                .ToListAsync();

            ViewBag.Wallet = wallet;
            ViewBag.History = history;

            return View();
        }

        /// <summary>
        /// 點數充值
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPoints(AddPointsViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            if (model.Amount <= 0)
            {
                return Json(new { success = false, message = "充值金額必須大於0" });
            }

            try
            {
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                {
                    wallet = await CreateNewWalletAsync(userId.Value);
                }

                var balanceBefore = wallet.UserPoint;
                wallet.UserPoint += model.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄交易歷史
                var history = new WalletHistory
                {
                    UserId = userId.Value,
                    TransactionType = "充值",
                    Amount = model.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.UserPoint,
                    Description = $"充值 {model.Amount} 點",
                    ReferenceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();

                _logger.LogInformation("用戶 {UserId} 充值 {Amount} 點，餘額: {Balance}", userId, model.Amount, wallet.UserPoint);

                return Json(new { success = true, message = $"充值成功！當前餘額: {wallet.UserPoint} 點", balance = wallet.UserPoint });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "充值過程中發生錯誤");
                return Json(new { success = false, message = "充值失敗，請稍後再試" });
            }
        }

        /// <summary>
        /// 點數消費
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SpendPoints(SpendPointsViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            if (model.Amount <= 0)
            {
                return Json(new { success = false, message = "消費金額必須大於0" });
            }

            try
            {
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                {
                    return Json(new { success = false, message = "錢包不存在" });
                }

                if (wallet.UserPoint < model.Amount)
                {
                    return Json(new { success = false, message = "餘額不足" });
                }

                var balanceBefore = wallet.UserPoint;
                wallet.UserPoint -= model.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄交易歷史
                var history = new WalletHistory
                {
                    UserId = userId.Value,
                    TransactionType = "消費",
                    Amount = -model.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.UserPoint,
                    Description = model.Description ?? $"消費 {model.Amount} 點",
                    ReferenceId = model.ReferenceId ?? Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(history);
                await _context.SaveChangesAsync();

                _logger.LogInformation("用戶 {UserId} 消費 {Amount} 點，餘額: {Balance}", userId, model.Amount, wallet.UserPoint);

                return Json(new { success = true, message = $"消費成功！當前餘額: {wallet.UserPoint} 點", balance = wallet.UserPoint });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "消費過程中發生錯誤");
                return Json(new { success = false, message = "消費失敗，請稍後再試" });
            }
        }

        /// <summary>
        /// 創建新錢包
        /// </summary>
        private async Task<UserWallet> CreateNewWalletAsync(int userId)
        {
            var wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserWallets.Add(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        /// <summary>
        /// 獲取當前用戶ID
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }

        /// <summary>
        /// 點數兌換電子禮券
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExchangeForEVoucher(ExchangeEVoucherViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            if (model.RequiredPoints <= 0)
            {
                return Json(new { success = false, message = "兌換點數必須大於0" });
            }

            try
            {
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet == null)
                {
                    return Json(new { success = false, message = "錢包不存在" });
                }

                // 檢查點數餘額
                if (wallet.UserPoint < model.RequiredPoints)
                {
                    return Json(new { success = false, message = "點數餘額不足" });
                }

                // 獲取電子禮券類型
                var eVoucherType = await _context.EVoucherTypes
                    .FirstOrDefaultAsync(et => et.EVoucherTypeId == model.EVoucherTypeId);

                if (eVoucherType == null || !eVoucherType.IsActive)
                {
                    return Json(new { success = false, message = "電子禮券類型不存在或已停用" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                // 扣除點數
                var balanceBefore = wallet.UserPoint;
                wallet.UserPoint -= model.RequiredPoints;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄點數消費歷史
                var spendHistory = new WalletHistory
                {
                    UserId = userId.Value,
                    TransactionType = "兌換電子禮券",
                    Amount = -model.RequiredPoints,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.UserPoint,
                    Description = $"兌換電子禮券: {eVoucherType.TypeName}",
                    ReferenceId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(spendHistory);

                // 生成電子禮券
                var eVoucherCode = GenerateEVoucherCode();
                var eVoucher = new EVoucher
                {
                    EVoucherCode = eVoucherCode,
                    EVoucherTypeId = model.EVoucherTypeId,
                    UserId = userId.Value,
                    IsUsed = false,
                    AcquiredTime = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30) // 30天後過期
                };

                _context.EVouchers.Add(eVoucher);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { 
                    success = true, 
                    message = $"成功兌換電子禮券: {eVoucherCode}",
                    eVoucherCode = eVoucherCode,
                    eVoucherType = eVoucherType.TypeName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "兌換電子禮券時發生錯誤");
                return Json(new { success = false, message = "兌換電子禮券失敗，請稍後再試" });
            }
        }

        /// <summary>
        /// 生成電子禮券代碼
        /// </summary>
        private string GenerateEVoucherCode()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"EV{result}";
        }
    }

    /// <summary>
    /// 充值視圖模型
    /// </summary>
    public class AddPointsViewModel
    {
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// 消費視圖模型
    /// </summary>
    public class SpendPointsViewModel
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ReferenceId { get; set; }
    }

    /// <summary>
    /// 兌換電子禮券視圖模型
    /// </summary>
    public class ExchangeEVoucherViewModel
    {
        public int EVoucherTypeId { get; set; }
        public decimal RequiredPoints { get; set; }
    }
}