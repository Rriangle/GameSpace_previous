using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;

namespace GameSpace.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// �u�f�������޲z���
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
        /// �u�f�������C��
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var couponTypes = await _context.CouponTypes
                .OrderBy(ct => ct.TypeName)
                .ToListAsync();

            return View(couponTypes);
        }

        /// <summary>
        /// �u�f�������Ա�
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
        /// �Ы��u�f����������
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// �Ы��u�f������
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
                
                TempData["SuccessMessage"] = "優惠券類型建立成功！";
                return RedirectToAction(nameof(Index));
            }

            return View(couponType);
        }

        /// <summary>
        /// �s���u�f����������
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
        /// �s���u�f������
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
        /// �R���u�f����������
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
        /// �R���u�f������
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
