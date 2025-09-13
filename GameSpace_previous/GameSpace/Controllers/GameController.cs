using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 小遊戲控制器
    /// </summary>
    public class GameController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<GameController> _logger;

        public GameController(GameSpaceDbContext context, ILogger<GameController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示小遊戲頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // 獲取用戶的遊戲記錄
            var gameRecords = await _context.MiniGame
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.StartTime)
                .Take(10)
                .ToListAsync();

            // 獲取今日遊戲次數
            var today = DateTime.UtcNow.Date;
            var todayGames = await _context.MiniGame
                .CountAsync(m => m.UserId == userId && m.StartTime.Date == today);

            ViewBag.TodayGames = todayGames;
            ViewBag.MaxGames = 3;
            ViewBag.GameRecords = gameRecords;

            return View();
        }

        /// <summary>
        /// 開始小遊戲
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartGame()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            // 檢查今日遊戲次數
            var today = DateTime.UtcNow.Date;
            var todayGames = await _context.MiniGame
                .CountAsync(m => m.UserId == userId && m.StartTime.Date == today);

            if (todayGames >= 3)
            {
                return Json(new { success = false, message = "今日遊戲次數已用完，明天再來吧！" });
            }

            // 獲取用戶的寵物
            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return Json(new { success = false, message = "請先創建寵物" });
            }

            // 創建遊戲記錄
            var gameRecord = new MiniGame
            {
                UserId = userId.Value,
                PetId = pet.PetId,
                Level = 1,
                MonsterCount = 5,
                SpeedMultiplier = 1.0m,
                Result = "進行中",
                ExpGained = 0,
                ExpGainedTime = DateTime.UtcNow,
                PointsGained = 0,
                PointsGainedTime = DateTime.UtcNow,
                CouponGained = "",
                CouponGainedTime = DateTime.UtcNow,
                HungerDelta = 0,
                MoodDelta = 0,
                StaminaDelta = 0,
                CleanlinessDelta = 0,
                StartTime = DateTime.UtcNow,
                EndTime = null,
                Aborted = false
            };

            _context.MiniGame.Add(gameRecord);
            await _context.SaveChangesAsync();

            return Json(new { success = true, gameId = gameRecord.PlayId, message = "遊戲開始！" });
        }

        /// <summary>
        /// 結束遊戲並記錄分數
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndGame(int gameId, int score, int duration)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var gameRecord = await _context.MiniGame
                .FirstOrDefaultAsync(m => m.PlayId == gameId && m.UserId == userId);

            if (gameRecord == null)
            {
                return Json(new { success = false, message = "找不到遊戲記錄" });
            }

            // 計算獎勵
            var expGained = score / 10;
            var pointsGained = score / 5;
            var hungerDelta = -Math.Max(1, score / 50);
            var moodDelta = Math.Max(1, score / 20);
            var staminaDelta = -Math.Max(1, score / 30);
            var cleanlinessDelta = -Math.Max(1, score / 40);

            // 更新遊戲記錄
            gameRecord.Result = "勝利";
            gameRecord.ExpGained = expGained;
            gameRecord.ExpGainedTime = DateTime.UtcNow;
            gameRecord.PointsGained = pointsGained;
            gameRecord.PointsGainedTime = DateTime.UtcNow;
            gameRecord.HungerDelta = hungerDelta;
            gameRecord.MoodDelta = moodDelta;
            gameRecord.StaminaDelta = staminaDelta;
            gameRecord.CleanlinessDelta = cleanlinessDelta;
            gameRecord.EndTime = DateTime.UtcNow;

            // 更新寵物屬性
            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.PetId == gameRecord.PetId);

            if (pet != null)
            {
                pet.Experience += expGained;
                pet.Hunger = Math.Max(0, Math.Min(100, pet.Hunger + hungerDelta));
                pet.Mood = Math.Max(0, Math.Min(100, pet.Mood + moodDelta));
                pet.Stamina = Math.Max(0, Math.Min(100, pet.Stamina + staminaDelta));
                pet.Cleanliness = Math.Max(0, Math.Min(100, pet.Cleanliness + cleanlinessDelta));
            }

            // 更新用戶錢包
            var wallet = await _context.UserWallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet != null)
            {
                wallet.UserPoint += pointsGained;
                wallet.UpdatedAt = DateTime.UtcNow;

                // 記錄錢包歷史
                var walletHistory = new WalletHistory
                {
                    UserId = userId.Value,
                    TransactionType = "遊戲獎勵",
                    Amount = pointsGained,
                    BalanceBefore = wallet.UserPoint - pointsGained,
                    BalanceAfter = wallet.UserPoint,
                    Description = $"小遊戲獲得 {pointsGained} 點",
                    ReferenceId = gameRecord.PlayId.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WalletHistories.Add(walletHistory);
            }

            await _context.SaveChangesAsync();

            // 計算排名（基於經驗值）
            var rank = await _context.MiniGame
                .CountAsync(m => m.ExpGained > expGained) + 1;

            return Json(new { 
                success = true, 
                message = $"遊戲結束！您的分數是 {score} 分，獲得 {expGained} 經驗和 {pointsGained} 點數！",
                score = score,
                rank = rank,
                expGained = expGained,
                pointsGained = pointsGained
            });
        }

        /// <summary>
        /// 獲取排行榜
        /// </summary>
        public async Task<IActionResult> Leaderboard()
        {
            var leaderboard = await _context.MiniGame
                .Where(m => m.Result == "勝利")
                .OrderByDescending(m => m.ExpGained)
                .Take(20)
                .Select(m => new {
                    UserName = m.User.UserName,
                    Score = m.ExpGained,
                    PointsGained = m.PointsGained,
                    StartTime = m.StartTime
                })
                .ToListAsync();

            return Json(leaderboard);
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