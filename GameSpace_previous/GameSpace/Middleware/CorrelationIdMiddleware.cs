using System.Text;

namespace GameSpace.Middleware
{
    /// <summary>
    /// CorrelationId middleware for tracking unique request identifiers
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
            // Check if request already has Correlation ID
            var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

            // If not, generate a new one
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // Add Correlation ID to response headers
            context.Response.Headers[CorrelationIdHeader] = correlationId;

            // Add Correlation ID to log context
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
            {
                _logger.LogInformation("Processing request {Method} {Path} (CorrelationId: {CorrelationId})", 
                    context.Request.Method, context.Request.Path, correlationId);

                await _next(context);
            }
        }
    }
}