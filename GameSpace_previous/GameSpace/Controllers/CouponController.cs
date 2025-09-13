using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 優惠券控制器
    /// </summary>
    public class CouponController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public CouponController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 優惠券列表頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var coupons = await _context.Coupons
                .Include(c => c.CouponType)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AcquiredTime)
                .ToListAsync();

            return View(coupons);
        }

        /// <summary>
        /// 獲取用戶可用的優惠券
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAvailableCoupons(int userId)
        {
            var coupons = await _context.Coupons
                .Include(c => c.CouponType)
                .Where(c => c.UserId == userId && !c.IsUsed && c.CouponType.IsActive)
                .Where(c => c.CouponType.ValidFrom <= DateTime.Now && c.CouponType.ValidTo >= DateTime.Now)
                .Select(c => new
                {
                    c.CouponId,
                    c.CouponCode,
                    c.CouponType.TypeName,
                    c.CouponType.DiscountType,
                    c.CouponType.DiscountValue,
                    c.CouponType.MinOrderAmount,
                    c.CouponType.MaxDiscountAmount,
                    c.AcquiredTime,
                    c.CouponType.ValidTo
                })
                .ToListAsync();

            return Json(coupons);
        }

        /// <summary>
        /// 使用優惠券
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UseCoupon(int couponId, int orderId)
        {
            try
            {
                var coupon = await _context.Coupons
                    .Include(c => c.CouponType)
                    .FirstOrDefaultAsync(c => c.CouponId == couponId);

                if (coupon == null)
                {
                    return Json(new { success = false, message = "優惠券不存在" });
                }

                if (coupon.IsUsed)
                {
                    return Json(new { success = false, message = "優惠券已使用" });
                }

                if (coupon.CouponType.ValidTo < DateTime.Now)
                {
                    return Json(new { success = false, message = "優惠券已過期" });
                }

                // 更新優惠券狀態
                coupon.IsUsed = true;
                coupon.UsedTime = DateTime.Now;
                coupon.UsedInOrderId = orderId;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "優惠券使用成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"使用優惠券失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 生成優惠券
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GenerateCoupon(int userId, int couponTypeId)
        {
            try
            {
                var couponType = await _context.CouponTypes
                    .FirstOrDefaultAsync(ct => ct.CouponTypeId == couponTypeId);

                if (couponType == null)
                {
                    return Json(new { success = false, message = "優惠券類型不存在" });
                }

                if (!couponType.IsActive)
                {
                    return Json(new { success = false, message = "優惠券類型已停用" });
                }

                // 生成唯一優惠券代碼
                var couponCode = GenerateCouponCode();

                var coupon = new Coupon
                {
                    CouponCode = couponCode,
                    CouponTypeId = couponTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = DateTime.Now
                };

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "優惠券生成成功", couponId = coupon.CouponId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"生成優惠券失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 生成優惠券代碼
        /// </summary>
        private string GenerateCouponCode()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"COUPON{result}";
        }
    }
}