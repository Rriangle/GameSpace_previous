using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GameSpace.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
        private readonly int _maxRequests;
        private readonly TimeSpan _window;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger, int maxRequests = 100, int windowMinutes = 1)
        {
            _next = next;
            _logger = logger;
            _maxRequests = maxRequests;
            _window = TimeSpan.FromMinutes(windowMinutes);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var clientId = GetClientIdentifier(context);
                var now = DateTime.UtcNow;

                // 清理過期的記錄
                CleanupExpiredEntries(now);

                // 檢查速率限制
                if (IsRateLimited(clientId, now))
                {
                    _logger.LogWarning("Rate limit exceeded for client: {ClientId}", clientId);
                    context.Response.StatusCode = 429;
                    context.Response.Headers.Add("Retry-After", _window.TotalSeconds.ToString());
                    await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    return;
                }

                // 記錄請求
                RecordRequest(clientId, now);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rate limit middleware error");
                throw;
            }
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // 優先使用真實 IP，然後是連接 ID
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            return $"{ip}:{userAgent.GetHashCode()}";
        }

        private bool IsRateLimited(string clientId, DateTime now)
        {
            if (!_rateLimitStore.TryGetValue(clientId, out var info))
            {
                return false;
            }

            // 檢查是否在時間窗口內
            if (now - info.WindowStart > _window)
            {
                return false;
            }

            return info.RequestCount >= _maxRequests;
        }

        private void RecordRequest(string clientId, DateTime now)
        {
            _rateLimitStore.AddOrUpdate(clientId, 
                new RateLimitInfo { RequestCount = 1, WindowStart = now },
                (key, existing) =>
                {
                    if (now - existing.WindowStart > _window)
                    {
                        return new RateLimitInfo { RequestCount = 1, WindowStart = now };
                    }
                    return new RateLimitInfo { RequestCount = existing.RequestCount + 1, WindowStart = existing.WindowStart };
                });
        }

        private void CleanupExpiredEntries(DateTime now)
        {
            var expiredKeys = _rateLimitStore
                .Where(kvp => now - kvp.Value.WindowStart > _window)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _rateLimitStore.TryRemove(key, out _);
            }
        }
    }

    public class RateLimitInfo
    {
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; }
    }
}