using System.Text;

namespace GameSpace.Middleware
{
    /// <summary>
    /// CorrelationId 中介軟體，用於追蹤請求的唯一識別符
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 檢查請求中是否已有 Correlation ID
            var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

            // 如果沒有，則生成一個新的
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // 將 Correlation ID 加入回應標頭
            context.Response.Headers[CorrelationIdHeader] = correlationId;

            // 將 Correlation ID 加入記錄內容
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
            {
                _logger.LogInformation("處理請求 {Method} {Path} (CorrelationId: {CorrelationId})", 
                    context.Request.Method, context.Request.Path, correlationId);

                await _next(context);
            }
        }
    }
}