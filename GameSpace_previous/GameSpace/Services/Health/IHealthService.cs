using System.Threading.Tasks;

namespace GameSpace.Services.Health
{
    public interface IHealthService
    {
        Task<HealthCheckResult> CheckDatabaseHealthAsync();
        Task<HealthCheckResult> CheckCacheHealthAsync();
        Task<HealthCheckResult> CheckExternalServicesHealthAsync();
        Task<HealthCheckResult> CheckDiskSpaceHealthAsync();
        Task<HealthCheckResult> CheckMemoryHealthAsync();
        Task<OverallHealthResult> GetOverallHealthAsync();
        Task<HealthCheckResult> CheckSpecificServiceAsync(string serviceName);
    }

    public class HealthCheckResult
    {
        public string ServiceName { get; set; } = string.Empty;
        public HealthStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan ResponseTime { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Details { get; set; } = new();
    }

    public class OverallHealthResult
    {
        public HealthStatus OverallStatus { get; set; }
        public List<HealthCheckResult> ServiceResults { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Summary { get; set; } = string.Empty;
    }

    public enum HealthStatus
    {
        Healthy,
        Degraded,
        Unhealthy,
        Unknown
    }
}