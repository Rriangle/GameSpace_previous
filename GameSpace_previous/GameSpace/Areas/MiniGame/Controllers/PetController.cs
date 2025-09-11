using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 寵物管理控制器 - 負責處理寵物飼養、屬性調整、互動功能
    /// 對應資料表：Pet
    /// </summary>
    [Area("MiniGame")]
    public class PetController : Controller
    {
        /// <summary>
        /// 寵物主頁面 - 顯示寵物狀態與基本資訊
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "我的寵物";
            
            // 模擬資料 - 對應 database.sql Pet 架構，實際邏輯將在 Stage 4 實作
            var viewModel = new PetStatusDisplayViewModel
            {
                Pet = new PetViewModel
                {
                    PetID = 1,
                    UserID = 1,
                    PetName = "小火龍",
                    Level = 5,
                    LevelUpTime = DateTime.Now.AddDays(-5),
                    Experience = 320,
                    Hunger = 80,
                    Mood = 75,
                    Stamina = 60,
                    Cleanliness = 90,
                    Health = 95,
                    SkinColor = "#FF6B35",
                    BackgroundColor = "#FFE5B4",
                    SkinColorChangedTime = DateTime.Now.AddDays(-10),
                    BackgroundColorChangedTime = DateTime.Now.AddDays(-15)
                },
                NextLevelExpRequired = 500,
                ExpProgressPercentage = 64.0, // 320/500 = 0.64
                HungerStatus = "飽足",
                MoodStatus = "愉快",
                OverallHealthScore = 80,
                NeedsCare = false,
                SuggestedCareActions = new List<string> { "建議陪玩增加心情", "補充體力" },
                RecentActivities = new List<PetActivityLogViewModel>
                {
                    new PetActivityLogViewModel { ActivityType = "餵食", Description = "餵食了美味的寵物糧食", ActivityTime = DateTime.Now.AddHours(-2), IconClass = "fas fa-utensils text-warning" },
                    new PetActivityLogViewModel { ActivityType = "陪玩", Description = "完成了一場愉快的遊戲", ActivityTime = DateTime.Now.AddHours(-5), IconClass = "fas fa-gamepad text-info" },
                    new PetActivityLogViewModel { ActivityType = "升級", Description = "等級提升到 Lv.5", ActivityTime = DateTime.Now.AddDays(-1), IconClass = "fas fa-level-up-alt text-success" }
                },
                LevelUpHistory = new List<PetLevelUpHistoryViewModel>
                {
                    new PetLevelUpHistoryViewModel { Level = 5, LevelUpTime = DateTime.Now.AddDays(-1), PointsReward = 25 },
                    new PetLevelUpHistoryViewModel { Level = 4, LevelUpTime = DateTime.Now.AddDays(-8), PointsReward = 20 }
                }
            };
            
            return View(viewModel);
        }

        /// <summary>
        /// 寵物詳細資訊頁面 - 顯示完整屬性與成長記錄
        /// </summary>
        public IActionResult Details(int id)
        {
            ViewData["Title"] = "寵物詳細資訊";
            ViewData["PetId"] = id;
            return View();
        }

        /// <summary>
        /// 寵物屬性調整頁面 - 設定外觀、顏色等可調整項目
        /// </summary>
        public IActionResult Customize(int id)
        {
            ViewData["Title"] = "寵物外觀設定";
            ViewData["PetId"] = id;
            return View();
        }

        /// <summary>
        /// 餵食寵物動作 - POST 方法
        /// </summary>
        [HttpPost]
        public IActionResult Feed(int id)
        {
            // 目前回傳成功訊息，實際邏輯待後續階段實作
            TempData["SuccessMessage"] = "餵食成功！寵物飢餓度 +20";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 與寵物玩耍動作 - POST 方法
        /// </summary>
        [HttpPost]
        public IActionResult Play(int id)
        {
            // 目前回傳成功訊息，實際邏輯待後續階段實作
            TempData["SuccessMessage"] = "陪玩成功！寵物心情 +15";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 取得寵物目前狀態 - AJAX 端點
        /// </summary>
        [HttpGet]
        public IActionResult GetPetStatus(int id)
        {
            // 目前回傳假資料，實際邏輯待後續階段實作
            return Json(new { 
                petId = id,
                name = "可愛小龍",
                level = 5,
                experience = 320,
                hunger = 80,
                mood = 75,
                stamina = 60,
                cleanliness = 90,
                health = 95
            });
        }
    }
}