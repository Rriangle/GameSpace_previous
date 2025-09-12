using System.Threading.Tasks;

namespace GameSpace.Services.Monitoring
{
    public interface IPerformanceService
    {
        Task<PerformanceMetrics> GetSystemMetricsAsync();
        Task<DatabaseMetrics> GetDatabaseMetricsAsync();
        Task<MemoryMetrics> GetMemoryMetricsAsync();
        Task<CpuMetrics> GetCpuMetricsAsync();
        Task<NetworkMetrics> GetNetworkMetricsAsync();
        Task<ApplicationMetrics> GetApplicationMetricsAsync();
        Task StartPerformanceTrackingAsync();
        Task StopPerformanceTrackingAsync();
        Task LogPerformanceEventAsync(string eventName, TimeSpan duration, Dictionary<string, object>? metadata = null);
    }

    public class PerformanceMetrics
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public CpuMetrics Cpu { get; set; } = new();
        public MemoryMetrics Memory { get; set; } = new();
        public DatabaseMetrics Database { get; set; } = new();
        public NetworkMetrics Network { get; set; } = new();
        public ApplicationMetrics Application { get; set; } = new();
    }

    public class CpuMetrics
    {
        public double UsagePercentage { get; set; }
        public int ProcessCount { get; set; }
        public TimeSpan Uptime { get; set; }
    }

    public class MemoryMetrics
    {
        public long TotalMemory { get; set; }
        public long UsedMemory { get; set; }
        public long AvailableMemory { get; set; }
        public double UsagePercentage { get; set; }
        public long GcMemory { get; set; }
    }

    public class DatabaseMetrics
    {
        public int ActiveConnections { get; set; }
        public TimeSpan AverageQueryTime { get; set; }
        public long TotalQueries { get; set; }
        public long FailedQueries { get; set; }
        public long DatabaseSize { get; set; }
    }

    public class NetworkMetrics
    {
        public long BytesReceived { get; set; }
        public long BytesSent { get; set; }
        public int ActiveConnections { get; set; }
        public double RequestsPerSecond { get; set; }
    }

    public class ApplicationMetrics
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
    }
}