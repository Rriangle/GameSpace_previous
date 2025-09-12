using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GameSpace.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 添加安全標頭
                AddSecurityHeaders(context);

                // 處理請求
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Security headers middleware error");
                throw;
            }
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            var response = context.Response;

            // 防止點擊劫持
            response.Headers.Add("X-Frame-Options", "DENY");

            // 防止 MIME 類型嗅探
            response.Headers.Add("X-Content-Type-Options", "nosniff");

            // XSS 保護
            response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // 強制 HTTPS
            response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

            // 內容安全策略
            response.Headers.Add("Content-Security-Policy", 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdnjs.cloudflare.com https://stackpath.bootstrapcdn.com; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdnjs.cloudflare.com https://stackpath.bootstrapcdn.com; " +
                "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
                "img-src 'self' data: https:; " +
                "connect-src 'self' https:; " +
                "frame-ancestors 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self'");

            // 引用者策略
            response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            // 權限策略
            response.Headers.Add("Permissions-Policy", 
                "camera=(), " +
                "microphone=(), " +
                "geolocation=(), " +
                "interest-cohort=()");

            // 移除服務器信息
            response.Headers.Remove("Server");
            response.Headers.Remove("X-Powered-By");
            response.Headers.Remove("X-AspNet-Version");
            response.Headers.Remove("X-AspNetMvc-Version");
        }
    }
}