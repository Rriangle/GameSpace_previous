using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSpace.Areas.social_hub.Services;
using GameSpace.Data;
using GameSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.social_hub.Controllers
{
	[Area("social_hub")]
	public class MessageCenterController : Controller
	{
		private readonly GameSpacedatabaseContext _context;
		private readonly INotificationService _notificationService;

		public MessageCenterController(GameSpacedatabaseContext context, INotificationService notificationService)
		{
			_context = context;
			_notificationService = notificationService;
		}

		// =========================
		// Utility: Get int from Cookie
		// =========================
		private int? TryGetCookieInt(string key)
		{
			if (!Request.Cookies.TryGetValue(key, out var val)) return null;
			return int.TryParse(val, out var n) ? n : (int?)null;
		}

		// =========================
		// Admin view all notifications (simple overview)
		// GET: social_hub/MessageCenter
		// =========================
		public async Task<IActionResult> Index()
		{
			// Management overview: List recent notifications (including source/action/sender/recipient count)
			var list = await _context.Notifications
				.AsNoTracking()
				.OrderByDescending(n => n.NotificationId)
				.Select(n => new NotificationAdminListItemVM
				{
					NotificationId = n.NotificationId,
					NotificationTitle = n.NotificationTitle,
					NotificationMessage = n.NotificationMessage,
					// ↓ Adjust according to your actual model names (e.g., Source.SourceName / Action.ActionName)
					SourceName = n.Source != null ? n.Source.SourceName : null,
					ActionName = n.Action != null ? n.Action.ActionName : null,
					SenderName =
						n.Sender != null
							? (n.Sender.UserName ?? n.Sender.UserAccount)       // If no UserName/UserAccount, change to your field
							: (n.SenderManager != null
								? n.SenderManager.ManagerName                   // If admin name field is different, adjust accordingly
								: "System"),
					CreatedAt = n.CreatedAt,                                   // If Nullable, add ?? DateTime.UtcNow
					RecipientCount = _context.NotificationRecipients.Count(r => r.NotificationId == n.NotificationId)
				})
				.Take(200)
				.ToListAsync();

			return View(list);
		}

		// =========================
		// User inbox (based on Cookie sh_uid)
		// GET: social_hub/MessageCenter/Inbox
		// =========================
		public async Task<IActionResult> Inbox()
		{
			var uid = TryGetCookieInt("sh_uid");
			if (uid is null || uid.Value <= 0)
				return Unauthorized(); // Or return empty list View, depending on your needs

			var list = await _context.NotificationRecipients
				.AsNoTracking()
				.Where(nr => nr.UserId == uid.Value)
				.OrderByDescending(nr => nr.RecipientId)
				.Select(nr => new NotificationInboxItemVM
				{
					RecipientId = nr.RecipientId,
					NotificationId = nr.NotificationId,
					NotificationTitle = nr.Notification.NotificationTitle,
					NotificationMessage = nr.Notification.NotificationMessage,
					SourceName = nr.Notification.Source != null ? nr.Notification.Source.SourceName : null,
					ActionName = nr.Notification.Action != null ? nr.Notification.Action.ActionName : null,
					SenderName =
						nr.Notification.Sender != null
							? (nr.Notification.Sender.UserName ?? nr.Notification.Sender.UserAccount)
							: (nr.Notification.SenderManager != null
								? nr.Notification.SenderManager.ManagerName
								: "System"),
					CreatedAt = nr.Notification.CreatedAt,
					IsRead = nr.IsRead
				})
				.Take(200)
				.ToListAsync();

			return View(list);
		}

		// =========================
		// Mark single recipient detail as read
		// POST: social_hub/MessageCenter/MarkRead/5
		// =========================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkRead(int id)
		{
			var rec = await _context.NotificationRecipients.FirstOrDefaultAsync(r => r.RecipientId == id);
			if (rec == null) return NotFound();

			if (!rec.IsRead)
			{
				rec.IsRead = true;
				// If your model has ReadAt field, you can add:
				// rec.ReadAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Inbox));
		}

		// =========================
		// Create notification (Admin)
		// GET: social_hub/MessageCenter/Create
		// =========================
		public IActionResult Create()
		{
			// TODO: If you need source/action dropdown options, populate ViewData with options
			return View();
		}

		// =========================
		// Create notification (Admin)
		// POST: social_hub/MessageCenter/Create
		// =========================
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(
			[Bind("NotificationTitle,NotificationMessage,SourceId,ActionId")] Notification notification,
			List<int> recipientIds)
		{
			if (!ModelState.IsValid)
			{
				// TODO: If dropdown needs to be repopulated, put ViewData here and return
				return View(notification);
			}

			int? senderUserId = TryGetCookieInt("sh_uid");
			int? senderManagerId = TryGetCookieInt("sh_mid");

			// Use service to handle FK decisions, recipient deduplication and validation
			var added = await _notificationService.CreateAsync(
				notification,
				recipientIds ?? Enumerable.Empty<int>(),
				senderUserId,
				senderManagerId
			);

			TempData["Msg"] = $"✅ Notification #{notification.NotificationId} created successfully, sent to {added} recipients.";
			return RedirectToAction(nameof(Index));
		}

		// =========================
		// (Optional) Delete notification main record (if you want to preserve history, it's recommended not to provide this)
		// =========================
		public async Task<IActionResult> Delete(int id)
		{
			var n = await _context.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.NotificationId == id);
			if (n == null) return NotFound();
			return View(n);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var n = await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
			if (n != null)
			{
				// Delete recipient details first, then main record (avoid FK)
				var recs = _context.NotificationRecipients.Where(r => r.NotificationId == id);
				_context.NotificationRecipients.RemoveRange(recs);
				_context.Notifications.Remove(n);
				await _context.SaveChangesAsync();
				TempData["Msg"] = "Notification and its recipient details deleted.";
			}
			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Internal VM (avoid dependency on external ViewModel)
		// =========================
		public class NotificationAdminListItemVM
		{
			public int NotificationId { get; set; }
			public string? NotificationTitle { get; set; }
			public string? NotificationMessage { get; set; }
			public string? SourceName { get; set; }
			public string? ActionName { get; set; }
			public string? SenderName { get; set; }
			public DateTime CreatedAt { get; set; }
			public int RecipientCount { get; set; }
		}

		public class NotificationInboxItemVM
		{
			public int RecipientId { get; set; }
			public int NotificationId { get; set; }
			public string? NotificationTitle { get; set; }
			public string? NotificationMessage { get; set; }
			public string? SourceName { get; set; }
			public string? ActionName { get; set; }
			public string? SenderName { get; set; }
			public DateTime CreatedAt { get; set; }
			public bool IsRead { get; set; }
		}
	}
}
