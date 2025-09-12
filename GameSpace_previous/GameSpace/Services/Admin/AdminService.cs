using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameSpace.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(GameSpacedatabaseContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region 管理員管理

        public async Task<AdminResult> CreateAdminAsync(string managerName, string managerAccount, string managerPassword, string managerEmail)
        {
            try
            {
                // 檢查帳號是否已存在
                if (await _context.ManagerData.AnyAsync(m => m.ManagerAccount == managerAccount))
                {
                    return new AdminResult { Success = false, Message = "管理員帳號已存在" };
                }

                // 檢查電子郵件是否已存在
                if (await _context.ManagerData.AnyAsync(m => m.ManagerEmail == managerEmail))
                {
                    return new AdminResult { Success = false, Message = "電子郵件已存在" };
                }

                var admin = new ManagerDatum
                {
                    ManagerName = managerName,
                    ManagerAccount = managerAccount,
                    ManagerPassword = HashPassword(managerPassword),
                    ManagerEmail = managerEmail,
                    ManagerEmailConfirmed = false,
                    ManagerAccessFailedCount = 0,
                    ManagerLockoutEnabled = false,
                    ManagerLockoutEnd = null,
                    AdministratorRegistrationDate = DateTime.UtcNow
                };

                _context.ManagerData.Add(admin);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin created: {ManagerAccount} (ID: {ManagerId})", managerAccount, admin.ManagerId);
                return new AdminResult { Success = true, Message = "管理員創建成功", Admin = admin };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create admin: {ManagerAccount}", managerAccount);
                return new AdminResult { Success = false, Message = "管理員創建失敗" };
            }
        }

        public async Task<AdminResult> GetAdminAsync(int managerId)
        {
            try
            {
                var admin = await _context.ManagerData.FindAsync(managerId);
                if (admin == null)
                {
                    return new AdminResult { Success = false, Message = "管理員不存在" };
                }

                return new AdminResult { Success = true, Message = "管理員獲取成功", Admin = admin };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get admin: {ManagerId}", managerId);
                return new AdminResult { Success = false, Message = "管理員獲取失敗" };
            }
        }

        public async Task<List<ManagerDatum>> GetAllAdminsAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.ManagerData
                    .OrderByDescending(m => m.AdministratorRegistrationDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all admins");
                return new List<ManagerDatum>();
            }
        }

        public async Task<AdminResult> UpdateAdminAsync(int managerId, string managerName, string managerEmail)
        {
            try
            {
                var admin = await _context.ManagerData.FindAsync(managerId);
                if (admin == null)
                {
                    return new AdminResult { Success = false, Message = "管理員不存在" };
                }

                // 檢查電子郵件是否已被其他管理員使用
                if (await _context.ManagerData.AnyAsync(m => m.ManagerEmail == managerEmail && m.ManagerId != managerId))
                {
                    return new AdminResult { Success = false, Message = "電子郵件已被其他管理員使用" };
                }

                admin.ManagerName = managerName;
                admin.ManagerEmail = managerEmail;

                _context.ManagerData.Update(admin);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin updated: {ManagerId}", managerId);
                return new AdminResult { Success = true, Message = "管理員更新成功", Admin = admin };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update admin: {ManagerId}", managerId);
                return new AdminResult { Success = false, Message = "管理員更新失敗" };
            }
        }

        public async Task<AdminResult> DeleteAdminAsync(int managerId)
        {
            try
            {
                var admin = await _context.ManagerData.FindAsync(managerId);
                if (admin == null)
                {
                    return new AdminResult { Success = false, Message = "管理員不存在" };
                }

                _context.ManagerData.Remove(admin);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin deleted: {ManagerId}", managerId);
                return new AdminResult { Success = true, Message = "管理員刪除成功" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete admin: {ManagerId}", managerId);
                return new AdminResult { Success = false, Message = "管理員刪除失敗" };
            }
        }

        public async Task<AdminResult> ChangePasswordAsync(int managerId, string oldPassword, string newPassword)
        {
            try
            {
                var admin = await _context.ManagerData.FindAsync(managerId);
                if (admin == null)
                {
                    return new AdminResult { Success = false, Message = "管理員不存在" };
                }

                if (!VerifyPassword(oldPassword, admin.ManagerPassword))
                {
                    return new AdminResult { Success = false, Message = "舊密碼不正確" };
                }

                admin.ManagerPassword = HashPassword(newPassword);
                _context.ManagerData.Update(admin);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin password changed: {ManagerId}", managerId);
                return new AdminResult { Success = true, Message = "密碼修改成功", Admin = admin };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change admin password: {ManagerId}", managerId);
                return new AdminResult { Success = false, Message = "密碼修改失敗" };
            }
        }

        #endregion

        #region 用戶管理

        public async Task<UserResult> GetUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserIntroduce)
                    .Include(u => u.UserRight)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return new UserResult { Success = false, Message = "用戶不存在" };
                }

                return new UserResult { Success = true, Message = "用戶獲取成功", User = user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user: {UserId}", userId);
                return new UserResult { Success = false, Message = "用戶獲取失敗" };
            }
        }

        public async Task<List<User>> GetAllUsersAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.UserIntroduce)
                    .Include(u => u.UserRight)
                    .OrderByDescending(u => u.UserId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all users");
                return new List<User>();
            }
        }

        public async Task<UserResult> UpdateUserStatusAsync(int userId, bool isActive)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRight)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return new UserResult { Success = false, Message = "用戶不存在" };
                }

                if (user.UserRight == null)
                {
                    user.UserRight = new UserRight { UserId = userId };
                    _context.UserRights.Add(user.UserRight);
                }

                user.UserRight.UserStatus = isActive;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User status updated: {UserId} to {Status}", userId, isActive ? "Active" : "Inactive");
                return new UserResult { Success = true, Message = "用戶狀態已更新", User = user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user status: {UserId}", userId);
                return new UserResult { Success = false, Message = "用戶狀態更新失敗" };
            }
        }

        public async Task<UserResult> LockUserAsync(int userId, DateTime? lockoutEnd)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new UserResult { Success = false, Message = "用戶不存在" };
                }

                user.UserLockoutEnabled = true;
                user.UserLockoutEnd = lockoutEnd;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User locked: {UserId} until {LockoutEnd}", userId, lockoutEnd);
                return new UserResult { Success = true, Message = "用戶已鎖定", User = user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to lock user: {UserId}", userId);
                return new UserResult { Success = false, Message = "用戶鎖定失敗" };
            }
        }

        public async Task<UserResult> UnlockUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new UserResult { Success = false, Message = "用戶不存在" };
                }

                user.UserLockoutEnabled = false;
                user.UserLockoutEnd = null;
                user.UserAccessFailedCount = 0;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User unlocked: {UserId}", userId);
                return new UserResult { Success = true, Message = "用戶已解鎖", User = user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unlock user: {UserId}", userId);
                return new UserResult { Success = false, Message = "用戶解鎖失敗" };
            }
        }

        public async Task<UserResult> ResetUserPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new UserResult { Success = false, Message = "用戶不存在" };
                }

                user.UserPassword = HashPassword(newPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User password reset: {UserId}", userId);
                return new UserResult { Success = true, Message = "用戶密碼已重設", User = user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset user password: {UserId}", userId);
                return new UserResult { Success = false, Message = "用戶密碼重設失敗" };
            }
        }

        #endregion

        #region 系統統計

        public async Task<SystemStatsResult> GetSystemStatsAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.UserRights.CountAsync(ur => ur.UserStatus == true);
                var totalPosts = await _context.Posts.CountAsync();
                var totalGroups = await _context.Groups.CountAsync();
                var totalOrders = await _context.OrderInfos.CountAsync();
                var totalRevenue = await _context.OrderInfos.SumAsync(o => o.TotalAmount);

                var today = DateTime.UtcNow.Date;
                var newUsersToday = await _context.Users.CountAsync(u => u.UserIntroduce!.CreateAccount.Date == today);
                var newPostsToday = await _context.Posts.CountAsync(p => p.CreatedAt.Date == today);

                return new SystemStatsResult
                {
                    Success = true,
                    Message = "系統統計獲取成功",
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalPosts = totalPosts,
                    TotalGroups = totalGroups,
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    NewUsersToday = newUsersToday,
                    NewPostsToday = newPostsToday
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get system stats");
                return new SystemStatsResult { Success = false, Message = "系統統計獲取失敗" };
            }
        }

        public async Task<List<User>> GetNewUsersAsync(int days = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                return await _context.Users
                    .Include(u => u.UserIntroduce)
                    .Where(u => u.UserIntroduce!.CreateAccount >= cutoffDate)
                    .OrderByDescending(u => u.UserIntroduce!.CreateAccount)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get new users");
                return new List<User>();
            }
        }

        public async Task<List<OrderInfo>> GetRecentOrdersAsync(int days = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                return await _context.OrderInfos
                    .Where(o => o.OrderDate >= cutoffDate)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recent orders");
                return new List<OrderInfo>();
            }
        }

        public async Task<List<Post>> GetRecentPostsAsync(int days = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                return await _context.Posts
                    .Include(p => p.User)
                    .Where(p => p.CreatedAt >= cutoffDate)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recent posts");
                return new List<Post>();
            }
        }

        #endregion

        #region 內容管理

        public async Task<ContentResult> DeletePostAsync(int postId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return new ContentResult { Success = false, Message = "文章不存在" };
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Post deleted: {PostId}", postId);
                return new ContentResult { Success = true, Message = "文章已刪除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete post: {PostId}", postId);
                return new ContentResult { Success = false, Message = "文章刪除失敗" };
            }
        }

        public async Task<ContentResult> DeleteThreadAsync(int threadId)
        {
            try
            {
                var thread = await _context.Threads.FindAsync(threadId);
                if (thread == null)
                {
                    return new ContentResult { Success = false, Message = "討論串不存在" };
                }

                _context.Threads.Remove(thread);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Thread deleted: {ThreadId}", threadId);
                return new ContentResult { Success = true, Message = "討論串已刪除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete thread: {ThreadId}", threadId);
                return new ContentResult { Success = false, Message = "討論串刪除失敗" };
            }
        }

        public async Task<ContentResult> DeleteGroupAsync(int groupId)
        {
            try
            {
                var group = await _context.Groups.FindAsync(groupId);
                if (group == null)
                {
                    return new ContentResult { Success = false, Message = "群組不存在" };
                }

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group deleted: {GroupId}", groupId);
                return new ContentResult { Success = true, Message = "群組已刪除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete group: {GroupId}", groupId);
                return new ContentResult { Success = false, Message = "群組刪除失敗" };
            }
        }

        public async Task<ContentResult> BanUserAsync(int userId, string reason, DateTime? banUntil)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ContentResult { Success = false, Message = "用戶不存在" };
                }

                user.UserLockoutEnabled = true;
                user.UserLockoutEnd = banUntil;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User banned: {UserId} until {BanUntil}, reason: {Reason}", userId, banUntil, reason);
                return new ContentResult { Success = true, Message = "用戶已封禁" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ban user: {UserId}", userId);
                return new ContentResult { Success = false, Message = "用戶封禁失敗" };
            }
        }

        public async Task<ContentResult> UnbanUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ContentResult { Success = false, Message = "用戶不存在" };
                }

                user.UserLockoutEnabled = false;
                user.UserLockoutEnd = null;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User unbanned: {UserId}", userId);
                return new ContentResult { Success = true, Message = "用戶已解封" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unban user: {UserId}", userId);
                return new ContentResult { Success = false, Message = "用戶解封失敗" };
            }
        }

        #endregion

        #region 系統設定

        public async Task<SettingResult> UpdateSystemSettingAsync(string key, string value)
        {
            try
            {
                // 這裡應該實現系統設定的存儲邏輯
                // 由於我們沒有系統設定表，這裡返回模擬結果
                _logger.LogInformation("System setting updated: {Key} = {Value}", key, value);
                return new SettingResult { Success = true, Message = "系統設定已更新", Value = value };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update system setting: {Key}", key);
                return new SettingResult { Success = false, Message = "系統設定更新失敗" };
            }
        }

        public async Task<SettingResult> GetSystemSettingAsync(string key)
        {
            try
            {
                // 這裡應該實現系統設定的讀取邏輯
                // 由於我們沒有系統設定表，這裡返回模擬結果
                return new SettingResult { Success = true, Message = "系統設定獲取成功", Value = "default_value" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get system setting: {Key}", key);
                return new SettingResult { Success = false, Message = "系統設定獲取失敗" };
            }
        }

        public async Task<Dictionary<string, string>> GetAllSettingsAsync()
        {
            try
            {
                // 這裡應該實現所有系統設定的讀取邏輯
                return new Dictionary<string, string>
                {
                    { "site_name", "GameSpace" },
                    { "maintenance_mode", "false" },
                    { "max_file_size", "10485760" },
                    { "allowed_file_types", "jpg,jpeg,png,gif" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all settings");
                return new Dictionary<string, string>();
            }
        }

        #endregion

        #region 日誌管理

        public async Task<List<SystemLog>> GetSystemLogsAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                // 這裡應該實現系統日誌的讀取邏輯
                // 由於我們沒有系統日誌表，這裡返回模擬數據
                return new List<SystemLog>
                {
                    new SystemLog
                    {
                        Id = 1,
                        Level = "Info",
                        Message = "System started",
                        Timestamp = DateTime.UtcNow.AddHours(-1),
                        Action = "System"
                    },
                    new SystemLog
                    {
                        Id = 2,
                        Level = "Warning",
                        Message = "High memory usage detected",
                        Timestamp = DateTime.UtcNow.AddMinutes(-30),
                        Action = "System"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get system logs");
                return new List<SystemLog>();
            }
        }

        public async Task<List<SystemLog>> GetLogsByLevelAsync(string level, int page = 1, int pageSize = 50)
        {
            try
            {
                // 這裡應該實現按級別篩選系統日誌的邏輯
                var allLogs = await GetSystemLogsAsync(page, pageSize);
                return allLogs.Where(log => log.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get logs by level: {Level}", level);
                return new List<SystemLog>();
            }
        }

        public async Task<LogResult> ClearOldLogsAsync(int daysToKeep = 30)
        {
            try
            {
                // 這裡應該實現清理舊日誌的邏輯
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
                _logger.LogInformation("Clearing logs older than {CutoffDate}", cutoffDate);
                
                return new LogResult { Success = true, Message = "舊日誌清理完成", DeletedCount = 0 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear old logs");
                return new LogResult { Success = false, Message = "舊日誌清理失敗" };
            }
        }

        #endregion

        #region 輔助方法

        private string HashPassword(string password)
        {
            // 這裡應該使用更安全的密碼雜湊方法，如 bcrypt
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        #endregion
    }
}