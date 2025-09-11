using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;
using GameSpace.Areas.MiniGame.Services;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 使用者簽到統計控制器 - 負責處理每日簽到與統計功能
    /// 對應資料表：UserSignInStats
    /// </summary>
    [Area("MiniGame")]
    public class UserSignInStatsController : Controller
    {
        private readonly IUserSignInService _signInService;

        public UserSignInStatsController(IUserSignInService signInService)
        {
            _signInService = signInService;
        }
        /// <summary>
        /// 簽到主頁面 - 顯示簽到狀態與連續簽到天數
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "每日簽到";
            
            // 使用簽到服務取得實際統計資料 - Stage 4 實作
            const int currentUserId = 1; // 實際會從認證系統取得
            var viewModel = await _signInService.GetSignInStatsAsync(currentUserId);
            
            return View(viewModel);
        }

        /// <summary>
        /// 簽到統計頁面 - 顯示歷史簽到記錄與獲得獎勵統計
        /// </summary>
        public IActionResult Statistics()
        {
            ViewData["Title"] = "簽到統計";
            return View();
        }

        /// <summary>
        /// 執行簽到動作 - POST 方法，實作真實簽到流程
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SignIn()
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            
            // 執行簽到作業 - 使用服務層處理業務邏輯
            var result = await _signInService.ProcessSignInAsync(currentUserId);
            
            if (result.Success)
            {
                var message = $"{result.Message}！獲得積分 +{result.PointsGained}，經驗值 +{result.ExpGained}";
                
                if (result.HasBonusReward)
                {
                    message += $"，額外獲得：{result.BonusDescription}";
                    if (!string.IsNullOrEmpty(result.BonusCouponCode))
                    {
                        message += $"，優惠券代碼：{result.BonusCouponCode}";
                    }
                }
                
                TempData["SuccessMessage"] = message;
                TempData["SignInResult"] = System.Text.Json.JsonSerializer.Serialize(result);
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 取得今日簽到狀態 - AJAX 端點，實作真實狀態查詢
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTodayStatus()
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            
            // 使用服務查詢真實簽到狀態
            var hasSignedToday = await _signInService.HasSignedTodayAsync(currentUserId);
            var consecutiveDays = await _signInService.GetConsecutiveDaysAsync(currentUserId);
            var todayPoints = 10 + (consecutiveDays / 7 * 5); // 基於連續天數計算獎勵
            
            return Json(new { 
                hasSignedToday = hasSignedToday, 
                consecutiveDays = consecutiveDays,
                todayPoints = todayPoints,
                expReward = 5 + (consecutiveDays / 7 * 2)
            });
        }
    }
}