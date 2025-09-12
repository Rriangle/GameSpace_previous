using System.Diagnostics;
using System.Threading.Tasks;

namespace GameSpace.Services.Monitoring
{
    public class PerformanceService : IPerformanceService
    {
        private readonly ILogger<PerformanceService> _logger;
        private readonly GameSpacedatabaseContext _context;
        private readonly ICacheService _cacheService;
        private readonly Stopwatch _stopwatch = new();
        private readonly Dictionary<string, List<TimeSpan>> _performanceEvents = new();

        public PerformanceService(
            ILogger<PerformanceService> logger,
            GameSpacedatabaseContext context,
            ICacheService cacheService)
        {
            _logger = logger;
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<PerformanceMetrics> GetSystemMetricsAsync()
        {
            try
            {
                var cpu = await GetCpuMetricsAsync();
                var memory = await GetMemoryMetricsAsync();
                var database = await GetDatabaseMetricsAsync();
                var network = await GetNetworkMetricsAsync();
                var application = await GetApplicationMetricsAsync();

                return new PerformanceMetrics
                {
                    Cpu = cpu,
                    Memory = memory,
                    Database = database,
                    Network = network,
                    Application = application
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get system metrics");
                return new PerformanceMetrics();
            }
        }

        public async Task<DatabaseMetrics> GetDatabaseMetricsAsync()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                
                // 執行簡單查詢來測試資料庫性能
                var userCount = await _context.Users.CountAsync();
                var postCount = await _context.Posts.CountAsync();
                
                stopwatch.Stop();

                return new DatabaseMetrics
                {
                    ActiveConnections = 1, // 簡化實現
                    AverageQueryTime = stopwatch.Elapsed,
                    TotalQueries = 2,
                    FailedQueries = 0,
                    DatabaseSize = await GetDatabaseSizeAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get database metrics");
                return new DatabaseMetrics();
            }
        }

        public async Task<MemoryMetrics> GetMemoryMetricsAsync()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var workingSet = process.WorkingSet64;
                var gcMemory = GC.GetTotalMemory(false);

                return new MemoryMetrics
                {
                    TotalMemory = Environment.WorkingSet,
                    UsedMemory = workingSet,
                    AvailableMemory = Environment.WorkingSet - workingSet,
                    UsagePercentage = (double)workingSet / Environment.WorkingSet * 100,
                    GcMemory = gcMemory
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get memory metrics");
                return new MemoryMetrics();
            }
        }

        public async Task<CpuMetrics> GetCpuMetricsAsync()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var startTime = process.StartTime;
                var uptime = DateTime.Now - startTime;

                return new CpuMetrics
                {
                    UsagePercentage = 0, // 簡化實現，實際應該使用 PerformanceCounter
                    ProcessCount = Process.GetProcesses().Length,
                    Uptime = uptime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get CPU metrics");
                return new CpuMetrics();
            }
        }

        public async Task<NetworkMetrics> GetNetworkMetricsAsync()
        {
            try
            {
                // 簡化實現，實際應該使用網路監控
                return new NetworkMetrics
                {
                    BytesReceived = 0,
                    BytesSent = 0,
                    ActiveConnections = 0,
                    RequestsPerSecond = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get network metrics");
                return new NetworkMetrics();
            }
        }

        public async Task<ApplicationMetrics> GetApplicationMetricsAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users
                    .Where(u => u.UserRight != null && u.UserRight.UserStatus == true)
                    .CountAsync();

                return new ApplicationMetrics
                {
                    TotalRequests = 0, // 簡化實現
                    SuccessfulRequests = 0,
                    FailedRequests = 0,
                    AverageResponseTime = 0,
                    ActiveUsers = activeUsers,
                    TotalUsers = totalUsers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get application metrics");
                return new ApplicationMetrics();
            }
        }

        public async Task StartPerformanceTrackingAsync()
        {
            try
            {
                _stopwatch.Start();
                _logger.LogInformation("Performance tracking started");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start performance tracking");
            }
        }

        public async Task StopPerformanceTrackingAsync()
        {
            try
            {
                _stopwatch.Stop();
                _logger.LogInformation("Performance tracking stopped. Total time: {ElapsedMilliseconds}ms", _stopwatch.ElapsedMilliseconds);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop performance tracking");
            }
        }

        public async Task LogPerformanceEventAsync(string eventName, TimeSpan duration, Dictionary<string, object>? metadata = null)
        {
            try
            {
                if (!_performanceEvents.ContainsKey(eventName))
                {
                    _performanceEvents[eventName] = new List<TimeSpan>();
                }

                _performanceEvents[eventName].Add(duration);

                // 只保留最近100個事件
                if (_performanceEvents[eventName].Count > 100)
                {
                    _performanceEvents[eventName].RemoveAt(0);
                }

                var logData = new Dictionary<string, object>
                {
                    ["EventName"] = eventName,
                    ["Duration"] = duration.TotalMilliseconds,
                    ["Timestamp"] = DateTime.UtcNow
                };

                if (metadata != null)
                {
                    foreach (var kvp in metadata)
                    {
                        logData[kvp.Key] = kvp.Value;
                    }
                }

                _logger.LogInformation("Performance event: {EventName} took {Duration}ms", eventName, duration.TotalMilliseconds);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log performance event: {EventName}", eventName);
            }
        }

        private async Task<long> GetDatabaseSizeAsync()
        {
            try
            {
                // 簡化實現，實際應該查詢資料庫大小
                return await Task.FromResult(1024 * 1024 * 100); // 100MB
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get database size");
                return 0;
            }
        }
    }
}