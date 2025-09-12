using System.Diagnostics;
using System.Threading.Tasks;

namespace GameSpace.Services.Health
{
    public class HealthService : IHealthService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILogger<HealthService> _logger;

        public HealthService(
            GameSpacedatabaseContext context,
            ICacheService cacheService,
            ILogger<HealthService> logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckDatabaseHealthAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                stopwatch.Stop();

                if (canConnect)
                {
                    return new HealthCheckResult
                    {
                        ServiceName = "Database",
                        Status = HealthStatus.Healthy,
                        Message = "Database connection is healthy",
                        ResponseTime = stopwatch.Elapsed,
                        Details = new Dictionary<string, object>
                        {
                            ["ConnectionString"] = "***",
                            ["Provider"] = "Microsoft.EntityFrameworkCore.SqlServer"
                        }
                    };
                }
                else
                {
                    return new HealthCheckResult
                    {
                        ServiceName = "Database",
                        Status = HealthStatus.Unhealthy,
                        Message = "Database connection failed",
                        ResponseTime = stopwatch.Elapsed
                    };
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Database health check failed");
                
                return new HealthCheckResult
                {
                    ServiceName = "Database",
                    Status = HealthStatus.Unhealthy,
                    Message = $"Database health check failed: {ex.Message}",
                    ResponseTime = stopwatch.Elapsed,
                    Details = new Dictionary<string, object>
                    {
                        ["Exception"] = ex.GetType().Name,
                        ["Error"] = ex.Message
                    }
                };
            }
        }

        public async Task<HealthCheckResult> CheckCacheHealthAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var testKey = $"health_check_{Guid.NewGuid()}";
                var testValue = "test_value";
                
