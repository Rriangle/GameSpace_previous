# GameSpace 系統優化指南

## 概述

本指南提供 GameSpace 系統的完整優化策略和實作方法，包括記憶體優化、資料庫優化、API 效能優化和系統監控。

## 優化架構

### 1. 記憶體優化

#### 記憶體監控服務
- **服務**: `MemoryOptimizerService`
- **功能**: 實時監控記憶體使用情況，檢測記憶體洩漏
- **端點**: `/api/SystemOptimization/memory`

#### 記憶體優化策略
```csharp
// 1. 定期監控記憶體使用
var memoryInfo = memoryOptimizer.GetMemoryInfo();

// 2. 檢測記憶體洩漏
var leakInfo = memoryOptimizer.CheckMemoryLeak();

// 3. 強制垃圾回收（必要時）
var gcResult = await memoryOptimizer.ForceGarbageCollectionAsync();
```

#### 記憶體優化建議
- 監控託管記憶體使用量，建議 < 1GB
- 定期檢查記憶體洩漏模式
- 使用物件池模式重用大型物件
- 避免長時間持有大型物件參考

### 2. 資料庫優化

#### 連線池優化服務
- **服務**: `DatabaseConnectionOptimizer`
- **功能**: 監控連線池使用，分析查詢效能
- **端點**: `/api/SystemOptimization/database/recommendations`

#### 資料庫優化策略
```csharp
// 1. 監控連線池狀態
var poolInfo = await databaseOptimizer.GetConnectionPoolInfoAsync();

// 2. 分析查詢執行計畫
var executionPlan = await databaseOptimizer.AnalyzeQueryExecutionPlanAsync(query, parameters);

// 3. 檢查索引使用情況
var indexUsage = await databaseOptimizer.CheckIndexUsageAsync();

// 4. 獲取優化建議
var recommendations = await databaseOptimizer.GenerateOptimizationRecommendationsAsync();
```

#### 資料庫優化建議
- 連線池使用率建議 < 80%
- 定期更新統計資訊
- 移除未使用的索引
- 使用適當的查詢提示

### 3. API 效能優化

#### 響應時間監控中介軟體
- **中介軟體**: `ResponseTimeOptimizationMiddleware`
- **功能**: 監控 API 響應時間，識別慢端點
- **端點**: `/api/SystemOptimization/endpoints`

#### API 優化策略
```csharp
// 1. 監控所有端點響應時間
app.UseMiddleware<ResponseTimeOptimizationMiddleware>();

// 2. 獲取慢端點統計
var slowEndpoints = responseTimeMiddleware.GetSlowEndpoints();

// 3. 獲取高錯誤率端點
var errorEndpoints = responseTimeMiddleware.GetHighErrorRateEndpoints();
```

#### API 優化建議
- 響應時間建議 < 500ms
- 錯誤率建議 < 5%
- 使用非同步處理
- 實施適當的快取策略

### 4. 查詢優化

#### 查詢分析功能
- **端點**: `/api/SystemOptimization/database/optimize-query`
- **功能**: 分析查詢執行計畫，提供優化建議

#### 查詢優化策略
```sql
-- 1. 使用適當的索引
CREATE INDEX IX_User_Wallet_UserId ON User_Wallet(UserId);

-- 2. 避免 SELECT * 查詢
SELECT Id, Username, Points FROM Users WHERE IsActive = 1;

-- 3. 使用參數化查詢
SELECT * FROM Posts WHERE CategoryId = @CategoryId;

-- 4. 使用分頁查詢
SELECT TOP 20 * FROM Posts ORDER BY CreatedAt DESC;
```

#### 查詢優化建議
- 使用覆蓋索引減少 I/O
- 避免函數在 WHERE 子句中使用
- 使用 EXISTS 而非 IN（大資料集）
- 定期分析查詢執行計畫

## 監控和警報

### 1. 系統健康監控

#### 健康檢查端點
- **基本健康檢查**: `/healthz`
- **詳細健康檢查**: `/health`
- **系統概覽**: `/api/HealthDashboard/overview`

#### 監控指標
- 記憶體使用量
- CPU 使用率
- 資料庫連線數
- API 響應時間
- 錯誤率

### 2. 效能監控

#### 效能指標
- 平均響應時間
- 95% 響應時間
- 吞吐量 (RPS)
- 並發使用者數
- 錯誤率

