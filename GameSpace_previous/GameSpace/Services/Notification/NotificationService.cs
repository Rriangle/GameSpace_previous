using GameSpace.Data;
using GameSpace.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Services.Notification
{
    /// <summary>
    /// 通知服務實現
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(GameSpaceDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<NotificationReadModel>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var notifications = await _context.Set<NotificationReadModel>()
                    .Where(n => n.UserID == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("獲取用戶通知，用戶ID: {UserId}, 頁面: {Page}, 大小: {PageSize}, 結果: {Count} 筆", 
                    userId, page, pageSize, notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶通知時發生錯誤，用戶ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<NotificationReadModel?> GetNotificationByIdAsync(long notificationId)
        {
            try
            {
                var notification = await _context.Set<NotificationReadModel>()
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId);

                _logger.LogInformation("獲取通知詳情，通知ID: {NotificationId}, 結果: {Found}", 
                    notificationId, notification != null);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知詳情時發生錯誤，通知ID: {NotificationId}", notificationId);
                throw;
            }
        }

        public async Task<bool> MarkAsReadAsync(long notificationId)
        {
            try
            {
                var notification = await _context.Set<NotificationReadModel>()
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId);

                if (notification == null)
                {
                    return false;
                }

                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("標記通知為已讀，通知ID: {NotificationId}", notificationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記通知為已讀時發生錯誤，通知ID: {NotificationId}", notificationId);
                throw;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var notifications = await _context.Set<NotificationReadModel>()
                    .Where(n => n.UserID == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("標記所有通知為已讀，用戶ID: {UserId}, 數量: {Count}", 
                    userId, notifications.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記所有通知為已讀時發生錯誤，用戶ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                var count = await _context.Set<NotificationReadModel>()
                    .CountAsync(n => n.UserID == userId && !n.IsRead);

                _logger.LogInformation("獲取未讀通知數量，用戶ID: {UserId}, 數量: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取未讀通知數量時發生錯誤，用戶ID: {UserId}", userId);
                throw;
            }
        }

        public async Task SendNotificationAsync(int userId, string title, string message, string type = "info")
        {
            try
            {
                var notification = new NotificationReadModel
                {
                    UserID = userId,
                    Title = title,
                    Content = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<NotificationReadModel>().Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("發送通知成功，用戶ID: {UserId}, 標題: {Title}", userId, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送通知時發生錯誤，用戶ID: {UserId}", userId);
                throw;
            }
        }

        public async Task SendSystemNotificationAsync(string title, string message)
        {
            try
            {
                // 這裡可以實現系統通知邏輯，例如發送給所有用戶
                _logger.LogInformation("發送系統通知，標題: {Title}", title);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送系統通知時發生錯誤，標題: {Title}", title);
                throw;
            }
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _context.Set<NotificationReadModel>()
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.UserID == userId);

                if (notification != null)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("標記通知為已讀，通知ID: {NotificationId}, 用戶ID: {UserId}", notificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記通知為已讀時發生錯誤，通知ID: {NotificationId}, 用戶ID: {UserId}", notificationId, userId);
                throw;
            }
        }

        public async Task<bool> NeedsSeedingAsync()
        {
            try
            {
                var userCount = await _context.Set<UserReadModel>().CountAsync();
                return userCount == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查是否需要種子數據時發生錯誤");
                return false;
            }
        }
    }
}