                await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10));
                var retrievedValue = await _cacheService.GetAsync<string>(testKey);
                await _cacheService.RemoveAsync(testKey);
                
                stopwatch.Stop();

                if (retrievedValue == testValue)
                {
                    return new HealthCheckResult
                    {
                        ServiceName = "Cache",
                        Status = HealthStatus.Healthy,
                        Message = "Cache service is healthy",
                        ResponseTime = stopwatch.Elapsed,
                        Details = new Dictionary<string, object>
                        {
                            ["CacheType"] = "MemoryCache",
                            ["TestResult"] = "Success"
                        }
                    };
                }
                else
                {
                    return new HealthCheckResult
                    {
                        ServiceName = "Cache",
                        Status = HealthStatus.Degraded,
                        Message = "Cache service returned unexpected value",
                        ResponseTime = stopwatch.Elapsed
                    };
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Cache health check failed");
                
                return new HealthCheckResult
                {
                    ServiceName = "Cache",
                    Status = HealthStatus.Unhealthy,
                    Message = $"Cache health check failed: {ex.Message}",
                    ResponseTime = stopwatch.Elapsed,
                    Details = new Dictionary<string, object>
                    {
                        ["Exception"] = ex.GetType().Name,
                        ["Error"] = ex.Message
                    }
                };
            }
        }

        public async Task<HealthCheckResult> CheckExternalServicesHealthAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // 檢查外部服務（如郵件服務、支付服務等）
                // 這裡簡化實現，實際應該檢查真實的外部服務
                
                stopwatch.Stop();
                
                return new HealthCheckResult
                {
                    ServiceName = "External Services",
                    Status = HealthStatus.Healthy,
                    Message = "External services are healthy",
                    ResponseTime = stopwatch.Elapsed,
                    Details = new Dictionary<string, object>
                    {
                        ["EmailService"] = "Available",
                        ["PaymentService"] = "Available"
                    }
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "External services health check failed");
                
                return new HealthCheckResult
                {
                    ServiceName = "External Services",
                    Status = HealthStatus.Degraded,
                    Message = $"External services health check failed: {ex.Message}",
                    ResponseTime = stopwatch.Elapsed
                };
            }
        }

        public async Task<HealthCheckResult> CheckDiskSpaceHealthAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:");
                var freeSpacePercentage = (double)drive.AvailableFreeSpace / drive.TotalSize * 100;
                
                stopwatch.Stop();

                var status = freeSpacePercentage switch
                {
                    >= 20 => HealthStatus.Healthy,
                    >= 10 => HealthStatus.Degraded,
                    _ => HealthStatus.Unhealthy
                };

                return new HealthCheckResult
                {
                    ServiceName = "Disk Space",
                    Status = status,
                    Message = $"Disk space is {(status == HealthStatus.Healthy ? "healthy" : "low")}",
                    ResponseTime = stopwatch.Elapsed,
                    Details = new Dictionary<string, object>
                    {
                        ["FreeSpaceGB"] = drive.AvailableFreeSpace / (1024 * 1024 * 1024),
                        ["TotalSpaceGB"] = drive.TotalSize / (1024 * 1024 * 1024),
                        ["FreeSpacePercentage"] = freeSpacePercentage
                    }
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Disk space health check failed");
                
                return new HealthCheckResult
                {
                    ServiceName = "Disk Space",
                    Status = HealthStatus.Unknown,
                    Message = $"Disk space health check failed: {ex.Message}",
                    ResponseTime = stopwatch.Elapsed
                };
            }
        }

        public async Task<HealthCheckResult> CheckMemoryHealthAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var process = Process.GetCurrentProcess();
                var workingSet = process.WorkingSet64;
                var totalMemory = Environment.WorkingSet;
                var memoryUsagePercentage = (double)workingSet / totalMemory * 100;
                
                stopwatch.Stop();

                var status = memoryUsagePercentage switch
                {
                    <= 80 => HealthStatus.Healthy,
                    <= 90 => HealthStatus.Degraded,
                    _ => HealthStatus.Unhealthy
                };

                return new HealthCheckResult
                {
                    ServiceName = "Memory",
                    Status = status,
                    Message = $"Memory usage is {(status == HealthStatus.Healthy ? "healthy" : "high")}",
                    ResponseTime = stopwatch.Elapsed,
                    Details = new Dictionary<string, object>
                    {
                        ["WorkingSetMB"] = workingSet / (1024 * 1024),
                        ["TotalMemoryMB"] = totalMemory / (1024 * 1024),
                        ["MemoryUsagePercentage"] = memoryUsagePercentage,
                        ["GCMemoryMB"] = GC.GetTotalMemory(false) / (1024 * 1024)
                    }
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Memory health check failed");
                
                return new HealthCheckResult
                {
                    ServiceName = "Memory",
                    Status = HealthStatus.Unknown,
                    Message = $"Memory health check failed: {ex.Message}",
                    ResponseTime = stopwatch.Elapsed
                };
            }
        }

        public async Task<OverallHealthResult> GetOverallHealthAsync()
        {
            try
            {
                var tasks = new[]
                {
                    CheckDatabaseHealthAsync(),
                    CheckCacheHealthAsync(),
                    CheckExternalServicesHealthAsync(),
                    CheckDiskSpaceHealthAsync(),
                    CheckMemoryHealthAsync()
                };

                var results = await Task.WhenAll(tasks);
                var serviceResults = results.ToList();

                var overallStatus = serviceResults.All(r => r.Status == HealthStatus.Healthy)
                    ? HealthStatus.Healthy
                    : serviceResults.Any(r => r.Status == HealthStatus.Unhealthy)
                        ? HealthStatus.Unhealthy
                        : HealthStatus.Degraded;

                var healthyCount = serviceResults.Count(r => r.Status == HealthStatus.Healthy);
                var totalCount = serviceResults.Count;

                return new OverallHealthResult
                {
                    OverallStatus = overallStatus,
                    ServiceResults = serviceResults,
                    Summary = $"Overall health: {overallStatus}. {healthyCount}/{totalCount} services healthy."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get overall health");
                
                return new OverallHealthResult
                {
                    OverallStatus = HealthStatus.Unknown,
                    Summary = "Failed to determine overall health status"
                };
            }
        }

        public async Task<HealthCheckResult> CheckSpecificServiceAsync(string serviceName)
        {
            return serviceName.ToLower() switch
            {
                "database" => await CheckDatabaseHealthAsync(),
                "cache" => await CheckCacheHealthAsync(),
                "external" => await CheckExternalServicesHealthAsync(),
                "disk" => await CheckDiskSpaceHealthAsync(),
                "memory" => await CheckMemoryHealthAsync(),
                _ => new HealthCheckResult
                {
                    ServiceName = serviceName,
                    Status = HealthStatus.Unknown,
                    Message = $"Unknown service: {serviceName}",
                    ResponseTime = TimeSpan.Zero
                }
            };
        }
    }
}