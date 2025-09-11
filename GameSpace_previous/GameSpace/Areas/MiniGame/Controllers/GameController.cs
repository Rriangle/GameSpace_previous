using Microsoft.AspNetCore.Mvc;

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
            return View();
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