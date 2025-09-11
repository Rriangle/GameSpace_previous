using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 使用者簽到統計控制器 - 負責處理每日簽到與統計功能
    /// 對應資料表：UserSignInStats
    /// </summary>
    [Area("MiniGame")]
    public class UserSignInStatsController : Controller
    {
        /// <summary>
        /// 簽到主頁面 - 顯示簽到狀態與連續簽到天數
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "每日簽到";
            return View();
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
        /// 執行簽到動作 - POST 方法
        /// </summary>
        [HttpPost]
        public IActionResult SignIn()
        {
            // 目前回傳成功訊息，實際邏輯待後續階段實作
            TempData["SuccessMessage"] = "簽到成功！獲得積分 +10";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 取得今日簽到狀態 - AJAX 端點
        /// </summary>
        [HttpGet]
        public IActionResult GetTodayStatus()
        {
            // 目前回傳假資料，實際邏輯待後續階段實作
            return Json(new { 
                hasSignedToday = false, 
                consecutiveDays = 3,
                todayPoints = 10 
            });
        }
    }
}