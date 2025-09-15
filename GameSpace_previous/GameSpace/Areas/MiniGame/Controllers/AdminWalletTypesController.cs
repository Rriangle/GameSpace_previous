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
    public class AdminWalletTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminWalletTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region CouponTypes CRUD

        // GET: MiniGame/AdminWalletTypes/CouponTypes
        [HttpGet("CouponTypes")]
        public async Task<IActionResult> CouponTypes(int page = 1, int pageSize = 20)
        {
            ViewData["Title"] = "優惠券類型管理";
            ViewData["Description"] = "管理系統中的優惠券類型定義";

            var totalCount = await _context.CouponType.CountAsync();
            var couponTypes = await _context.CouponType
                .AsNoTracking()
                .OrderBy(ct => ct.CouponTypeID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ct => new CouponTypeReadModel
                {
                    CouponTypeID = ct.CouponTypeID,
                    Name = ct.Name,
                    DiscountType = ct.DiscountType,
                    DiscountValue = ct.DiscountValue,
                    MinSpend = ct.MinSpend,
                    ValidFrom = ct.ValidFrom,
                    ValidTo = ct.ValidTo,
                    PointsCost = ct.PointsCost,
                    Description = ct.Description,
                    UsageCount = _context.Coupon.Count(c => c.CouponTypeID == ct.CouponTypeID)
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(couponTypes);
        }

        // GET: MiniGame/AdminWalletTypes/CreateCouponType
        [HttpGet("CreateCouponType")]
        public IActionResult CreateCouponType()
        {
            ViewData["Title"] = "新增優惠券類型";
            ViewData["Description"] = "建立新的優惠券類型定義";

            return View(new CouponType());
        }

        // POST: MiniGame/AdminWalletTypes/CreateCouponType
        [HttpPost("CreateCouponType")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCouponType(CouponType couponType)
        {
            ViewData["Title"] = "新增優惠券類型";
            ViewData["Description"] = "建立新的優惠券類型定義";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.CouponType.Add(couponType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"優惠券類型 '{couponType.Name}' 已成功建立";
                    return RedirectToAction(nameof(CouponTypes));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"建立失敗：{ex.Message}");
                }
            }

            return View(couponType);
        }

        // GET: MiniGame/AdminWalletTypes/EditCouponType/5
        [HttpGet("EditCouponType/{id}")]
        public async Task<IActionResult> EditCouponType(int id)
        {
            ViewData["Title"] = "編輯優惠券類型";
            ViewData["Description"] = "修改優惠券類型的設定";

            var couponType = await _context.CouponType.FindAsync(id);
            if (couponType == null)
            {
                return NotFound($"找不到ID {id} 的優惠券類型");
            }

            return View(couponType);
        }

        // POST: MiniGame/AdminWalletTypes/EditCouponType/5
        [HttpPost("EditCouponType/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCouponType(int id, CouponType couponType)
        {
            ViewData["Title"] = "編輯優惠券類型";
            ViewData["Description"] = "修改優惠券類型的設定";

            if (id != couponType.CouponTypeID)
            {
                return BadRequest("ID 不匹配");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(couponType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"優惠券類型 '{couponType.Name}' 已成功更新";
                    return RedirectToAction(nameof(CouponTypes));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CouponTypeExists(couponType.CouponTypeID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"更新失敗：{ex.Message}");
                }
            }

            return View(couponType);
        }

        // GET: MiniGame/AdminWalletTypes/DeleteCouponType/5
        [HttpGet("DeleteCouponType/{id}")]
        public async Task<IActionResult> DeleteCouponType(int id)
        {
            ViewData["Title"] = "刪除優惠券類型";
            ViewData["Description"] = "確認刪除優惠券類型";

            var couponType = await _context.CouponType
                .AsNoTracking()
                .Where(ct => ct.CouponTypeID == id)
                .Select(ct => new CouponTypeReadModel
                {
                    CouponTypeID = ct.CouponTypeID,
                    Name = ct.Name,
                    DiscountType = ct.DiscountType,
                    DiscountValue = ct.DiscountValue,
                    MinSpend = ct.MinSpend,
                    ValidFrom = ct.ValidFrom,
                    ValidTo = ct.ValidTo,
                    PointsCost = ct.PointsCost,
                    Description = ct.Description,
                    UsageCount = _context.Coupon.Count(c => c.CouponTypeID == ct.CouponTypeID)
                })
                .FirstOrDefaultAsync();

            if (couponType == null)
            {
                return NotFound($"找不到ID {id} 的優惠券類型");
            }

            return View(couponType);
        }

        // POST: MiniGame/AdminWalletTypes/DeleteCouponType/5
        [HttpPost("DeleteCouponType/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCouponTypeConfirmed(int id)
        {
            try
            {
                var couponType = await _context.CouponType.FindAsync(id);
                if (couponType != null)
                {
                    // 檢查是否有相關的優惠券
                    var hasRelatedCoupons = await _context.Coupon.AnyAsync(c => c.CouponTypeID == id);
                    if (hasRelatedCoupons)
                    {
                        TempData["ErrorMessage"] = "無法刪除此優惠券類型，因為已有相關的優惠券使用此類型";
                        return RedirectToAction(nameof(CouponTypes));
                    }

                    _context.CouponType.Remove(couponType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"優惠券類型 '{couponType.Name}' 已成功刪除";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"刪除失敗：{ex.Message}";
            }

            return RedirectToAction(nameof(CouponTypes));
        }

        #endregion

        #region EVoucherTypes CRUD

        // GET: MiniGame/AdminWalletTypes/EVoucherTypes
        [HttpGet("EVoucherTypes")]
        public async Task<IActionResult> EVoucherTypes(int page = 1, int pageSize = 20)
        {
            ViewData["Title"] = "電子禮券類型管理";
            ViewData["Description"] = "管理系統中的電子禮券類型定義";

            var totalCount = await _context.EVoucherType.CountAsync();
            var evoucherTypes = await _context.EVoucherType
                .AsNoTracking()
                .OrderBy(evt => evt.EVoucherTypeID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(evt => new EVoucherTypeReadModel
                {
                    EVoucherTypeID = evt.EVoucherTypeID,
                    Name = evt.Name,
                    ValueAmount = evt.ValueAmount,
                    ValidFrom = evt.ValidFrom,
                    ValidTo = evt.ValidTo,
                    PointsCost = evt.PointsCost,
                    TotalAvailable = evt.TotalAvailable,
                    Description = evt.Description,
                    UsageCount = _context.EVoucher.Count(e => e.EVoucherTypeID == evt.EVoucherTypeID)
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(evoucherTypes);
        }

        // GET: MiniGame/AdminWalletTypes/CreateEVoucherType
        [HttpGet("CreateEVoucherType")]
        public IActionResult CreateEVoucherType()
        {
            ViewData["Title"] = "新增電子禮券類型";
            ViewData["Description"] = "建立新的電子禮券類型定義";

            return View(new EVoucherType());
        }

        // POST: MiniGame/AdminWalletTypes/CreateEVoucherType
        [HttpPost("CreateEVoucherType")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEVoucherType(EVoucherType evoucherType)
        {
            ViewData["Title"] = "新增電子禮券類型";
            ViewData["Description"] = "建立新的電子禮券類型定義";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.EVoucherType.Add(evoucherType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"電子禮券類型 '{evoucherType.Name}' 已成功建立";
                    return RedirectToAction(nameof(EVoucherTypes));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"建立失敗：{ex.Message}");
                }
            }

            return View(evoucherType);
        }

        // GET: MiniGame/AdminWalletTypes/EditEVoucherType/5
        [HttpGet("EditEVoucherType/{id}")]
        public async Task<IActionResult> EditEVoucherType(int id)
        {
            ViewData["Title"] = "編輯電子禮券類型";
            ViewData["Description"] = "修改電子禮券類型的設定";

            var evoucherType = await _context.EVoucherType.FindAsync(id);
            if (evoucherType == null)
            {
                return NotFound($"找不到ID {id} 的電子禮券類型");
            }

            return View(evoucherType);
        }

        // POST: MiniGame/AdminWalletTypes/EditEVoucherType/5
        [HttpPost("EditEVoucherType/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEVoucherType(int id, EVoucherType evoucherType)
        {
            ViewData["Title"] = "編輯電子禮券類型";
            ViewData["Description"] = "修改電子禮券類型的設定";

            if (id != evoucherType.EVoucherTypeID)
            {
                return BadRequest("ID 不匹配");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evoucherType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"電子禮券類型 '{evoucherType.Name}' 已成功更新";
                    return RedirectToAction(nameof(EVoucherTypes));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EVoucherTypeExists(evoucherType.EVoucherTypeID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"更新失敗：{ex.Message}");
                }
            }

            return View(evoucherType);
        }

        #endregion

        #region Helper Methods

        private async Task<bool> CouponTypeExists(int id)
        {
            return await _context.CouponType.AnyAsync(e => e.CouponTypeID == id);
        }

        private async Task<bool> EVoucherTypeExists(int id)
        {
            return await _context.EVoucherType.AnyAsync(e => e.EVoucherTypeID == id);
        }

        #endregion
    }

    // Read Models
    public class CouponTypeReadModel
    {
        public int CouponTypeID { get; set; }
        public string Name { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinSpend { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int PointsCost { get; set; }
        public string Description { get; set; }
        public int UsageCount { get; set; }
    }

    public class EVoucherTypeReadModel
    {
        public int EVoucherTypeID { get; set; }
        public string Name { get; set; }
        public decimal ValueAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int PointsCost { get; set; }
        public int TotalAvailable { get; set; }
        public string Description { get; set; }
        public int UsageCount { get; set; }
    }
}