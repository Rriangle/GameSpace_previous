using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 管理後台控制器
    /// </summary>
    public class AdminController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public AdminController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 管理後台首頁
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // 獲取統計數據
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalOrders = await _context.OrderInfos.CountAsync(),
                TotalCoupons = await _context.Coupons.CountAsync(),
                TotalEVouchers = await _context.EVouchers.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.LastLoginTime.HasValue && u.LastLoginTime > DateTime.Now.AddDays(-30)),
                PendingOrders = await _context.OrderInfos.CountAsync(o => o.OrderStatus == "Pending")
            };

            ViewBag.Stats = stats;
            return View();
        }

        /// <summary>
        /// 用戶管理頁面
        /// </summary>
        public async Task<IActionResult> Users(int page = 1, int pageSize = 20)
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Users.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;

            return View(users);
        }

        /// <summary>
        /// 訂單管理頁面
        /// </summary>
        public async Task<IActionResult> Orders(int page = 1, int pageSize = 20)
        {
            var orders = await _context.OrderInfos
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.OrderInfos.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;

            return View(orders);
        }

        /// <summary>
        /// 優惠券管理頁面
        /// </summary>
        public async Task<IActionResult> Coupons(int page = 1, int pageSize = 20)
        {
            var coupons = await _context.Coupons
                .Include(c => c.CouponType)
                .Include(c => c.User)
                .OrderByDescending(c => c.AcquiredTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Coupons.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;

            return View(coupons);
        }

        /// <summary>
        /// 電子禮券管理頁面
        /// </summary>
        public async Task<IActionResult> EVouchers(int page = 1, int pageSize = 20)
        {
            var eVouchers = await _context.EVouchers
                .Include(e => e.EVoucherType)
                .Include(e => e.User)
                .OrderByDescending(e => e.AcquiredTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.EVouchers.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;

            return View(eVouchers);
        }

        /// <summary>
        /// 系統監控頁面
        /// </summary>
        public async Task<IActionResult> Monitoring()
        {
            var monitoring = new
            {
                SystemUptime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime,
                MemoryUsage = GC.GetTotalMemory(false),
                ActiveConnections = _context.Database.GetDbConnection().State.ToString(),
                LastBackup = DateTime.Now.AddDays(-1), // 模擬數據
                ErrorLogs = new List<object>(), // 模擬數據
                PerformanceMetrics = new
                {
                    AvgResponseTime = "120ms",
                    RequestsPerSecond = 45,
                    ErrorRate = "0.1%"
                }
            };

            ViewBag.Monitoring = monitoring;
            return View();
        }

        /// <summary>
        /// 獲取用戶統計數據
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserStats()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt.Date == DateTime.Today),
                ActiveUsers = await _context.Users.CountAsync(u => u.LastLoginTime.HasValue && u.LastLoginTime > DateTime.Now.AddDays(-7)),
                BannedUsers = await _context.Users.CountAsync(u => u.IsBanned)
            };

            return Json(stats);
        }

        /// <summary>
        /// 獲取訂單統計數據
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrderStats()
        {
            var stats = new
            {
                TotalOrders = await _context.OrderInfos.CountAsync(),
                PendingOrders = await _context.OrderInfos.CountAsync(o => o.OrderStatus == "Pending"),
                CompletedOrders = await _context.OrderInfos.CountAsync(o => o.OrderStatus == "Completed"),
                CancelledOrders = await _context.OrderInfos.CountAsync(o => o.OrderStatus == "Cancelled"),
                TotalRevenue = await _context.OrderInfos
                    .Where(o => o.OrderStatus == "Completed")
                    .SumAsync(o => o.TotalAmount)
            };

            return Json(stats);
        }

        /// <summary>
        /// 獲取券類統計數據
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCouponStats()
        {
            var stats = new
            {
                TotalCoupons = await _context.Coupons.CountAsync(),
                UsedCoupons = await _context.Coupons.CountAsync(c => c.IsUsed),
                AvailableCoupons = await _context.Coupons.CountAsync(c => !c.IsUsed),
                TotalEVouchers = await _context.EVouchers.CountAsync(),
                UsedEVouchers = await _context.EVouchers.CountAsync(e => e.IsUsed),
                AvailableEVouchers = await _context.EVouchers.CountAsync(e => !e.IsUsed)
            };

            return Json(stats);
        }
    }
}