using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSpace.Areas.social_hub.Services;
using GameSpace.Data;
using GameSpace.Filters;
using GameSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.social_hub.Controllers
{
	[Area("social_hub")]
	public class NotificationsController : Controller
	{
		private readonly GameSpacedatabaseContext _context;
		private readonly INotificationService _notificationService;

		public NotificationsController(GameSpacedatabaseContext context, INotificationService notificationService)
		{
			_context = context;
			_notificationService = notificationService;
		}

		// Get current integer from Cookie (sh_uid / sh_mid)
		private int? TryGetCookieInt(string key)
		{
			if (!Request.Cookies.TryGetValue(key, out var val)) return null;
			return int.TryParse(val, out var n) ? n : (int?)null;
		}

		private async Task<bool> IsCurrentUserAdminAsync()
		{
			var uid = TryGetCookieInt("sh_uid");
			if (uid is null || uid <= 0) return false;

			// Adjust according to your actual admin/role model (following your original query method)
			return await _context.ManagerData
				.AsNoTracking()
				.Where(m => m.ManagerId == uid)
				.SelectMany(m => m.ManagerRoles)
				.AnyAsync(rp => rp.ManagerRoleId == 1 || rp.ManagerRoleId == 2 || rp.ManagerRoleId == 8);
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.IsAdmin = await IsCurrentUserAdminAsync();

			// Simply list main records; if you need to show recipients/read status, query NotificationRecipients instead
			var list = await _context.Notifications
				.AsNoTracking()
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();

			return View(list);
		}

		[AdminOnly]
		public IActionResult Create() => View();

		/// <summary>
		/// Create a single notification and specify multiple recipients (recipients are in NotificationRecipients, not in Notification entity)
		/// </summary>
		[AdminOnly]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(
			[Bind("NotificationTitle,NotificationMessage,SourceId,ActionId")] Notification input,
			List<int> recipientIds)
		{
			if (!ModelState.IsValid) return View(input);

			var senderUserId = TryGetCookieInt("sh_uid");
			var senderManagerId = TryGetCookieInt("sh_mid");

			// Use service to handle uniformly: Sender field determination, recipient deduplication and validity filtering
			var added = await _notificationService.CreateAsync(
				input,
				recipientIds ?? Enumerable.Empty<int>(),
				senderUserId,
				senderManagerId
			);

			TempData["Toast"] = $"✅ Notification #{input.NotificationId} created successfully, sent to {added} recipients.";
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// Example: Broadcast by admin role (Note: If ManagerId is not Users.UserId, it will be filtered out by service layer)
		/// You can map Manager to corresponding UserId based on actual table relationships before passing in.
		/// </summary>
		[AdminOnly]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BroadcastToRole(
			int roleId,
			[Bind("NotificationTitle,NotificationMessage,SourceId,ActionId")] Notification template)
		{
			var senderUserId = TryGetCookieInt("sh_uid");
			var senderManagerId = TryGetCookieInt("sh_mid");

			// Currently using ManagerId as recipient list; if these IDs are not in Users, they will be filtered out by service layer (won't cause FK error)
			var receivers = await _context.ManagerData
				.AsNoTracking()
				.Where(m => m.ManagerRoles.Any(rp => rp.ManagerRoleId == roleId))
				.Select(m => m.ManagerId)
				.ToListAsync();

			var added = await _notificationService.CreateAsync(
				template,
				receivers,
				senderUserId,
				senderManagerId
			);

			TempData["Toast"] = $"📣 Broadcast completed (valid recipients: {added}).";
			return RedirectToAction(nameof(Index));
		}
	}
}
