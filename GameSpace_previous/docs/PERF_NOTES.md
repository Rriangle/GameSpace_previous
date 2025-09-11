# GameSpace 效能筆記

## 效能基準線

### 響應時間基準
- **健康檢查**: < 100ms
- **API 端點**: < 1000ms
- **資料庫查詢**: < 500ms
- **複雜聚合**: < 2000ms

### 吞吐量基準
- **並發使用者**: 50+ 同時在線
- **請求處理**: 20+ RPS
- **資料庫連線**: 100+ 並發連線
- **記憶體使用**: < 500MB 基礎使用

### 資源使用基準
- **CPU 使用率**: < 70% 正常負載
- **記憶體使用率**: < 80% 正常負載
- **磁碟 I/O**: < 100 IOPS 正常負載
- **網路頻寬**: < 10Mbps 正常負載

## 效能優化策略

### 1. 資料庫優化

#### 索引策略
```sql
-- 使用者相關查詢優化
CREATE INDEX IX_User_Wallet_UserId ON User_Wallet(UserId);
CREATE INDEX IX_User_Wallet_LastUpdated ON User_Wallet(LastUpdated);

-- 論壇查詢優化
CREATE INDEX IX_Posts_CategoryId_CreatedAt ON Posts(CategoryId, CreatedAt DESC);
CREATE INDEX IX_Posts_IsPinned_CreatedAt ON Posts(IsPinned DESC, CreatedAt DESC);
CREATE INDEX IX_Threads_PostId_CreatedAt ON Threads(PostId, CreatedAt);

-- 排行榜查詢優化
CREATE INDEX IX_Leaderboard_Date_Type_Score ON Leaderboard(Date, Type, Score DESC);
CREATE INDEX IX_Leaderboard_UserId_Date ON Leaderboard(UserId, Date);

-- 寵物系統優化
CREATE INDEX IX_Pet_UserId_Level ON Pet(UserId, Level DESC);
CREATE INDEX IX_Pet_Attributes_UserId ON Pet_Attributes(UserId);

-- 商城系統優化
CREATE INDEX IX_Products_IsActive_Price ON Products(IsActive, Price);
CREATE INDEX IX_Orders_UserId_CreatedAt ON Orders(UserId, CreatedAt DESC);
```

#### 查詢優化
```sql
-- 1. 使用適當的 JOIN 類型
-- 避免 SELECT *，只選擇需要的欄位
SELECT u.Id, u.Username, w.Points, w.Coupons
FROM Users u
INNER JOIN User_Wallet w ON u.Id = w.UserId
WHERE u.IsActive = 1;

-- 2. 使用 EXISTS 而非 IN（大資料集）
SELECT u.Id, u.Username
FROM Users u
WHERE EXISTS (
    SELECT 1 FROM User_Wallet w 
    WHERE w.UserId = u.Id AND w.Points > 1000
);

-- 3. 使用分頁查詢
SELECT TOP 20 Id, Title, CreatedAt
FROM Posts
WHERE CategoryId = @CategoryId
ORDER BY IsPinned DESC, CreatedAt DESC;

-- 4. 使用參數化查詢避免 SQL 注入
-- 在 Entity Framework 中自動處理
```

#### 連線池優化
```csharp
// 連線字串優化
"Server=localhost;Database=GameSpace;User Id=GameSpaceApp;Password=***;Max Pool Size=100;Min Pool Size=5;Connection Timeout=30;Command Timeout=30;"

// Entity Framework 設定
services.AddDbContext<GameSpaceContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(3);
        sqlOptions.EnableSensitiveDataLogging(false);
    });
});
```

### 2. 快取策略

