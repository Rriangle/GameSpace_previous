using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;

namespace GameSpace.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// §�������޲z���
    /// </summary>
    [Area("Admin")]
    public class EVoucherTypeController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public EVoucherTypeController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// §�������C��
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var evoucherTypes = await _context.EVoucherTypes
                .OrderBy(et => et.TypeName)
                .ToListAsync();

            return View(evoucherTypes);
        }

        /// <summary>
        /// §�������Ա�
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var evoucherType = await _context.EVoucherTypes.FindAsync(id);
            if (evoucherType == null)
            {
                return NotFound();
            }

            return View(evoucherType);
        }

        /// <summary>
        /// �Ы�§����������
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// �Ы�§������
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EVoucherType evoucherType)
        {
            if (ModelState.IsValid)
            {
                evoucherType.CreatedAt = DateTime.UtcNow;
                evoucherType.UpdatedAt = DateTime.UtcNow;
                _context.Add(evoucherType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(evoucherType);
        }

        /// <summary>
        /// �s��§����������
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var evoucherType = await _context.EVoucherTypes.FindAsync(id);
            if (evoucherType == null)
            {
                return NotFound();
            }
            return View(evoucherType);
        }

        /// <summary>
        /// �s��§������
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EVoucherType evoucherType)
        {
            if (id != evoucherType.TypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    evoucherType.UpdatedAt = DateTime.UtcNow;
                    _context.Update(evoucherType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EVoucherTypeExists(evoucherType.TypeID))
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
            return View(evoucherType);
        }

        /// <summary>
        /// �R��§����������
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var evoucherType = await _context.EVoucherTypes.FindAsync(id);
            if (evoucherType == null)
            {
                return NotFound();
            }

            return View(evoucherType);
        }

        /// <summary>
        /// �R��§������
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evoucherType = await _context.EVoucherTypes.FindAsync(id);
            if (evoucherType != null)
            {
                _context.EVoucherTypes.Remove(evoucherType);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EVoucherTypeExists(int id)
        {
            return _context.EVoucherTypes.Any(e => e.TypeID == id);
        }
    }
}
