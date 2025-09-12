using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameSpace.Services.Admin
{
    public interface IAdminService
    {
        // 管理員管理
        Task<AdminResult> CreateAdminAsync(string managerName, string managerAccount, string managerPassword, string managerEmail);
        Task<AdminResult> GetAdminAsync(int managerId);
        Task<List<ManagerDatum>> GetAllAdminsAsync(int page = 1, int pageSize = 20);
        Task<AdminResult> UpdateAdminAsync(int managerId, string managerName, string managerEmail);
        Task<AdminResult> DeleteAdminAsync(int managerId);
        Task<AdminResult> ChangePasswordAsync(int managerId, string oldPassword, string newPassword);
        
        // 用戶管理
        Task<UserResult> GetUserAsync(int userId);
        Task<List<User>> GetAllUsersAsync(int page = 1, int pageSize = 20);
        Task<UserResult> UpdateUserStatusAsync(int userId, bool isActive);
        Task<UserResult> LockUserAsync(int userId, DateTime? lockoutEnd);
        Task<UserResult> UnlockUserAsync(int userId);
        Task<UserResult> ResetUserPasswordAsync(int userId, string newPassword);
        
        // 系統統計
        Task<SystemStatsResult> GetSystemStatsAsync();
        Task<List<User>> GetNewUsersAsync(int days = 7);
        Task<List<OrderInfo>> GetRecentOrdersAsync(int days = 7);
        Task<List<Post>> GetRecentPostsAsync(int days = 7);
        
        // 內容管理
        Task<ContentResult> DeletePostAsync(int postId);
        Task<ContentResult> DeleteThreadAsync(int threadId);
        Task<ContentResult> DeleteGroupAsync(int groupId);
        Task<ContentResult> BanUserAsync(int userId, string reason, DateTime? banUntil);
        Task<ContentResult> UnbanUserAsync(int userId);
        
        // 系統設定
        Task<SettingResult> UpdateSystemSettingAsync(string key, string value);
        Task<SettingResult> GetSystemSettingAsync(string key);
        Task<Dictionary<string, string>> GetAllSettingsAsync();
        
        // 日誌管理
        Task<List<SystemLog>> GetSystemLogsAsync(int page = 1, int pageSize = 50);
        Task<List<SystemLog>> GetLogsByLevelAsync(string level, int page = 1, int pageSize = 50);
        Task<LogResult> ClearOldLogsAsync(int daysToKeep = 30);
    }

    public class AdminResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ManagerDatum? Admin { get; set; }
    }

    public class UserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
    }

    public class SystemStatsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalGroups { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int NewUsersToday { get; set; }
        public int NewPostsToday { get; set; }
    }

    public class ContentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class SettingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    public class LogResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int DeletedCount { get; set; }
    }

    public class SystemLog
    {
        public int Id { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public string? Action { get; set; }
    }
}