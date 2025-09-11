using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;

namespace GameSpace.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// �Τ�޲z���
    /// </summary>
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public UserController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// �Τ�C��
        /// </summary>
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var users = await _context.Users
                .OrderBy(u => u.UserID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Users.CountAsync();
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(users);
        }

        /// <summary>
        /// �Τ�Ա�
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // ����Τ������T
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserID == id);
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserID == id);
            var signInStats = await _context.UserSignInStats
                .Where(s => s.UserID == id)
                .OrderByDescending(s => s.SignInDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Wallet = wallet;
            ViewBag.Pet = pet;
            ViewBag.SignInStats = signInStats;

            return View(user);
        }

        /// <summary>
        /// �s��Τ᭶��
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        /// <summary>
        /// �s��Τ�
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
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
            return View(user);
        }

        /// <summary>
        /// �R���Τ᭶��
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// �R���Τ�
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
