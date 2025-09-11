using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;
using GameSpace.Areas.MiniGame.Services;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 小遊戲控制器 - 負責處理遊戲邏輯、記錄、獎勵發放
    /// 對應資料表：MiniGame
    /// </summary>
    [Area("MiniGame")]
    public class GameController : Controller
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }
        /// <summary>
        /// 遊戲大廳頁面 - 顯示可用遊戲列表與個人遊戲統計
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "小遊戲大廳";
            
            const int currentUserId = 1; // 實際會從認證系統取得
            
            // 使用遊戲服務取得實際大廳資料
            var viewModel = await _gameService.GetGameHallDataAsync(currentUserId);
            
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
        /// 提交遊戲結果 - POST 方法，實作真實遊戲結果處理
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SubmitResult([FromBody] GameResultSubmissionModel model)
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            const int petId = 1; // 實際會查詢使用者目前選擇的寵物

            try
            {
                // 使用遊戲服務處理結果提交
                var result = await _gameService.SubmitGameResultAsync(model, currentUserId, petId);
                
                if (result.Success)
                {
                    var response = new
                    {
                        success = true,
                        message = result.Message,
                        playRecordId = result.PlayRecordId,
                        pointsEarned = result.PointsGained,
                        expGained = result.ExpGained,
                        gameRank = result.GameRank,
                        newHighScore = result.NewHighScore,
                        totalPoints = result.TotalUserPoints,
                        bonusCoupon = result.BonusCouponCode,
                        petChanges = new
                        {
                            hungerDelta = result.PetChanges.HungerDelta,
                            moodDelta = result.PetChanges.MoodDelta,
                            staminaDelta = result.PetChanges.StaminaDelta,
                            cleanlinessDelta = result.PetChanges.CleanlinessDelta,
                            experienceDelta = result.PetChanges.ExperienceDelta
                        },
                        petLevelUp = result.PetLevelUpTriggered ? new
                        {
                            newLevel = result.PetLevelUpReward?.NewLevel,
                            pointsReward = result.PetLevelUpReward?.PointsReward,
                            message = result.PetLevelUpReward?.Message
                        } : null
                    };

                    return Json(response);
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = result.Message 
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "遊戲結果提交發生錯誤" 
                });
            }
        }

        /// <summary>
        /// 取得遊戲列表 - AJAX 端點，使用服務層查詢
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAvailableGames()
        {
            try
            {
                var games = await _gameService.GetAvailableGamesAsync();
                
                var response = games.Select(g => new 
                { 
                    id = g.GameId, 
                    name = g.GameName, 
                    description = g.Description, 
                    pointsReward = g.PointsReward,
                    difficultyLevel = g.DifficultyLevel,
                    iconClass = g.IconClass,
                    isEnabled = g.IsEnabled
                });
                
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "取得遊戲列表失敗" });
            }
        }
    }
}