#### 警報設定
```yaml
# 記憶體警報
memory_warning: 512MB
memory_critical: 1024MB

# 效能警報
response_time_warning: 500ms
response_time_critical: 1000ms
error_rate_warning: 1%
error_rate_critical: 5%

# 資料庫警報
connection_pool_warning: 80%
query_timeout_warning: 30s
```

### 3. 自動化優化

#### 自動垃圾回收
- 當記憶體使用量超過閾值時自動觸發
- 監控垃圾回收頻率和效果
- 記錄記憶體釋放情況

#### 自動查詢優化
- 定期分析慢查詢
- 自動生成索引建議
- 監控統計資訊過期情況

## 優化工具和命令

### 1. 記憶體優化命令

```bash
# 檢查記憶體使用情況
curl http://localhost:5000/api/SystemOptimization/memory

# 強制垃圾回收
curl -X POST http://localhost:5000/api/SystemOptimization/memory/gc

# 檢查記憶體洩漏
curl http://localhost:5000/api/SystemOptimization/memory | jq '.LeakDetection'
```

### 2. 資料庫優化命令

```bash
# 獲取連線池狀態
curl http://localhost:5000/api/SystemOptimization/database/connection-pool

# 分析查詢執行計畫
curl -X POST http://localhost:5000/api/SystemOptimization/database/optimize-query \
  -H "Content-Type: application/json" \
  -d '{"query": "SELECT * FROM Users WHERE Id = @id", "parameters": {"id": 1}}'

# 獲取優化建議
curl http://localhost:5000/api/SystemOptimization/database/recommendations
```

### 3. API 效能監控命令

```bash
# 獲取所有端點統計
curl http://localhost:5000/api/SystemOptimization/endpoints

# 獲取慢端點
curl http://localhost:5000/api/SystemOptimization/endpoints/slow

# 獲取高錯誤率端點
curl http://localhost:5000/api/SystemOptimization/endpoints/errors

# 重置統計資料
curl -X POST http://localhost:5000/api/SystemOptimization/reset-stats
```

## 效能基準測試

### 1. 負載測試

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

### 2. 記憶體測試

```csharp
[Fact]
public async Task MemoryOptimizer_UnderMemoryPressure_ShouldHandleGracefully()
{
    // 記憶體壓力測試
    var memoryOptimizer = new MemoryOptimizerService(logger);
    
    // 創建大量物件
    var objects = new List<object>();
    for (int i = 0; i < 10000; i++)
    {
        objects.Add(new { Id = i, Data = new string('x', 1000) });
    }
    
    // 檢查記憶體使用情況
    var memoryInfo = memoryOptimizer.GetMemoryInfo();
    var leakInfo = memoryOptimizer.CheckMemoryLeak();
    
    // 執行垃圾回收
    var gcResult = await memoryOptimizer.ForceGarbageCollectionAsync();
    
    Assert.True(gcResult.MemoryFreed >= 0);
    Assert.True(gcResult.AfterMemoryMB < memoryInfo.ManagedMemory / (1024.0 * 1024.0));
}
```

## 最佳實踐

### 1. 記憶體管理
- 定期監控記憶體使用情況
- 使用 `using` 語句確保資源釋放
- 避免長時間持有大型物件參考
- 實施物件池模式

### 2. 資料庫優化
- 使用適當的索引策略
- 定期更新統計資訊
- 監控連線池使用情況
- 使用參數化查詢

### 3. API 效能
- 實施響應時間監控
- 使用非同步處理
- 實施適當的快取策略
- 監控錯誤率

### 4. 系統監控
- 建立完整的監控體系
- 設定適當的警報閾值
- 定期分析效能資料
- 建立自動化優化機制

## 故障排除

### 1. 記憶體問題
- 檢查記憶體洩漏
- 分析垃圾回收頻率
- 檢查大型物件使用情況
- 調整垃圾回收器設定

### 2. 資料庫問題
- 檢查連線池狀態
- 分析慢查詢
- 檢查索引使用情況
- 更新統計資訊

### 3. API 效能問題
- 檢查響應時間統計
- 分析慢端點
- 檢查錯誤率
- 優化查詢和處理邏輯

## 結論

GameSpace 系統優化指南提供了完整的優化策略和實作方法，包括記憶體優化、資料庫優化、API 效能優化和系統監控。通過遵循這些最佳實踐，可以確保系統在生產環境中保持高效能和穩定性。

---

**最後更新**: 2025-01-09
**版本**: 1.0.0
**維護者**: GameSpace 開發團隊