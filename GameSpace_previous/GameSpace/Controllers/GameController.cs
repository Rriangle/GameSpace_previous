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
                .OrderByDescending(m => m.CreatedAt)
                .Take(10)
                .ToListAsync();

            // 獲取今日遊戲次數
            var today = DateTime.UtcNow.Date;
            var todayGames = await _context.MiniGame
                .CountAsync(m => m.UserId == userId && m.CreatedAt.Date == today);

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
                .CountAsync(m => m.UserId == userId && m.CreatedAt.Date == today);

            if (todayGames >= 3)
            {
                return Json(new { success = false, message = "今日遊戲次數已用完，明天再來吧！" });
            }

            // 創建遊戲記錄
            var gameRecord = new MiniGame
            {
                UserId = userId.Value,
                GameType = "冒險挑戰",
                Score = 0,
                Duration = 0,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MiniGame.Add(gameRecord);
            await _context.SaveChangesAsync();

            return Json(new { success = true, gameId = gameRecord.GameId, message = "遊戲開始！" });
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
                .FirstOrDefaultAsync(m => m.GameId == gameId && m.UserId == userId);

            if (gameRecord == null)
            {
                return Json(new { success = false, message = "找不到遊戲記錄" });
            }

            // 更新遊戲記錄
            gameRecord.Score = score;
            gameRecord.Duration = duration;
            gameRecord.IsCompleted = true;
            gameRecord.UpdatedAt = DateTime.UtcNow;

            // 更新寵物經驗值
            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet != null)
            {
                pet.Experience += score / 10; // 每10分獲得1經驗
                pet.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // 計算排名
            var rank = await _context.MiniGame
                .CountAsync(m => m.Score > score) + 1;

            return Json(new { 
                success = true, 
                message = $"遊戲結束！您的分數是 {score} 分，排名第 {rank} 名！",
                score = score,
                rank = rank
            });
        }

        /// <summary>
        /// 獲取排行榜
        /// </summary>
        public async Task<IActionResult> Leaderboard()
        {
            var leaderboard = await _context.MiniGame
                .Where(m => m.IsCompleted)
                .OrderByDescending(m => m.Score)
                .Take(20)
                .Select(m => new {
                    UserName = m.User.UserName,
                    Score = m.Score,
                    Duration = m.Duration,
                    CreatedAt = m.CreatedAt
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