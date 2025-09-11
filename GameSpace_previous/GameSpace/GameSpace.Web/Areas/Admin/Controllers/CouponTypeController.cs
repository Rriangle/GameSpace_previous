using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;

namespace GameSpace.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 優惠券類型管理控制器
    /// </summary>
    [Area("Admin")]
    public class CouponTypeController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public CouponTypeController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 優惠券類型列表
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var couponTypes = await _context.CouponTypes
                .OrderBy(ct => ct.TypeName)
                .ToListAsync();

            return View(couponTypes);
        }

        /// <summary>
        /// 優惠券類型詳情
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);
        }

        /// <summary>
        /// 創建優惠券類型頁面
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 創建優惠券類型
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponType couponType)
        {
            if (ModelState.IsValid)
            {
                couponType.CreatedAt = DateTime.Now;
                couponType.UpdatedAt = DateTime.Now;
                
                _context.CouponTypes.Add(couponType);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "優惠券類型創建成功！";
                return RedirectToAction(nameof(Index));
            }

            return View(couponType);
        }

        /// <summary>
        /// 編輯優惠券類型頁面
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);
        }

        /// <summary>
        /// 編輯優惠券類型
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponType couponType)
        {
            if (id != couponType.TypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    couponType.UpdatedAt = DateTime.Now;
                    _context.Update(couponType);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "優惠券類型更新成功！";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponTypeExists(couponType.TypeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(couponType);
        }

        /// <summary>
        /// 刪除優惠券類型頁面
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType == null)
            {
                return NotFound();
            }

            return View(couponType);
        }

        /// <summary>
        /// 刪除優惠券類型
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var couponType = await _context.CouponTypes.FindAsync(id);
            if (couponType != null)
            {
                _context.CouponTypes.Remove(couponType);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "優惠券類型刪除成功！";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CouponTypeExists(int id)
        {
            return _context.CouponTypes.Any(e => e.TypeID == id);
        }
    }
}
