using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;
using GameSpace.Services;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 遊戲熱度追蹤控制器
    /// </summary>
    public class GameHeatController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<GameHeatController> _logger;
        private readonly GameHeatTrackingService _heatTrackingService;

        public GameHeatController(GameSpaceDbContext context, ILogger<GameHeatController> logger, GameHeatTrackingService heatTrackingService)
        {
            _context = context;
            _logger = logger;
            _heatTrackingService = heatTrackingService;
        }

        /// <summary>
        /// 顯示遊戲熱度排行榜
        /// </summary>
        public async Task<IActionResult> Index(string snapshotType = "Daily", int page = 1)
        {
            var leaderboard = await _heatTrackingService.GetLeaderboardAsync(snapshotType, page, 20);
            
            ViewBag.SnapshotType = snapshotType;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = await GetTotalPagesAsync(snapshotType, 20);
            
            return View(leaderboard);
        }

        /// <summary>
        /// 顯示遊戲熱度趨勢
        /// </summary>
        public async Task<IActionResult> Trend(int gameId, int days = 30)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
            {
                return NotFound();
            }

            var trend = await _heatTrackingService.GetHeatTrendAsync(gameId, days);
            
            ViewBag.Game = game;
            ViewBag.Days = days;
            
            return View(trend);
        }

        /// <summary>
        /// 顯示遊戲詳細指標
        /// </summary>
        public async Task<IActionResult> Details(int gameId, DateTime? date = null)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
            {
                return NotFound();
            }

            var targetDate = date ?? DateTime.UtcNow.Date;
            
            var metrics = await _context.GameMetricDailies
                .Include(m => m.Source)
                .Where(m => m.GameId == gameId && m.MetricDate == targetDate)
                .ToListAsync();

            ViewBag.Game = game;
            ViewBag.TargetDate = targetDate;
            
            return View(metrics);
        }

        /// <summary>
        /// 管理遊戲清單
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageGames()
        {
            var games = await _context.Games
                .OrderBy(g => g.GameName)
                .ToListAsync();
            
            return View(games);
        }

        /// <summary>
        /// 管理指標來源
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageSources()
        {
            var sources = await _context.MetricSources
                .OrderBy(s => s.SourceName)
                .ToListAsync();
            
            return View(sources);
        }

        /// <summary>
        /// 創建新遊戲
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGame(CreateGameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    GameName = model.GameName,
                    GameDescription = model.GameDescription,
                    GameUrl = model.GameUrl,
                    GameImageUrl = model.GameImageUrl,
                    GameCategory = model.GameCategory,
                    GamePlatform = model.GamePlatform,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                _logger.LogInformation("管理員創建新遊戲: {GameName}", game.GameName);
                return RedirectToAction(nameof(ManageGames));
            }

            return View(model);
        }

        /// <summary>
        /// 創建新指標來源
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSource(CreateSourceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var source = new MetricSource
                {
                    SourceName = model.SourceName,
                    SourceType = model.SourceType,
                    ApiEndpoint = model.ApiEndpoint,
                    ApiKey = model.ApiKey,
                    Description = model.Description,
                    IsActive = true,
                    UpdateFrequency = model.UpdateFrequency,
                    LastUpdated = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.MetricSources.Add(source);
                await _context.SaveChangesAsync();

                _logger.LogInformation("管理員創建新指標來源: {SourceName}", source.SourceName);
                return RedirectToAction(nameof(ManageSources));
            }

            return View(model);
        }

        /// <summary>
        /// 手動更新遊戲指標
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMetrics(int gameId, int sourceId)
        {
            var data = await _heatTrackingService.FetchExternalDataAsync(sourceId, gameId);
            if (data != null)
            {
                var success = await _heatTrackingService.UpdateDailyMetricsAsync(gameId, sourceId, data);
                if (success)
                {
                    TempData["SuccessMessage"] = "指標更新成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "指標更新失敗";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "無法獲取外部數據";
            }

            return RedirectToAction(nameof(Details), new { gameId });
        }

        /// <summary>
        /// 生成排行榜快照
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateSnapshot(string snapshotType)
        {
            await _heatTrackingService.GenerateLeaderboardSnapshotAsync(snapshotType);
            TempData["SuccessMessage"] = $"{snapshotType} 排行榜快照生成成功";
            
            return RedirectToAction(nameof(Index), new { snapshotType });
        }

        /// <summary>
        /// 獲取總頁數
        /// </summary>
        private async Task<int> GetTotalPagesAsync(string snapshotType, int pageSize)
        {
            var today = DateTime.UtcNow.Date;
            var totalCount = await _context.LeaderboardSnapshots
                .Where(s => s.SnapshotType == snapshotType && s.SnapshotDate == today)
                .CountAsync();
            
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }
    }

    /// <summary>
    /// 創建遊戲視圖模型
    /// </summary>
    public class CreateGameViewModel
    {
        public string GameName { get; set; } = string.Empty;
        public string? GameDescription { get; set; }
        public string? GameUrl { get; set; }
        public string? GameImageUrl { get; set; }
        public string? GameCategory { get; set; }
        public string? GamePlatform { get; set; }
    }

    /// <summary>
    /// 創建指標來源視圖模型
    /// </summary>
    public class CreateSourceViewModel
    {
        public string SourceName { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string? ApiEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? Description { get; set; }
        public int UpdateFrequency { get; set; } = 60; // 預設60分鐘
    }
}