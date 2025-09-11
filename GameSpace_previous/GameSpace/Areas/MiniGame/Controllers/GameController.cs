using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 小遊戲控制器 - 負責處理遊戲邏輯、記錄、獎勵發放
    /// 對應資料表：MiniGame
    /// </summary>
    [Area("MiniGame")]
    public class GameController : Controller
    {
        /// <summary>
        /// 遊戲大廳頁面 - 顯示可用遊戲列表與個人遊戲統計
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "小遊戲大廳";
            
            // 模擬資料 - 對應 database.sql MiniGame 架構，實際邏輯將在 Stage 4 實作
            var viewModel = new GameHallDisplayViewModel
            {
                UserId = 1,
                TodayGameCount = 3,
                TodayPointsEarned = 150,
                HighestScore = 2450,
                CurrentGameLevel = 12,
                AvailableGames = new List<AvailableGameViewModel>
                {
                    new AvailableGameViewModel { GameId = 1, GameName = "打怪獸", Description = "經典射擊遊戲，考驗反應速度與準確度", PointsReward = 50, DifficultyLevel = 3, IconClass = "fas fa-crosshairs text-danger", IsEnabled = true },
                    new AvailableGameViewModel { GameId = 2, GameName = "寵物冒險", Description = "策略冒險遊戲，與寵物一起探索未知世界", PointsReward = 75, DifficultyLevel = 4, IconClass = "fas fa-map text-success", IsEnabled = true },
                    new AvailableGameViewModel { GameId = 3, GameName = "記憶挑戰", Description = "記憶力訓練遊戲，提升專注力與記憶能力", PointsReward = 30, DifficultyLevel = 2, IconClass = "fas fa-brain text-warning", IsEnabled = true }
                },
                RecentGameHistory = new List<MiniGameViewModel>
                {
                    new MiniGameViewModel { PlayID = 1, Level = 5, Result = "勝利", ExpGained = 25, PointsGained = 50, StartTime = DateTime.Now.AddHours(-3), EndTime = DateTime.Now.AddHours(-3).AddMinutes(3).AddSeconds(45) },
                    new MiniGameViewModel { PlayID = 2, Level = 3, Result = "勝利", ExpGained = 15, PointsGained = 30, StartTime = DateTime.Now.AddHours(-6), EndTime = DateTime.Now.AddHours(-6).AddMinutes(2).AddSeconds(30) },
                    new MiniGameViewModel { PlayID = 3, Level = 8, Result = "勝利", ExpGained = 40, PointsGained = 75, StartTime = DateTime.Now.AddDays(-1), EndTime = DateTime.Now.AddDays(-1).AddMinutes(8).AddSeconds(20) }
                },
                WeeklyStats = new WeeklyGameStatsViewModel
                {
                    WeeklyGameCount = 15,
                    WeeklyPointsEarned = 720,
                    WeeklyHighScore = 2450,
                    AveragePlayTimeMinutes = 4
                }
            };
            
            return View(viewModel);
        }

        /// <summary>
        /// 遊戲詳細頁面 - 顯示特定遊戲規則與歷史記錄
        /// </summary>
        public IActionResult Details(int id)
        {
            ViewData["Title"] = "遊戲詳細資訊";
            ViewData["GameId"] = id;
            return View();
        }

        /// <summary>
        /// 開始遊戲頁面 - 遊戲進行介面
        /// </summary>
        public IActionResult Play(int id)
        {
            ViewData["Title"] = "遊戲進行中";
            ViewData["GameId"] = id;
            return View();
        }

        /// <summary>
        /// 遊戲記錄頁面 - 顯示個人遊戲歷史與成就
        /// </summary>
        public IActionResult History()
        {
            ViewData["Title"] = "遊戲記錄";
            return View();
        }

        /// <summary>
        /// 提交遊戲結果 - POST 方法
        /// </summary>
        [HttpPost]
        public IActionResult SubmitResult([FromBody] GameResultModel model)
        {
            // 目前回傳成功訊息，實際邏輯待後續階段實作
            return Json(new { 
                success = true, 
                message = "遊戲結果已記錄",
                pointsEarned = model.Score * 10,
                expGained = model.Score * 5
            });
        }

        /// <summary>
        /// 取得遊戲列表 - AJAX 端點
        /// </summary>
        [HttpGet]
        public IActionResult GetAvailableGames()
        {
            // 目前回傳假資料，實際邏輯待後續階段實作
            var games = new[]
            {
                new { id = 1, name = "打怪獸", description = "經典射擊遊戲", pointsReward = 50 },
                new { id = 2, name = "寵物冒險", description = "策略冒險遊戲", pointsReward = 75 },
                new { id = 3, name = "記憶挑戰", description = "記憶力訓練遊戲", pointsReward = 30 }
            };
            
            return Json(games);
        }
    }

    /// <summary>
    /// 遊戲結果提交模型
    /// </summary>
    public class GameResultModel
    {
        public int GameId { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public bool IsCompleted { get; set; }
        public int Duration { get; set; } // 遊戲時間（秒）
    }
}