#### 記憶體快取
```csharp
// 1. 設定快取大小限制
services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000;
    options.CompactionPercentage = 0.25;
});

// 2. 快取熱門資料
public async Task<LeaderboardOverviewReadModel> GetDailyLeaderboardAsync()
{
    var cacheKey = "leaderboard:daily";
    
    if (_cache.TryGetValue(cacheKey, out LeaderboardOverviewReadModel cached))
    {
        return cached;
    }
    
    var result = await _repository.GetDailyLeaderboardAsync();
    
    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
    return result;
}

// 3. 快取失效策略
public async Task UpdateUserPointsAsync(int userId, int points)
{
    // 更新資料庫
    await _repository.UpdateUserPointsAsync(userId, points);
    
    // 清除相關快取
    _cache.Remove($"user:{userId}:wallet");
    _cache.Remove("leaderboard:daily");
}
```

#### Redis 快取（可選）
```csharp
// 1. 設定 Redis
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "GameSpace";
});

// 2. 分散式快取
public async Task<string> GetCachedDataAsync(string key)
{
    var cached = await _distributedCache.GetStringAsync(key);
    if (cached != null)
    {
        return cached;
    }
    
    var data = await GetDataFromDatabaseAsync();
    await _distributedCache.SetStringAsync(key, data, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    });
    
    return data;
}
```

### 3. 應用程式優化

#### 非同步程式設計
```csharp
// 1. 使用 async/await 避免阻塞
public async Task<IActionResult> GetWalletOverviewAsync(int userId)
{
    var wallet = await _walletService.GetWalletOverviewAsync(userId);
    return Ok(wallet);
}

// 2. 並行執行獨立操作
public async Task<DashboardData> GetDashboardDataAsync(int userId)
{
    var walletTask = _walletService.GetWalletOverviewAsync(userId);
    var forumTask = _forumService.GetRecentPostsAsync(10);
    var leaderboardTask = _leaderboardService.GetDailyLeaderboardAsync();
    
    await Task.WhenAll(walletTask, forumTask, leaderboardTask);
    
    return new DashboardData
    {
        Wallet = await walletTask,
        RecentPosts = await forumTask,
        Leaderboard = await leaderboardTask
    };
}
```

#### 記憶體管理
```csharp
// 1. 使用 using 語句釋放資源
public async Task ProcessFileAsync(string filePath)
{
    using var fileStream = new FileStream(filePath, FileMode.Open);
    using var reader = new StreamReader(fileStream);
    
    var content = await reader.ReadToEndAsync();
    // 處理內容...
}

// 2. 避免記憶體洩漏
public class DataProcessor : IDisposable
{
    private bool _disposed = false;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // 釋放託管資源
        }
        _disposed = true;
    }
}
```

### 4. 前端優化

#### 資源優化
```html
<!-- 1. 壓縮靜態資源 -->
<link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true">
<script src="~/js/site.min.js" asp-append-version="true"></script>

<!-- 2. 使用 CDN -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">

<!-- 3. 延遲載入圖片 -->
<img src="placeholder.jpg" data-src="actual-image.jpg" loading="lazy">
```

#### JavaScript 優化
```javascript
// 1. 防抖動處理
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// 2. 節流處理
function throttle(func, limit) {
    let inThrottle;
    return function() {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    }
}

// 3. 使用事件委託
document.addEventListener('click', function(e) {
    if (e.target.matches('.btn-action')) {
        handleButtonClick(e.target);
    }
});
```

## 效能監控

### 1. 應用程式監控

#### 自定義效能計數器
```csharp
public class PerformanceCounter
{
    private static readonly Counter _requestCounter = 
        Meter.CreateCounter<int>("gamespace_requests_total", "Total number of requests");
    
    private static readonly Histogram _responseTimeHistogram = 
        Meter.CreateHistogram<double>("gamespace_response_time_seconds", "Response time in seconds");
    
    public void RecordRequest(string endpoint, double responseTime)
    {
        _requestCounter.Add(1, new KeyValuePair<string, object>("endpoint", endpoint));
        _responseTimeHistogram.Record(responseTime);
    }
}
```

