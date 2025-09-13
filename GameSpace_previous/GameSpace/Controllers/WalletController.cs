using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GameSpace.Data;
using GameSpace.Models;
using GameSpace.Services;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 錢包控制器
    /// </summary>
    public class WalletController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<WalletController> _logger;
        private readonly WalletService _walletService;

        public WalletController(GameSpaceDbContext context, ILogger<WalletController> logger, WalletService walletService)
        {
            _context = context;
            _logger = logger;
            _walletService = walletService;
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

            var result = await _walletService.AddPointsAsync(
                userId.Value, 
                model.Amount, 
                $"充值 {model.Amount} 點"
            );

            return Json(new { 
                success = result.Success, 
                message = result.Message, 
                balance = result.NewBalance 
            });
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

            var result = await _walletService.SpendPointsAsync(
                userId.Value, 
                model.Amount, 
                model.Description ?? $"消費 {model.Amount} 點",
                model.ReferenceId
            );

            return Json(new { 
                success = result.Success, 
                message = result.Message, 
                balance = result.NewBalance 
            });
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
                // 獲取電子禮券類型
                var eVoucherType = await _context.EVoucherTypes
                    .FirstOrDefaultAsync(et => et.EVoucherTypeId == model.EVoucherTypeId);

                if (eVoucherType == null || !eVoucherType.IsActive)
                {
                    return Json(new { success = false, message = "電子禮券類型不存在或已停用" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                // 使用服務扣除點數
                var spendResult = await _walletService.SpendPointsAsync(
                    userId.Value,
                    model.RequiredPoints,
                    $"兌換電子禮券: {eVoucherType.TypeName}",
                    Guid.NewGuid().ToString()
                );

                if (!spendResult.Success)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = spendResult.Message });
                }

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
                    eVoucherType = eVoucherType.TypeName,
                    balance = spendResult.NewBalance
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

        /// <summary>
        /// 兌換優惠券
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExchangeForCoupon(ExchangeCouponViewModel model)
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
                var couponType = await _context.CouponTypes.FirstOrDefaultAsync(ct => ct.CouponTypeId == model.CouponTypeId);
                if (couponType == null || !couponType.IsActive)
                {
                    return Json(new { success = false, message = "優惠券類型不存在或已停用" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                // 使用服務扣除點數
                var spendResult = await _walletService.SpendPointsAsync(
                    userId.Value,
                    model.RequiredPoints,
                    $"兌換優惠券: {couponType.TypeName}",
                    Guid.NewGuid().ToString()
                );

                if (!spendResult.Success)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = spendResult.Message });
                }

                // 生成優惠券
                var couponCode = GenerateCouponCode();
                var coupon = new Coupon
                {
                    UserId = userId.Value,
                    CouponCode = couponCode,
                    CouponTypeId = model.CouponTypeId,
                    IsUsed = false,
                    AcquiredTime = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Coupons.Add(coupon);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("用戶 {UserId} 兌換優惠券成功: {CouponCode}", userId, couponCode);

                return Json(new { 
                    success = true, 
                    message = $"成功兌換優惠券: {couponCode}", 
                    couponCode = couponCode,
                    couponType = couponType.TypeName,
                    balance = spendResult.NewBalance
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "兌換優惠券時發生錯誤");
                return Json(new { success = false, message = "兌換優惠券失敗，請稍後再試" });
            }
        }

        /// <summary>
        /// 生成優惠券代碼
        /// </summary>
        private string GenerateCouponCode()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"CP{result}";
        }

        /// <summary>
        /// 轉帳給其他用戶
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferPoints(TransferPointsViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var result = await _walletService.TransferPointsAsync(
                userId.Value,
                model.ToUserId,
                model.Amount,
                model.Description ?? $"轉帳給用戶 {model.ToUserId}"
            );

            return Json(new { 
                success = result.Success, 
                message = result.Message, 
                balance = result.NewBalance 
            });
        }

        /// <summary>
        /// 管理員調整點數
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminAdjustPoints(AdminAdjustPointsViewModel model)
        {
            var adminUserId = GetCurrentUserId();
            if (adminUserId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var result = await _walletService.AdminAdjustPointsAsync(
                model.UserId,
                model.Amount,
                model.Reason,
                adminUserId.Value
            );

            return Json(new { 
                success = result.Success, 
                message = result.Message, 
                balance = result.NewBalance 
            });
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

    /// <summary>
    /// 兌換優惠券視圖模型
    /// </summary>
    public class ExchangeCouponViewModel
    {
        public int CouponTypeId { get; set; }
        public decimal RequiredPoints { get; set; }
    }

    /// <summary>
    /// 轉帳視圖模型
    /// </summary>
    public class TransferPointsViewModel
    {
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// 管理員調整點數視圖模型
    /// </summary>
    public class AdminAdjustPointsViewModel
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}