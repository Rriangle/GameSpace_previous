using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GameSpace.Areas.MiniGame.Controllers
{
    [Area("MiniGame")]
    [Route("MiniGame/[controller]")]
    public class AdminMiniGameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminMiniGameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MiniGame/AdminMiniGame
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", string result = "", DateTime? dateFrom = null, DateTime? dateTo = null, int page = 1, int pageSize = 50)
        {
            ViewData["Title"] = "小遊戲記錄管理";
            ViewData["Description"] = "查看和分析所有小遊戲的記錄與統計";

            var gamesQuery = _context.MiniGame
                .Include(g => g.User)
                .Include(g => g.Pet)
                .AsNoTracking();

            // 搜尋功能
            if (!string.IsNullOrEmpty(searchTerm))
            {
                gamesQuery = gamesQuery.Where(g => 
                    g.User.User_name.Contains(searchTerm) ||
                    g.User.User_Account.Contains(searchTerm) ||
                    g.Pet.PetName.Contains(searchTerm));
                ViewData["SearchTerm"] = searchTerm;
            }

            // 結果篩選
            if (!string.IsNullOrEmpty(result))
            {
                gamesQuery = gamesQuery.Where(g => g.Result == result);
                ViewData["Result"] = result;
            }

            // 日期篩選
            if (dateFrom.HasValue)
            {
                gamesQuery = gamesQuery.Where(g => g.StartTime >= dateFrom.Value);
                ViewData["DateFrom"] = dateFrom.Value.ToString("yyyy-MM-dd");
            }

            if (dateTo.HasValue)
            {
                var dateToEnd = dateTo.Value.AddDays(1);
                gamesQuery = gamesQuery.Where(g => g.StartTime < dateToEnd);
                ViewData["DateTo"] = dateTo.Value.ToString("yyyy-MM-dd");
            }

            // 分頁
            var totalCount = await gamesQuery.CountAsync();
            var games = await gamesQuery
                .OrderByDescending(g => g.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new MiniGameRecordReadModel
                {
                    PlayID = g.PlayID,
                    UserName = g.User.User_name,
                    UserAccount = g.User.User_Account,
                    PetName = g.Pet.PetName,
                    Level = g.Level,
                    MonsterCount = g.MonsterCount,
                    SpeedMultiplier = g.SpeedMultiplier,
                    Result = g.Result,
                    ExpGained = g.ExpGained,
                    PointsChanged = g.PointsChanged,
                    CouponGained = g.CouponGained,
                    StartTime = g.StartTime,
                    EndTime = g.EndTime,
                    Aborted = g.Aborted,
                    Duration = g.EndTime.HasValue ? g.EndTime.Value - g.StartTime : (TimeSpan?)null
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            // 提供結果選項給篩選下拉選單
            ViewData["Results"] = new List<string> { "Win", "Lose", "Abort" };

            return View(games);
        }

        // GET: MiniGame/AdminMiniGame/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "遊戲記錄詳細";
            ViewData["Description"] = "查看遊戲記錄的詳細資訊";

            var game = await _context.MiniGame
                .Include(g => g.User)
                .Include(g => g.Pet)
                .AsNoTracking()
                .Where(g => g.PlayID == id)
                .Select(g => new MiniGameDetailReadModel
                {
                    PlayID = g.PlayID,
                    UserID = g.UserID,
                    UserName = g.User.User_name,
                    UserAccount = g.User.User_Account,
                    PetID = g.PetID,
                    PetName = g.Pet.PetName,
                    PetLevel = g.Pet.Level,
                    Level = g.Level,
                    MonsterCount = g.MonsterCount,
                    SpeedMultiplier = g.SpeedMultiplier,
                    Result = g.Result,
                    ExpGained = g.ExpGained,
                    ExpGainedTime = g.ExpGainedTime,
                    PointsChanged = g.PointsChanged,
                    PointsChangedTime = g.PointsChangedTime,
                    CouponGained = g.CouponGained,
                    CouponGainedTime = g.CouponGainedTime,
                    HungerDelta = g.HungerDelta,
                    MoodDelta = g.MoodDelta,
                    StaminaDelta = g.StaminaDelta,
                    CleanlinessDelta = g.CleanlinessDelta,
                    StartTime = g.StartTime,
                    EndTime = g.EndTime,
                    Aborted = g.Aborted,
                    Duration = g.EndTime.HasValue ? g.EndTime.Value - g.StartTime : (TimeSpan?)null
                })
                .FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound($"找不到ID {id} 的遊戲記錄");
            }

            return View(game);
        }

        // GET: MiniGame/AdminMiniGame/Settings
        [HttpGet("Settings")]
        public async Task<IActionResult> Settings()
        {
            ViewData["Title"] = "遊戲設定管理";
            ViewData["Description"] = "管理小遊戲的規則與參數設定";

            // 這是一個 Stub 實現，顯示遊戲設定的概要
            var settings = new MiniGameSettingsViewModel
            {
                MaxDailyPlays = 3,
                LevelConfigurations = new List<GameLevelConfig>
                {
                    new GameLevelConfig { Level = 1, MonsterCount = 6, SpeedMultiplier = 1.0m, ExpReward = 100, PointReward = 10 },
                    new GameLevelConfig { Level = 2, MonsterCount = 8, SpeedMultiplier = 1.5m, ExpReward = 200, PointReward = 20 },
                    new GameLevelConfig { Level = 3, MonsterCount = 10, SpeedMultiplier = 2.0m, ExpReward = 300, PointReward = 30 }
                }
            };

            // 從資料庫取得實際的統計資料
            var totalGames = await _context.MiniGame.CountAsync();
            var winRate = totalGames > 0 ? 
                (double)await _context.MiniGame.CountAsync(g => g.Result == "Win") / totalGames * 100 : 0;
            var abortRate = totalGames > 0 ?
                (double)await _context.MiniGame.CountAsync(g => g.Aborted) / totalGames * 100 : 0;

            settings.TotalGamesPlayed = totalGames;
            settings.OverallWinRate = Math.Round(winRate, 2);
            settings.OverallAbortRate = Math.Round(abortRate, 2);

            return View(settings);
        }

        // GET: MiniGame/AdminMiniGame/Statistics
        [HttpGet("Statistics")]
        public async Task<IActionResult> Statistics()
        {
            ViewData["Title"] = "遊戲統計分析";
            ViewData["Description"] = "查看小遊戲的詳細統計與趨勢分析";

            var stats = new MiniGameStatisticsViewModel();

            // 今日遊戲統計
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            stats.TodayGamesCount = await _context.MiniGame
                .CountAsync(g => g.StartTime >= today && g.StartTime < tomorrow);

            // 本週遊戲統計
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            stats.ThisWeekGamesCount = await _context.MiniGame
                .CountAsync(g => g.StartTime >= weekStart);

            // 本月遊戲統計
            var monthStart = new DateTime(today.Year, today.Month, 1);
            stats.ThisMonthGamesCount = await _context.MiniGame
                .CountAsync(g => g.StartTime >= monthStart);

            // 總遊戲次數
            stats.TotalGamesCount = await _context.MiniGame.CountAsync();

            // 勝率統計
            if (stats.TotalGamesCount > 0)
            {
                var winCount = await _context.MiniGame.CountAsync(g => g.Result == "Win");
                var loseCount = await _context.MiniGame.CountAsync(g => g.Result == "Lose");
                var abortCount = await _context.MiniGame.CountAsync(g => g.Result == "Abort");

                stats.WinRate = Math.Round((double)winCount / stats.TotalGamesCount * 100, 2);
                stats.LoseRate = Math.Round((double)loseCount / stats.TotalGamesCount * 100, 2);
                stats.AbortRate = Math.Round((double)abortCount / stats.TotalGamesCount * 100, 2);
            }

            // 關卡統計
            stats.LevelStats = await _context.MiniGame
                .GroupBy(g => g.Level)
                .Select(g => new LevelStats
                {
                    Level = g.Key,
                    TotalPlays = g.Count(),
                    WinCount = g.Count(x => x.Result == "Win"),
                    LoseCount = g.Count(x => x.Result == "Lose"),
                    AbortCount = g.Count(x => x.Result == "Abort"),
                    AvgDuration = g.Where(x => x.EndTime.HasValue)
                                   .Average(x => EF.Functions.DateDiffSecond(x.StartTime, x.EndTime.Value))
                })
                .OrderBy(ls => ls.Level)
                .ToListAsync();

            // 計算關卡勝率
            foreach (var levelStat in stats.LevelStats)
            {
                if (levelStat.TotalPlays > 0)
                {
                    levelStat.WinRate = Math.Round((double)levelStat.WinCount / levelStat.TotalPlays * 100, 2);
                }
            }

            // 最近7天遊戲趨勢
            stats.Last7DaysTrend = new List<DailyGameTrend>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var nextDate = date.AddDays(1);
                var count = await _context.MiniGame
                    .CountAsync(g => g.StartTime >= date && g.StartTime < nextDate);

                stats.Last7DaysTrend.Add(new DailyGameTrend
                {
                    Date = date,
                    GameCount = count
                });
            }

            // 獎勵發放統計
            stats.TotalExpAwarded = await _context.MiniGame
                .Where(g => g.ExpGained > 0)
                .SumAsync(g => g.ExpGained);

            stats.TotalPointsAwarded = await _context.MiniGame
                .Where(g => g.PointsChanged > 0)
                .SumAsync(g => g.PointsChanged);

            stats.TotalCouponsAwarded = await _context.MiniGame
                .CountAsync(g => !string.IsNullOrEmpty(g.CouponGained) && g.CouponGained != "0");

            return View(stats);
        }

        // POST: MiniGame/AdminMiniGame/UpdateSettings (Stub for future implementation)
        [HttpPost("UpdateSettings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(MiniGameSettingsViewModel settings)
        {
            // 這是一個 Stub 實現，實際的設定更新功能將在後續階段實現
            // 目前只返回成功訊息，不實際修改設定
            
            TempData["InfoMessage"] = "遊戲設定更新功能尚未完全實現。設定將在後續階段開放修改。";
            
            return RedirectToAction(nameof(Settings));
        }
    }

    // Read Models and ViewModels
    public class MiniGameRecordReadModel
    {
        public int PlayID { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string PetName { get; set; }
        public int Level { get; set; }
        public int MonsterCount { get; set; }
        public decimal SpeedMultiplier { get; set; }
        public string Result { get; set; }
        public int ExpGained { get; set; }
        public int PointsChanged { get; set; }
        public string CouponGained { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Aborted { get; set; }
        public TimeSpan? Duration { get; set; }
    }

    public class MiniGameDetailReadModel : MiniGameRecordReadModel
    {
        public int UserID { get; set; }
        public int PetID { get; set; }
        public int PetLevel { get; set; }
        public DateTime? ExpGainedTime { get; set; }
        public DateTime? PointsChangedTime { get; set; }
        public DateTime CouponGainedTime { get; set; }
        public int HungerDelta { get; set; }
        public int MoodDelta { get; set; }
        public int StaminaDelta { get; set; }
        public int CleanlinessDelta { get; set; }
    }

    public class MiniGameSettingsViewModel
    {
        public int MaxDailyPlays { get; set; }
        public List<GameLevelConfig> LevelConfigurations { get; set; } = new List<GameLevelConfig>();
        public int TotalGamesPlayed { get; set; }
        public double OverallWinRate { get; set; }
        public double OverallAbortRate { get; set; }
    }

    public class GameLevelConfig
    {
        public int Level { get; set; }
        public int MonsterCount { get; set; }
        public decimal SpeedMultiplier { get; set; }
        public int ExpReward { get; set; }
        public int PointReward { get; set; }
    }

    public class MiniGameStatisticsViewModel
    {
        public int TodayGamesCount { get; set; }
        public int ThisWeekGamesCount { get; set; }
        public int ThisMonthGamesCount { get; set; }
        public int TotalGamesCount { get; set; }
        public double WinRate { get; set; }
        public double LoseRate { get; set; }
        public double AbortRate { get; set; }
        public int TotalExpAwarded { get; set; }
        public int TotalPointsAwarded { get; set; }
        public int TotalCouponsAwarded { get; set; }
        public List<LevelStats> LevelStats { get; set; } = new List<LevelStats>();
        public List<DailyGameTrend> Last7DaysTrend { get; set; } = new List<DailyGameTrend>();
    }

    public class LevelStats
    {
        public int Level { get; set; }
        public int TotalPlays { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public int AbortCount { get; set; }
        public double WinRate { get; set; }
        public double? AvgDuration { get; set; }
    }

    public class DailyGameTrend
    {
        public DateTime Date { get; set; }
        public int GameCount { get; set; }
    }
}