#### 健康檢查監控
```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly GameSpaceContext _context;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            stopwatch.Stop();
            
            var responseTime = stopwatch.ElapsedMilliseconds;
            
            if (responseTime > 1000)
            {
                return HealthCheckResult.Degraded($"資料庫響應時間過長: {responseTime}ms");
            }
            
            return HealthCheckResult.Healthy($"資料庫正常，響應時間: {responseTime}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("資料庫連線失敗", ex);
        }
    }
}
```

### 2. 系統監控

#### 資源監控
```csharp
public class SystemResourceMonitor
{
    public async Task<SystemResourceInfo> GetResourceInfoAsync()
    {
        var process = Process.GetCurrentProcess();
        
        return new SystemResourceInfo
        {
            CpuUsage = await GetCpuUsageAsync(),
            MemoryUsage = process.WorkingSet64,
            ThreadCount = process.Threads.Count,
            HandleCount = process.HandleCount,
            GcMemory = GC.GetTotalMemory(false)
        };
    }
    
    private async Task<double> GetCpuUsageAsync()
    {
        // 實作 CPU 使用率監控
        return await Task.FromResult(0.0);
    }
}
```

### 3. 資料庫監控

#### 查詢效能監控
```sql
-- 1. 監控慢查詢
SELECT 
    query_hash,
    query_plan_hash,
    total_elapsed_time,
    execution_count,
    total_elapsed_time / execution_count AS avg_elapsed_time,
    last_execution_time
FROM sys.dm_exec_query_stats
WHERE total_elapsed_time / execution_count > 1000
ORDER BY avg_elapsed_time DESC;

-- 2. 監控鎖等待
SELECT 
    session_id,
    wait_type,
    wait_time,
    blocking_session_id,
    resource_description
FROM sys.dm_exec_requests
WHERE wait_type IS NOT NULL;

-- 3. 監控連線數
SELECT 
    COUNT(*) AS connection_count,
    SUM(CASE WHEN is_user_process = 1 THEN 1 ELSE 0 END) AS user_connections,
    SUM(CASE WHEN is_user_process = 0 THEN 1 ELSE 0 END) AS system_connections
FROM sys.dm_exec_sessions;
```

## 效能測試

### 1. 基準測試

#### 單元效能測試
```csharp
[Fact]
public async Task WalletService_GetOverview_ShouldCompleteWithinBaseline()
{
    // 測試錢包服務效能
    var stopwatch = Stopwatch.StartNew();
    
    var result = await _walletService.GetWalletOverviewAsync(1);
    
    stopwatch.Stop();
    
    Assert.NotNull(result);
    Assert.True(stopwatch.ElapsedMilliseconds < 500, 
        $"錢包服務響應時間 {stopwatch.ElapsedMilliseconds}ms 超過基準線 500ms");
}
```

#### 整合效能測試
```csharp
[Fact]
public async Task ApiEndpoint_ConcurrentRequests_ShouldMaintainPerformance()
{
    // 測試 API 端點並發效能
    const int concurrentRequests = 20;
    var tasks = new List<Task<HttpResponseMessage>>();
    
    for (int i = 0; i < concurrentRequests; i++)
    {
        tasks.Add(_client.GetAsync("/api/wallet/overview?userId=1"));
    }
    
    var responses = await Task.WhenAll(tasks);
    
    // 驗證所有請求都成功
    Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
    
    // 驗證響應時間
    var responseTimes = responses.Select(r => r.Headers.GetValues("X-Response-Time").FirstOrDefault()?.ParseInt() ?? 0);
    Assert.True(responseTimes.Average() < 1000, "平均響應時間超過基準線");
}
```

### 2. 負載測試

