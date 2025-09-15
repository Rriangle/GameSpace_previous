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
    public class AdminSignInStatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSignInStatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MiniGame/AdminSignInStats
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", DateTime? dateFrom = null, DateTime? dateTo = null, int page = 1, int pageSize = 50)
        {
            ViewData["Title"] = "簽到統計管理";
            ViewData["Description"] = "查看和分析會員簽到統計資料";

            var statsQuery = _context.UserSignInStats
                .Include(s => s.User)
                .AsNoTracking();

            // 搜尋功能
            if (!string.IsNullOrEmpty(searchTerm))
            {
                statsQuery = statsQuery.Where(s => 
                    s.User.User_name.Contains(searchTerm) ||
                    s.User.User_Account.Contains(searchTerm));
                ViewData["SearchTerm"] = searchTerm;
            }

            // 日期篩選
            if (dateFrom.HasValue)
            {
                statsQuery = statsQuery.Where(s => s.SignTime >= dateFrom.Value);
                ViewData["DateFrom"] = dateFrom.Value.ToString("yyyy-MM-dd");
            }

            if (dateTo.HasValue)
            {
                var dateToEnd = dateTo.Value.AddDays(1); // 包含整天
                statsQuery = statsQuery.Where(s => s.SignTime < dateToEnd);
                ViewData["DateTo"] = dateTo.Value.ToString("yyyy-MM-dd");
            }

            // 分頁
            var totalCount = await statsQuery.CountAsync();
            var signInStats = await statsQuery
                .OrderByDescending(s => s.SignTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SignInStatsReadModel
                {
                    LogID = s.LogID,
                    UserName = s.User.User_name,
                    UserAccount = s.User.User_Account,
                    SignTime = s.SignTime,
                    PointsChanged = s.PointsChanged,
                    ExpGained = s.ExpGained,
                    CouponGained = s.CouponGained,
                    PointsChangedTime = s.PointsChangedTime,
                    ExpGainedTime = s.ExpGainedTime,
                    CouponGainedTime = s.CouponGainedTime
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(signInStats);
        }

        // GET: MiniGame/AdminSignInStats/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "簽到記錄詳細";
            ViewData["Description"] = "查看簽到記錄的詳細資訊";

            var signInStat = await _context.UserSignInStats
                .Include(s => s.User)
                .AsNoTracking()
                .Where(s => s.LogID == id)
                .Select(s => new SignInStatsDetailReadModel
                {
                    LogID = s.LogID,
                    UserID = s.UserID,
                    UserName = s.User.User_name,
                    UserAccount = s.User.User_Account,
                    SignTime = s.SignTime,
                    PointsChanged = s.PointsChanged,
                    ExpGained = s.ExpGained,
                    CouponGained = s.CouponGained,
                    PointsChangedTime = s.PointsChangedTime,
                    ExpGainedTime = s.ExpGainedTime,
                    CouponGainedTime = s.CouponGainedTime
                })
                .FirstOrDefaultAsync();

            if (signInStat == null)
            {
                return NotFound($"找不到ID {id} 的簽到記錄");
            }

            // 取得該用戶的簽到統計摘要
            signInStat.UserSignInSummary = await GetUserSignInSummary(signInStat.UserID);

            return View(signInStat);
        }

        // GET: MiniGame/AdminSignInStats/UserStats/{userId}
        [HttpGet("UserStats/{userId}")]
        public async Task<IActionResult> UserStats(int userId, int page = 1, int pageSize = 30)
        {
            ViewData["Title"] = "用戶簽到統計";
            ViewData["Description"] = "查看特定用戶的簽到歷史與統計";

            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.User_ID == userId)
                .Select(u => new { u.User_name, u.User_Account })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound($"找不到用戶ID {userId}");
            }

            ViewData["UserName"] = user.User_name;
            ViewData["UserAccount"] = user.User_Account;

            // 取得用戶簽到記錄
            var totalCount = await _context.UserSignInStats
                .Where(s => s.UserID == userId)
                .CountAsync();

            var userSignInStats = await _context.UserSignInStats
                .AsNoTracking()
                .Where(s => s.UserID == userId)
                .OrderByDescending(s => s.SignTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SignInStatsReadModel
                {
                    LogID = s.LogID,
                    SignTime = s.SignTime,
                    PointsChanged = s.PointsChanged,
                    ExpGained = s.ExpGained,
                    CouponGained = s.CouponGained,
                    PointsChangedTime = s.PointsChangedTime,
                    ExpGainedTime = s.ExpGainedTime,
                    CouponGainedTime = s.CouponGainedTime
                })
                .ToListAsync();

            // 取得簽到統計摘要
            var summary = await GetUserSignInSummary(userId);

            var viewModel = new UserSignInStatsViewModel
            {
                UserId = userId,
                UserName = user.User_name,
                UserAccount = user.User_Account,
                Summary = summary,
                SignInRecords = userSignInStats,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // GET: MiniGame/AdminSignInStats/Statistics
        [HttpGet("Statistics")]
        public async Task<IActionResult> Statistics()
        {
            ViewData["Title"] = "簽到統計分析";
            ViewData["Description"] = "查看整體簽到統計與趨勢分析";

            var stats = new SignInStatisticsViewModel();

            // 今日簽到統計
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            stats.TodaySignInCount = await _context.UserSignInStats
                .CountAsync(s => s.SignTime >= today && s.SignTime < tomorrow);

            // 本週簽到統計
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            stats.ThisWeekSignInCount = await _context.UserSignInStats
                .CountAsync(s => s.SignTime >= weekStart);

            // 本月簽到統計
            var monthStart = new DateTime(today.Year, today.Month, 1);
            stats.ThisMonthSignInCount = await _context.UserSignInStats
                .CountAsync(s => s.SignTime >= monthStart);

            // 總簽到次數
            stats.TotalSignInCount = await _context.UserSignInStats.CountAsync();

            // 活躍用戶數（本月至少簽到一次）
            stats.ActiveUsersThisMonth = await _context.UserSignInStats
                .Where(s => s.SignTime >= monthStart)
                .Select(s => s.UserID)
                .Distinct()
                .CountAsync();

            // 最近7天簽到趨勢
            stats.Last7DaysTrend = new List<DailySignInTrend>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var nextDate = date.AddDays(1);
                var count = await _context.UserSignInStats
                    .CountAsync(s => s.SignTime >= date && s.SignTime < nextDate);

                stats.Last7DaysTrend.Add(new DailySignInTrend
                {
                    Date = date,
                    SignInCount = count
                });
            }

            // 獎勵發放統計
            stats.TotalPointsAwarded = await _context.UserSignInStats
                .Where(s => s.PointsChanged > 0)
                .SumAsync(s => s.PointsChanged);

            stats.TotalExpAwarded = await _context.UserSignInStats
                .Where(s => s.ExpGained > 0)
                .SumAsync(s => s.ExpGained);

            stats.TotalCouponsAwarded = await _context.UserSignInStats
                .CountAsync(s => !string.IsNullOrEmpty(s.CouponGained) && s.CouponGained != "0");

            return View(stats);
        }

        #region Helper Methods

        private async Task<UserSignInSummary> GetUserSignInSummary(int userId)
        {
            var summary = new UserSignInSummary();

            // 總簽到次數
            summary.TotalSignInCount = await _context.UserSignInStats
                .CountAsync(s => s.UserID == userId);

            // 本月簽到次數
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            summary.ThisMonthSignInCount = await _context.UserSignInStats
                .CountAsync(s => s.UserID == userId && s.SignTime >= monthStart);

            // 最後簽到時間
            summary.LastSignInTime = await _context.UserSignInStats
                .Where(s => s.UserID == userId)
                .MaxAsync(s => (DateTime?)s.SignTime);

            // 累積獲得點數
            summary.TotalPointsEarned = await _context.UserSignInStats
                .Where(s => s.UserID == userId && s.PointsChanged > 0)
                .SumAsync(s => s.PointsChanged);

            // 累積獲得經驗
            summary.TotalExpEarned = await _context.UserSignInStats
                .Where(s => s.UserID == userId && s.ExpGained > 0)
                .SumAsync(s => s.ExpGained);

            // 獲得優惠券次數
            summary.CouponsEarned = await _context.UserSignInStats
                .CountAsync(s => s.UserID == userId && !string.IsNullOrEmpty(s.CouponGained) && s.CouponGained != "0");

            return summary;
        }

        #endregion
    }

    // Read Models
    public class SignInStatsReadModel
    {
        public int LogID { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public DateTime SignTime { get; set; }
        public int PointsChanged { get; set; }
        public int ExpGained { get; set; }
        public string CouponGained { get; set; }
        public DateTime PointsChangedTime { get; set; }
        public DateTime ExpGainedTime { get; set; }
        public DateTime CouponGainedTime { get; set; }
    }

    public class SignInStatsDetailReadModel : SignInStatsReadModel
    {
        public int UserID { get; set; }
        public UserSignInSummary UserSignInSummary { get; set; }
    }

    public class UserSignInSummary
    {
        public int TotalSignInCount { get; set; }
        public int ThisMonthSignInCount { get; set; }
        public DateTime? LastSignInTime { get; set; }
        public int TotalPointsEarned { get; set; }
        public int TotalExpEarned { get; set; }
        public int CouponsEarned { get; set; }
    }

    public class UserSignInStatsViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public UserSignInSummary Summary { get; set; }
        public List<SignInStatsReadModel> SignInRecords { get; set; } = new List<SignInStatsReadModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    public class SignInStatisticsViewModel
    {
        public int TodaySignInCount { get; set; }
        public int ThisWeekSignInCount { get; set; }
        public int ThisMonthSignInCount { get; set; }
        public int TotalSignInCount { get; set; }
        public int ActiveUsersThisMonth { get; set; }
        public int TotalPointsAwarded { get; set; }
        public int TotalExpAwarded { get; set; }
        public int TotalCouponsAwarded { get; set; }
        public List<DailySignInTrend> Last7DaysTrend { get; set; } = new List<DailySignInTrend>();
    }

    public class DailySignInTrend
    {
        public DateTime Date { get; set; }
        public int SignInCount { get; set; }
    }
}