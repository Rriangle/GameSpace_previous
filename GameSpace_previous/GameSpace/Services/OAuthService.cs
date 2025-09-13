using GameSpace.Data;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GameSpace.Services
{
    /// <summary>
    /// OAuth認證服務
    /// </summary>
    public class OAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OAuthService> _logger;

        public OAuthService(ApplicationDbContext context, ILogger<OAuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 儲存OAuth令牌
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="provider">提供者名稱</param>
        /// <param name="tokenName">令牌名稱</param>
        /// <param name="tokenValue">令牌值</param>
        /// <param name="expireAt">過期時間</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SaveTokenAsync(int userId, string provider, string tokenName, string tokenValue, DateTime expireAt)
        {
            try
            {
                // 檢查是否已存在相同的令牌
                var existingToken = await _context.UserTokens
                    .FirstOrDefaultAsync(t => t.UserId == userId && 
                                            t.Provider == provider && 
                                            t.Name == tokenName);

                if (existingToken != null)
                {
                    // 更新現有令牌
                    existingToken.Value = tokenValue;
                    existingToken.ExpireAt = expireAt;
                }
                else
                {
                    // 創建新令牌
                    var newToken = new UserToken
                    {
                        UserId = userId,
                        Provider = provider,
                        Name = tokenName,
                        Value = tokenValue,
                        ExpireAt = expireAt
                    };
                    _context.UserTokens.Add(newToken);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("OAuth令牌已儲存：用戶ID {UserId}, 提供者 {Provider}, 令牌類型 {TokenName}", 
                    userId, provider, tokenName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "儲存OAuth令牌時發生錯誤：用戶ID {UserId}, 提供者 {Provider}", userId, provider);
                return false;
            }
        }

        /// <summary>
        /// 獲取OAuth令牌
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="provider">提供者名稱</param>
        /// <param name="tokenName">令牌名稱</param>
        /// <returns>令牌值，如果不存在或已過期則返回null</returns>
        public async Task<string?> GetTokenAsync(int userId, string provider, string tokenName)
        {
            try
            {
                var token = await _context.UserTokens
                    .FirstOrDefaultAsync(t => t.UserId == userId && 
                                            t.Provider == provider && 
                                            t.Name == tokenName &&
                                            t.ExpireAt > DateTime.UtcNow);

                return token?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取OAuth令牌時發生錯誤：用戶ID {UserId}, 提供者 {Provider}, 令牌類型 {TokenName}", 
                    userId, provider, tokenName);
                return null;
            }
        }

        /// <summary>
        /// 刪除過期的OAuth令牌
        /// </summary>
        /// <returns>刪除的令牌數量</returns>
        public async Task<int> CleanupExpiredTokensAsync()
        {
            try
            {
                var expiredTokens = await _context.UserTokens
                    .Where(t => t.ExpireAt <= DateTime.UtcNow)
                    .ToListAsync();

                if (expiredTokens.Any())
                {
                    _context.UserTokens.RemoveRange(expiredTokens);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("已清理 {Count} 個過期的OAuth令牌", expiredTokens.Count);
                }

                return expiredTokens.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理過期OAuth令牌時發生錯誤");
                return 0;
            }
        }

        /// <summary>
        /// 檢查用戶是否已綁定特定提供者
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="provider">提供者名稱</param>
        /// <returns>是否已綁定</returns>
        public async Task<bool> IsProviderLinkedAsync(int userId, string provider)
        {
            try
            {
                return await _context.UserTokens
                    .AnyAsync(t => t.UserId == userId && 
                                 t.Provider == provider &&
                                 t.ExpireAt > DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查提供者綁定狀態時發生錯誤：用戶ID {UserId}, 提供者 {Provider}", userId, provider);
                return false;
            }
        }

        /// <summary>
        /// 取消綁定OAuth提供者
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="provider">提供者名稱</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UnlinkProviderAsync(int userId, string provider)
        {
            try
            {
                var tokens = await _context.UserTokens
                    .Where(t => t.UserId == userId && t.Provider == provider)
                    .ToListAsync();

                if (tokens.Any())
                {
                    _context.UserTokens.RemoveRange(tokens);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("已取消綁定OAuth提供者：用戶ID {UserId}, 提供者 {Provider}", userId, provider);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消綁定OAuth提供者時發生錯誤：用戶ID {UserId}, 提供者 {Provider}", userId, provider);
                return false;
            }
        }

        /// <summary>
        /// 生成安全的隨機字符串
        /// </summary>
        /// <param name="length">長度</param>
        /// <returns>隨機字符串</returns>
        public static string GenerateRandomString(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            var result = new StringBuilder(length);
            foreach (var b in bytes)
            {
                result.Append(chars[b % chars.Length]);
            }
            return result.ToString();
        }
    }
}