using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;
using GameSpace.Areas.social_hub.Services;

namespace GameSpace.Areas.social_hub.Controllers
{
	[Area("social_hub")]
	public class MutesController : Controller
	{
		// Fields
		private readonly GameSpace.Models.GameSpacedatabaseContext _context;
		private readonly GameSpace.Areas.social_hub.Services.IMuteFilter _muteFilter;

		// Constructor
		public MutesController(
			GameSpace.Models.GameSpacedatabaseContext context,
			GameSpace.Areas.social_hub.Services.IMuteFilter muteFilter)
		{
			_context = context;
			_muteFilter = muteFilter;
		}


		// GET: social_hub/Mutes
		public async Task<IActionResult> Index()
		{
			var list = await _context.Mutes
				.AsNoTracking()
				.OrderByDescending(m => m.MuteId)
				.ToListAsync();
			return View(list);
		}

		// GET: social_hub/Mutes/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var mute = await _context.Mutes
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.MuteId == id.Value);

			if (mute == null) return NotFound();

			return View(mute);
		}

		// GET: social_hub/Mutes/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: social_hub/Mutes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("MuteName,IsActive")] Mute mute)
		{
			if (!ModelState.IsValid) return View(mute);

			// If your Mute model has CreatedAt or ManagerId, add them here:
			// mute.CreatedAt = DateTime.UtcNow;
			// mute.ManagerId = currentManagerId;

			_context.Add(mute);
			await _context.SaveChangesAsync();

			// Refresh filter rules in cache after vocabulary update
			await _muteFilter.RefreshAsync();

			TempData["Msg"] = "Vocabulary added and filter rules refreshed.";
			return RedirectToAction(nameof(Index));
		}

		// GET: social_hub/Mutes/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var mute = await _context.Mutes.FindAsync(id.Value);
			if (mute == null) return NotFound();

			return View(mute);
		}

		// POST: social_hub/Mutes/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("MuteId,MuteName,IsActive")] Mute mute)
		{
			if (id != mute.MuteId) return NotFound();
			if (!ModelState.IsValid) return View(mute);

			try
			{
				_context.Update(mute);
				await _context.SaveChangesAsync();

				await _muteFilter.RefreshAsync();

				TempData["Msg"] = "Vocabulary updated and filter rules refreshed.";
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await _context.Mutes.AnyAsync(e => e.MuteId == id))
					return NotFound();
				throw;
			}
		}

		// GET: social_hub/Mutes/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var mute = await _context.Mutes
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.MuteId == id.Value);

			if (mute == null) return NotFound();

			return View(mute);
		}

		// POST: social_hub/Mutes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var mute = await _context.Mutes.FirstOrDefaultAsync(m => m.MuteId == id);
			if (mute != null)
			{
				_context.Mutes.Remove(mute);
				await _context.SaveChangesAsync();

				await _muteFilter.RefreshAsync();

				TempData["Msg"] = "Vocabulary deleted and filter rules refreshed.";
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
