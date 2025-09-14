using GameSpace.Core.Models;
using System.Net;
using System.Text.Json;

namespace GameSpace.Middleware
{
    /// <summary>
    /// 全域異常處理中介軟體
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "未處理的異常發生");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "內部伺服器錯誤",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = exception.Message,
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            // 根據異常類型設定不同的狀態碼和訊息
            switch (exception)
            {
                case ArgumentNullException:
                    errorResponse.Status = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = "請求參數錯誤";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    break;
                case ArgumentException:
                    errorResponse.Status = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = "請求參數錯誤";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    break;
                case UnauthorizedAccessException:
                    errorResponse.Status = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Title = "未授權";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                    break;
                case KeyNotFoundException:
                    errorResponse.Status = (int)HttpStatusCode.NotFound;
                    errorResponse.Title = "資源不存在";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    break;
                case TimeoutException:
                    errorResponse.Status = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Title = "請求超時";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.7";
                    break;
            }

            context.Response.StatusCode = errorResponse.Status;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
