using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 通知控制器
    /// </summary>
    public class NotificationController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public NotificationController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 通知中心頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var notifications = await _context.NotificationRecipients
                .Include(nr => nr.Notification)
                .ThenInclude(n => n.NotificationSource)
                .Include(nr => nr.Notification)
                .ThenInclude(n => n.NotificationAction)
                .Where(nr => nr.UserId == userId)
                .OrderByDescending(nr => nr.Notification.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        /// <summary>
        /// 獲取用戶未讀通知數量
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var unreadCount = await _context.NotificationRecipients
                .Where(nr => nr.UserId == userId && !nr.IsRead)
                .CountAsync();

            return Json(new { unreadCount });
        }

        /// <summary>
        /// 標記通知為已讀
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId, int userId)
        {
            try
            {
                var recipient = await _context.NotificationRecipients
                    .FirstOrDefaultAsync(nr => nr.NotificationId == notificationId && nr.UserId == userId);

                if (recipient == null)
                {
                    return Json(new { success = false, message = "通知不存在" });
                }

                recipient.IsRead = true;
                recipient.ReadAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "通知已標記為已讀" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"標記失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 標記所有通知為已讀
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            try
            {
                var recipients = await _context.NotificationRecipients
                    .Where(nr => nr.UserId == userId && !nr.IsRead)
                    .ToListAsync();

                foreach (var recipient in recipients)
                {
                    recipient.IsRead = true;
                    recipient.ReadAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "所有通知已標記為已讀" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"標記失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 刪除通知
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(int notificationId, int userId)
        {
            try
            {
                var recipient = await _context.NotificationRecipients
                    .FirstOrDefaultAsync(nr => nr.NotificationId == notificationId && nr.UserId == userId);

                if (recipient == null)
                {
                    return Json(new { success = false, message = "通知不存在" });
                }

                _context.NotificationRecipients.Remove(recipient);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "通知已刪除" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"刪除失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 發送通知
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendNotification(SendNotificationRequest request)
        {
            try
            {
                var notification = new Notification
                {
                    SourceId = request.SourceId,
                    ActionId = request.ActionId,
                    Title = request.Title,
                    Content = request.Content,
                    Priority = request.Priority,
                    Type = request.Type,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = request.ExpiresAt
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // 為每個接收者創建通知記錄
                foreach (var userId in request.UserIds)
                {
                    var recipient = new NotificationRecipient
                    {
                        NotificationId = notification.NotificationId,
                        UserId = userId,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    _context.NotificationRecipients.Add(recipient);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "通知發送成功", notificationId = notification.NotificationId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"發送失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 獲取通知統計
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotificationStats(int userId)
        {
            var stats = new
            {
                TotalNotifications = await _context.NotificationRecipients
                    .Where(nr => nr.UserId == userId)
                    .CountAsync(),
                UnreadNotifications = await _context.NotificationRecipients
                    .Where(nr => nr.UserId == userId && !nr.IsRead)
                    .CountAsync(),
                TodayNotifications = await _context.NotificationRecipients
                    .Where(nr => nr.UserId == userId && nr.Notification.CreatedAt.Date == DateTime.Today)
                    .CountAsync(),
                HighPriorityNotifications = await _context.NotificationRecipients
                    .Where(nr => nr.UserId == userId && !nr.IsRead && nr.Notification.Priority == "High")
                    .CountAsync()
            };

            return Json(stats);
        }
    }

    /// <summary>
    /// 發送通知請求模型
    /// </summary>
    public class SendNotificationRequest
    {
        public int SourceId { get; set; }
        public int ActionId { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
        public string Priority { get; set; } = "Normal";
        public string Type { get; set; } = "Info";
        public DateTime? ExpiresAt { get; set; }
    }
}