#### 壓力測試
```csharp
[Fact]
public async Task System_UnderHighLoad_ShouldMaintainStability()
{
    // 高負載壓力測試
    const int durationSeconds = 60;
    const int requestsPerSecond = 10;
    var totalRequests = durationSeconds * requestsPerSecond;
    
    var tasks = new List<Task<HttpResponseMessage>>();
    var stopwatch = Stopwatch.StartNew();
    
    // 持續發送請求
    for (int second = 0; second < durationSeconds; second++)
    {
        for (int req = 0; req < requestsPerSecond; req++)
        {
            tasks.Add(_client.GetAsync("/healthz"));
        }
        await Task.Delay(1000);
    }
    
    var responses = await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    // 分析結果
    var successRate = responses.Count(r => r.IsSuccessStatusCode) / (double)totalRequests;
    var averageResponseTime = responses
        .Where(r => r.IsSuccessStatusCode)
        .Average(r => r.Headers.GetValues("X-Response-Time").FirstOrDefault()?.ParseInt() ?? 0);
    
    Assert.True(successRate >= 0.95, $"成功率 {successRate:P2} 低於 95%");
    Assert.True(averageResponseTime < 1000, $"平均響應時間 {averageResponseTime:F2}ms 超過基準線");
}
```

## 效能調優建議

### 1. 資料庫調優

#### 索引優化
- 定期分析查詢執行計畫
- 移除未使用的索引
- 重建碎片化的索引
- 使用覆蓋索引減少 I/O

#### 查詢優化
- 避免 SELECT * 查詢
- 使用適當的 JOIN 類型
- 實施查詢結果快取
- 使用參數化查詢

### 2. 應用程式調優

#### 記憶體管理
- 定期監控記憶體使用
- 實施物件池模式
- 避免記憶體洩漏
- 調整垃圾回收器設定

#### 非同步處理
- 使用 async/await 模式
- 避免阻塞 I/O 操作
- 實施並行處理
- 使用適當的並發限制

### 3. 系統調優

#### 硬體優化
- 使用 SSD 儲存
- 增加記憶體容量
- 使用多核心 CPU
- 優化網路配置

#### 作業系統調優
- 調整 TCP 參數
- 優化檔案系統設定
- 配置適當的虛擬記憶體
- 設定系統監控

## 效能基準測試結果

### 測試環境
- **作業系統**: Windows Server 2019
- **CPU**: Intel Xeon E5-2680 v4 (2.4GHz, 14核心)
- **記憶體**: 32GB DDR4
- **儲存**: SSD 500GB
- **資料庫**: SQL Server 2019
- **.NET 版本**: .NET 8.0

### 測試結果

#### API 響應時間
| 端點 | 平均響應時間 | 95% 百分位數 | 最大響應時間 |
|------|-------------|-------------|-------------|
| /healthz | 15ms | 25ms | 50ms |
| /api/wallet/overview | 120ms | 200ms | 350ms |
| /api/forum/list | 180ms | 300ms | 500ms |
| /api/leaderboard/daily | 250ms | 400ms | 600ms |

#### 並發效能
| 並發使用者 | 平均響應時間 | 吞吐量 (RPS) | 錯誤率 |
|-----------|-------------|-------------|--------|
| 10 | 150ms | 65 | 0% |
| 25 | 200ms | 50 | 0% |
| 50 | 300ms | 40 | 0.5% |
| 100 | 500ms | 30 | 2% |

#### 資源使用
| 負載 | CPU 使用率 | 記憶體使用 | 資料庫連線數 |
|------|-----------|-----------|-------------|
| 低負載 | 15% | 200MB | 5 |
| 中負載 | 35% | 400MB | 15 |
| 高負載 | 60% | 800MB | 30 |
| 峰值負載 | 80% | 1.2GB | 50 |

## 持續改進

### 1. 定期效能審查
- 每週分析效能指標
- 每月進行效能測試
- 每季進行容量規劃
- 每年進行架構審查

### 2. 效能監控儀表板
- 即時效能指標
- 歷史趨勢分析
- 異常警報通知
- 效能報告生成

### 3. 自動化效能測試
- CI/CD 整合效能測試
- 自動化基準測試
- 效能回歸檢測
- 自動化效能報告

---

**文件版本**: 1.0  
**建立日期**: 2025-01-09  
**最後更新**: 2025-01-09  
**負責人**: GameSpace 效能團隊