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
    public class AdminWalletController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminWalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MiniGame/AdminWallet
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 20)
        {
            ViewData["Title"] = "會員錢包管理";
            ViewData["Description"] = "查看和管理所有會員的錢包餘額與交易記錄";

            var walletsQuery = _context.User_Wallet
                .Include(w => w.User)
                .AsNoTracking();

            // 搜尋功能
            if (!string.IsNullOrEmpty(searchTerm))
            {
                walletsQuery = walletsQuery.Where(w => 
                    w.User.User_name.Contains(searchTerm) ||
                    w.User.User_Account.Contains(searchTerm));
                ViewData["SearchTerm"] = searchTerm;
            }

            // 分頁
            var totalCount = await walletsQuery.CountAsync();
            var wallets = await walletsQuery
                .OrderByDescending(w => w.User_Point)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WalletReadModel
                {
                    UserId = w.User_Id,
                    UserName = w.User.User_name,
                    UserAccount = w.User.User_Account,
                    UserPoint = w.User_Point,
                    CouponCount = _context.Coupon.Count(c => c.UserID == w.User_Id && !c.IsUsed),
                    EVoucherCount = _context.EVoucher.Count(e => e.UserID == w.User_Id && !e.IsUsed)
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(wallets);
        }

        // GET: MiniGame/AdminWallet/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "錢包詳細資料";
            ViewData["Description"] = "查看會員錢包的詳細資訊與交易歷史";

            var wallet = await _context.User_Wallet
                .Include(w => w.User)
                .AsNoTracking()
                .Where(w => w.User_Id == id)
                .Select(w => new WalletDetailReadModel
                {
                    UserId = w.User_Id,
                    UserName = w.User.User_name,
                    UserAccount = w.User.User_Account,
                    UserPoint = w.User_Point
                })
                .FirstOrDefaultAsync();

            if (wallet == null)
            {
                return NotFound($"找不到用戶ID {id} 的錢包資料");
            }

            // 取得交易歷史（最近50筆）
            wallet.TransactionHistory = await _context.WalletHistory
                .AsNoTracking()
                .Where(h => h.UserID == id)
                .OrderByDescending(h => h.ChangeTime)
                .Take(50)
                .Select(h => new WalletHistoryReadModel
                {
                    ChangeType = h.ChangeType,
                    PointsChanged = h.PointsChanged,
                    ItemCode = h.ItemCode,
                    Description = h.Description,
                    ChangeTime = h.ChangeTime
                })
                .ToListAsync();

            // 取得優惠券
            wallet.Coupons = await _context.Coupon
                .Include(c => c.CouponType)
                .AsNoTracking()
                .Where(c => c.UserID == id)
                .OrderByDescending(c => c.AcquiredTime)
                .Take(20)
                .Select(c => new CouponReadModel
                {
                    CouponCode = c.CouponCode,
                    CouponTypeName = c.CouponType.Name,
                    IsUsed = c.IsUsed,
                    AcquiredTime = c.AcquiredTime,
                    UsedTime = c.UsedTime
                })
                .ToListAsync();

            // 取得電子禮券
            wallet.EVouchers = await _context.EVoucher
                .Include(e => e.EVoucherType)
                .AsNoTracking()
                .Where(e => e.UserID == id)
                .OrderByDescending(e => e.AcquiredTime)
                .Take(20)
                .Select(e => new EVoucherReadModel
                {
                    EVoucherCode = e.EVoucherCode,
                    EVoucherTypeName = e.EVoucherType.Name,
                    IsUsed = e.IsUsed,
                    AcquiredTime = e.AcquiredTime,
                    UsedTime = e.UsedTime
                })
                .ToListAsync();

            return View(wallet);
        }

        // GET: MiniGame/AdminWallet/History
        [HttpGet("History")]
        public async Task<IActionResult> History(string searchTerm = "", string changeType = "", int page = 1, int pageSize = 50)
        {
            ViewData["Title"] = "交易歷史記錄";
            ViewData["Description"] = "查看所有會員的錢包交易歷史";

            var historyQuery = _context.WalletHistory
                .Include(h => h.User)
                .AsNoTracking();

            // 篩選條件
            if (!string.IsNullOrEmpty(searchTerm))
            {
                historyQuery = historyQuery.Where(h => 
                    h.User.User_name.Contains(searchTerm) ||
                    h.User.User_Account.Contains(searchTerm) ||
                    h.Description.Contains(searchTerm));
                ViewData["SearchTerm"] = searchTerm;
            }

            if (!string.IsNullOrEmpty(changeType))
            {
                historyQuery = historyQuery.Where(h => h.ChangeType == changeType);
                ViewData["ChangeType"] = changeType;
            }

            // 分頁
            var totalCount = await historyQuery.CountAsync();
            var history = await historyQuery
                .OrderByDescending(h => h.ChangeTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new WalletHistoryReadModel
                {
                    UserName = h.User.User_name,
                    UserAccount = h.User.User_Account,
                    ChangeType = h.ChangeType,
                    PointsChanged = h.PointsChanged,
                    ItemCode = h.ItemCode,
                    Description = h.Description,
                    ChangeTime = h.ChangeTime
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            // 提供變更類型選項給篩選下拉選單
            ViewData["ChangeTypes"] = await _context.WalletHistory
                .Select(h => h.ChangeType)
                .Distinct()
                .OrderBy(ct => ct)
                .ToListAsync();

            return View(history);
        }
    }

    // Read Models for AsNoTracking queries
    public class WalletReadModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public int UserPoint { get; set; }
        public int CouponCount { get; set; }
        public int EVoucherCount { get; set; }
    }

    public class WalletDetailReadModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public int UserPoint { get; set; }
        public List<WalletHistoryReadModel> TransactionHistory { get; set; } = new List<WalletHistoryReadModel>();
        public List<CouponReadModel> Coupons { get; set; } = new List<CouponReadModel>();
        public List<EVoucherReadModel> EVouchers { get; set; } = new List<EVoucherReadModel>();
    }

    public class WalletHistoryReadModel
    {
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string ChangeType { get; set; }
        public int PointsChanged { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public DateTime ChangeTime { get; set; }
    }

    public class CouponReadModel
    {
        public string CouponCode { get; set; }
        public string CouponTypeName { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AcquiredTime { get; set; }
        public DateTime? UsedTime { get; set; }
    }

    public class EVoucherReadModel
    {
        public string EVoucherCode { get; set; }
        public string EVoucherTypeName { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AcquiredTime { get; set; }
        public DateTime? UsedTime { get; set; }
    }
}