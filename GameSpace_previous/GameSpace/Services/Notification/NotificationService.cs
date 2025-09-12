using GameSpace.Data;
using GameSpace.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Services.Notification
{
    /// <summary>
    /// Notification service implementation
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

                _logger.LogInformation("Retrieved user notifications, UserID: {UserId}, Page: {Page}, Size: {PageSize}, Result: {Count} items", 
                    userId, page, pageSize, notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user notifications, UserID: {UserId}", userId);
                throw;
            }
        }

        public async Task<NotificationReadModel?> GetNotificationByIdAsync(long notificationId)
        {
            try
            {
                var notification = await _context.Set<NotificationReadModel>()
                    .FirstOrDefaultAsync(n => n.NotificationID == notificationId);

                _logger.LogInformation("Retrieved notification details, NotificationID: {NotificationId}, Result: {Found}", 
                    notificationId, notification != null);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving notification details, NotificationID: {NotificationId}", notificationId);
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

                _logger.LogInformation("Marked notification as read, NotificationID: {NotificationId}", notificationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking notification as read, NotificationID: {NotificationId}", notificationId);
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

                _logger.LogInformation("Marked all notifications as read, UserID: {UserId}, Count: {Count}", 
                    userId, notifications.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking all notifications as read, UserID: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                var count = await _context.Set<NotificationReadModel>()
                    .CountAsync(n => n.UserID == userId && !n.IsRead);

                _logger.LogInformation("Retrieved unread notification count, UserID: {UserId}, Count: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unread notification count, UserID: {UserId}", userId);
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

                _logger.LogInformation("Notification sent successfully, UserID: {UserId}, Title: {Title}", userId, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending notification, UserID: {UserId}", userId);
                throw;
            }
        }

        public async Task SendSystemNotificationAsync(string title, string message)
        {
            try
            {
                // Here you can implement system notification logic, such as sending to all users
                _logger.LogInformation("Sending system notification, Title: {Title}", title);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending system notification, Title: {Title}", title);
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

                _logger.LogInformation("Marked notification as read, NotificationID: {NotificationId}, UserID: {UserId}", notificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking notification as read, NotificationID: {NotificationId}, UserID: {UserId}", notificationId, userId);
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
                _logger.LogError(ex, "Error occurred while checking if seeding is needed");
                return false;
            }
        }
    }
}
