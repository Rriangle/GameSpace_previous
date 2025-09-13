using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 電子禮券控制器
    /// </summary>
    public class EVoucherController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public EVoucherController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 電子禮券列表頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var eVouchers = await _context.EVouchers
                .Include(e => e.EVoucherType)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.AcquiredTime)
                .ToListAsync();

            return View(eVouchers);
        }

        /// <summary>
        /// 獲取用戶可用的電子禮券
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAvailableEVouchers(int userId)
        {
            var eVouchers = await _context.EVouchers
                .Include(e => e.EVoucherType)
                .Where(e => e.UserId == userId && !e.IsUsed && e.EVoucherType.IsActive)
                .Where(e => e.EVoucherType.ValidFrom <= DateTime.Now && e.EVoucherType.ValidTo >= DateTime.Now)
                .Where(e => e.ExpiryDate == null || e.ExpiryDate >= DateTime.Now)
                .Select(e => new
                {
                    e.EVoucherId,
                    e.EVoucherCode,
                    e.EVoucherType.TypeName,
                    e.EVoucherType.Value,
                    e.EVoucherType.Currency,
                    e.AcquiredTime,
                    e.ExpiryDate,
                    e.EVoucherType.ValidTo
                })
                .ToListAsync();

            return Json(eVouchers);
        }

        /// <summary>
        /// 使用電子禮券
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UseEVoucher(int eVoucherId, int orderId, decimal amount)
        {
            try
            {
                var eVoucher = await _context.EVouchers
                    .Include(e => e.EVoucherType)
                    .FirstOrDefaultAsync(e => e.EVoucherId == eVoucherId);

                if (eVoucher == null)
                {
                    return Json(new { success = false, message = "電子禮券不存在" });
                }

                if (eVoucher.IsUsed)
                {
                    return Json(new { success = false, message = "電子禮券已使用" });
                }

                if (eVoucher.EVoucherType.ValidTo < DateTime.Now)
                {
                    return Json(new { success = false, message = "電子禮券已過期" });
                }

                if (eVoucher.ExpiryDate.HasValue && eVoucher.ExpiryDate < DateTime.Now)
                {
                    return Json(new { success = false, message = "電子禮券已過期" });
                }

                if (amount > eVoucher.EVoucherType.Value)
                {
                    return Json(new { success = false, message = "使用金額超過禮券面額" });
                }

                // 更新電子禮券狀態
                eVoucher.IsUsed = true;
                eVoucher.UsedTime = DateTime.Now;
                eVoucher.UsedInOrderId = orderId;

                // 記錄兌換日誌
                var redeemLog = new EVoucherRedeemLog
                {
                    EVoucherId = eVoucherId,
                    UserId = eVoucher.UserId,
                    RedeemTime = DateTime.Now,
                    RedeemAmount = amount,
                    Description = $"訂單 {orderId} 使用電子禮券",
                    Status = "Success"
                };

                _context.EVoucherRedeemLogs.Add(redeemLog);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "電子禮券使用成功", redeemAmount = amount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"使用電子禮券失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 生成電子禮券
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GenerateEVoucher(int userId, int eVoucherTypeId)
        {
            try
            {
                var eVoucherType = await _context.EVoucherTypes
                    .FirstOrDefaultAsync(et => et.EVoucherTypeId == eVoucherTypeId);

                if (eVoucherType == null)
                {
                    return Json(new { success = false, message = "電子禮券類型不存在" });
                }

                if (!eVoucherType.IsActive)
                {
                    return Json(new { success = false, message = "電子禮券類型已停用" });
                }

                // 生成唯一電子禮券代碼
                var eVoucherCode = GenerateEVoucherCode();

                var eVoucher = new EVoucher
                {
                    EVoucherCode = eVoucherCode,
                    EVoucherTypeId = eVoucherTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(30) // 30天後過期
                };

                _context.EVouchers.Add(eVoucher);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "電子禮券生成成功", eVoucherId = eVoucher.EVoucherId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"生成電子禮券失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 生成電子禮券代碼
        /// </summary>
        private string GenerateEVoucherCode()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"EV{result}";
        }

        /// <summary>
        /// 獲取兌換記錄
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRedeemLogs(int userId)
        {
            var logs = await _context.EVoucherRedeemLogs
                .Include(l => l.EVoucher)
                .ThenInclude(e => e.EVoucherType)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.RedeemTime)
                .Select(l => new
                {
                    l.LogId,
                    l.EVoucher.EVoucherCode,
                    l.EVoucher.EVoucherType.TypeName,
                    l.RedeemAmount,
                    l.RedeemTime,
                    l.Status,
                    l.Description
                })
                .ToListAsync();

            return Json(logs);
        }
    }
}