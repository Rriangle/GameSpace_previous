using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 每日簽到控制器
    /// </summary>
    public class CheckInController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<CheckInController> _logger;

        public CheckInController(GameSpaceDbContext context, ILogger<CheckInController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示簽到頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var today = DateTime.UtcNow.Date;
            var todayCheckIn = await _context.DailyCheckIns
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CheckInDate.Date == today);

            var consecutiveDays = await GetConsecutiveDaysAsync(userId.Value);
            var thisMonthCheckIns = await _context.DailyCheckIns
                .Where(c => c.UserId == userId && c.CheckInDate.Month == today.Month && c.CheckInDate.Year == today.Year)
                .OrderByDescending(c => c.CheckInDate)
                .ToListAsync();

            ViewBag.TodayCheckIn = todayCheckIn;
            ViewBag.ConsecutiveDays = consecutiveDays;
            ViewBag.ThisMonthCheckIns = thisMonthCheckIns;
            ViewBag.CanCheckIn = todayCheckIn == null;

            return View();
        }

        /// <summary>
        /// 執行簽到
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var today = DateTime.UtcNow.Date;
            var existingCheckIn = await _context.DailyCheckIns
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CheckInDate.Date == today);

            if (existingCheckIn != null)
            {
                return Json(new { success = false, message = "今日已簽到" });
            }

            try
            {
                var consecutiveDays = await GetConsecutiveDaysAsync(userId.Value);
                var newConsecutiveDays = consecutiveDays + 1;

                // 計算獎勵
                var pointsEarned = CalculatePointsReward(newConsecutiveDays);
                var petExpEarned = CalculatePetExpReward(newConsecutiveDays);
                var couponEarned = CalculateCouponReward(newConsecutiveDays);

                // 創建簽到記錄
                var checkIn = new DailyCheckIn
                {
                    UserId = userId.Value,
                    CheckInDate = today,
                    ConsecutiveDays = newConsecutiveDays,
                    PointsEarned = pointsEarned,
                    PetExpEarned = petExpEarned,
                    CouponEarned = couponEarned,
                    CreatedAt = DateTime.UtcNow
                };

                _context.DailyCheckIns.Add(checkIn);

                // 更新用戶錢包
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wallet != null)
                {
                    wallet.UserPoint += pointsEarned;
                    wallet.UpdatedAt = DateTime.UtcNow;

                    // 記錄錢包歷史
                    var walletHistory = new WalletHistory
                    {
                        UserId = userId.Value,
                        TransactionType = "簽到獎勵",
                        Amount = pointsEarned,
                        BalanceBefore = wallet.UserPoint - pointsEarned,
                        BalanceAfter = wallet.UserPoint,
                        Description = $"連續簽到 {newConsecutiveDays} 天獎勵",
                        ReferenceId = checkIn.CheckInId.ToString(),
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.WalletHistories.Add(walletHistory);
                }

                // 更新寵物經驗值
                var pet = await _context.Pet
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (pet != null)
                {
                    pet.Experience += petExpEarned;
                    pet.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("用戶 {UserId} 簽到成功，連續 {Days} 天，獲得 {Points} 點", userId, newConsecutiveDays, pointsEarned);

                return Json(new { 
                    success = true, 
                    message = $"簽到成功！連續 {newConsecutiveDays} 天",
                    consecutiveDays = newConsecutiveDays,
                    pointsEarned = pointsEarned,
                    petExpEarned = petExpEarned,
                    couponEarned = couponEarned
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "簽到過程中發生錯誤");
                return Json(new { success = false, message = "簽到失敗，請稍後再試" });
            }
        }

        /// <summary>
        /// 獲取連續簽到天數
        /// </summary>
        private async Task<int> GetConsecutiveDaysAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            var consecutiveDays = 0;

            for (int i = 0; i < 365; i++) // 最多檢查365天
            {
                var checkDate = today.AddDays(-i);
                var checkIn = await _context.DailyCheckIns
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.CheckInDate.Date == checkDate);

                if (checkIn != null)
                {
                    consecutiveDays++;
                }
                else
                {
                    break;
                }
            }

            return consecutiveDays;
        }

        /// <summary>
        /// 計算點數獎勵
        /// </summary>
        private int CalculatePointsReward(int consecutiveDays)
        {
            // 基礎獎勵 + 連續天數獎勵
            var baseReward = 10;
            var consecutiveBonus = Math.Min(consecutiveDays * 2, 50); // 最多50點額外獎勵
            return baseReward + consecutiveBonus;
        }

        /// <summary>
        /// 計算寵物經驗值獎勵
        /// </summary>
        private int CalculatePetExpReward(int consecutiveDays)
        {
            var baseExp = 5;
            var consecutiveBonus = Math.Min(consecutiveDays, 20); // 最多20點額外經驗
            return baseExp + consecutiveBonus;
        }

        /// <summary>
        /// 計算優惠券獎勵
        /// </summary>
        private string? CalculateCouponReward(int consecutiveDays)
        {
            // 每7天連續簽到獲得優惠券
            if (consecutiveDays % 7 == 0)
            {
                return $"COUPON_{consecutiveDays}";
            }
            return null;
        }

        /// <summary>
        /// 獲取當前用戶ID
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}