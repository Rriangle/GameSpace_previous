# GameSpace 運維手冊

## 系統概覽

### 技術架構
- **後端**: ASP.NET Core 8.0 + Entity Framework Core
- **資料庫**: SQL Server 2022
- **快取**: Memory Cache + Redis (可選)
- **監控**: Serilog + 自定義效能監控
- **部署**: IIS / Kestrel

### 系統組件
- **API 層**: RESTful API + Swagger 文件
- **業務層**: 錢包、論壇、排行榜、寵物、商城系統
- **資料層**: 唯讀/寫入儲存庫 + 交易管理
- **監控層**: 健康檢查 + 效能監控 + 日誌聚合

## 部署指南

### 1. 環境準備

#### 系統需求
- Windows Server 2019+ 或 Linux (Ubuntu 20.04+)
- .NET 8.0 Runtime
- SQL Server 2019+ (建議 2022)
- IIS 10+ (Windows) 或 Nginx (Linux)
- 最少 4GB RAM, 建議 8GB+
- 最少 50GB 磁碟空間

#### 網路需求
- 端口 80 (HTTP)
- 端口 443 (HTTPS)
- 端口 1433 (SQL Server)
- 端口 6379 (Redis, 可選)

### 2. 資料庫部署

#### 建立資料庫
```sql
-- 1. 建立資料庫
CREATE DATABASE GameSpace;
GO

-- 2. 設定資料庫選項
ALTER DATABASE GameSpace SET RECOVERY SIMPLE;
ALTER DATABASE GameSpace SET AUTO_SHRINK OFF;
ALTER DATABASE GameSpace SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE GameSpace SET AUTO_UPDATE_STATISTICS ON;
GO

-- 3. 建立登入和使用者
CREATE LOGIN [GameSpaceApp] WITH PASSWORD = 'YourStrongPassword123!';
USE GameSpace;
CREATE USER [GameSpaceApp] FOR LOGIN [GameSpaceApp];
ALTER ROLE db_owner ADD MEMBER [GameSpaceApp];
GO
```

#### 執行資料庫架構
```bash
# 執行不可變的資料庫架構
sqlcmd -S localhost -d GameSpace -i database_unchangeable.txt
```

#### 執行種子資料
```bash
# 執行種子資料（冪等性）
sqlcmd -S localhost -d GameSpace -i seed_hyper_real_independent_v2_chunked.sql
```

### 3. 應用程式部署

#### 發佈應用程式
```bash
# 發佈到 Release 模式
dotnet publish -c Release -o ./publish

# 或使用 MSBuild
msbuild GameSpace.sln /p:Configuration=Release /p:PublishProfile=FolderProfile
```

#### 設定 IIS (Windows)
```xml
<!-- web.config -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\GameSpace.Web.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

#### 設定 Nginx (Linux)
```nginx
server {
    listen 80;
    server_name your-domain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 4. 環境變數設定

#### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GameSpace;User Id=GameSpaceApp;Password=YourStrongPassword123!;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/gamespace-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  },
  "PerformanceMonitoring": {
    "Enabled": true,
    "SampleRate": 1.0,
    "SlowQueryThresholdMs": 1000
  }
}
```

## 監控與維護

### 1. 健康檢查

#### 基本健康檢查
```bash
# 檢查應用程式狀態
curl https://your-domain.com/healthz

# 檢查詳細健康狀況
curl https://your-domain.com/api/health/detailed
```

#### 資料庫健康檢查
```bash
# 檢查資料庫連線
curl https://your-domain.com/api/health/database

# 檢查資料庫統計
curl https://your-domain.com/api/performance/database-stats
```

### 2. 效能監控

#### 即時監控
- 訪問管理後台: `https://your-domain.com/Admin/System`
- 查看效能儀表板
- 監控系統資源使用率
- 查看效能瓶頸和優化建議

#### 日誌監控
```bash
# 查看應用程式日誌
tail -f logs/gamespace-*.log

# 查看錯誤日誌
grep "ERROR" logs/gamespace-*.log

# 查看效能日誌
grep "Performance" logs/gamespace-*.log
```

### 3. 資料庫維護

#### 定期維護任務
```sql
-- 1. 更新統計資訊
EXEC sp_updatestats;

-- 2. 重建索引（每週）
ALTER INDEX ALL ON [TableName] REBUILD;

-- 3. 檢查資料庫完整性
DBCC CHECKDB('GameSpace') WITH NO_INFOMSGS;

-- 4. 清理舊日誌
EXEC sp_cycle_errorlog;
```

#### 備份策略
```sql
-- 完整備份（每日）
BACKUP DATABASE GameSpace 
TO DISK = 'C:\Backups\GameSpace_Full.bak'
WITH FORMAT, INIT, COMPRESSION;

-- 差異備份（每6小時）
BACKUP DATABASE GameSpace 
TO DISK = 'C:\Backups\GameSpace_Diff.bak'
WITH DIFFERENTIAL, COMPRESSION;

-- 交易日誌備份（每小時）
BACKUP LOG GameSpace 
TO DISK = 'C:\Backups\GameSpace_Log.trn'
WITH COMPRESSION;
```

### 4. 效能優化

#### 資料庫優化
```sql
-- 1. 建立索引
CREATE INDEX IX_User_Wallet_UserId ON User_Wallet(UserId);
CREATE INDEX IX_Posts_CreatedAt ON Posts(CreatedAt);
CREATE INDEX IX_Leaderboard_Date_Score ON Leaderboard(Date, Score DESC);

-- 2. 查詢優化
-- 使用執行計畫分析慢查詢
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- 3. 連線池設定
-- 在連線字串中設定:
-- Max Pool Size=100;Min Pool Size=5;Connection Timeout=30;
```

#### 應用程式優化
```csharp
// 1. 快取設定
services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000;
    options.CompactionPercentage = 0.25;
});

// 2. 連線池設定
services.AddDbContext<GameSpaceContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(3);
    });
});

// 3. 效能監控
services.AddScoped<PerformanceOptimizerService>();
```

## 故障排除

### 1. 常見問題

#### 應用程式無法啟動
```bash
# 檢查 .NET Runtime
dotnet --version

# 檢查應用程式日誌
cat logs/gamespace-*.log | grep "ERROR"

# 檢查資料庫連線
sqlcmd -S localhost -d GameSpace -Q "SELECT 1"
```

#### 資料庫連線問題
```bash
# 檢查 SQL Server 服務
systemctl status mssql-server  # Linux
Get-Service MSSQLSERVER        # Windows

# 檢查防火牆
netstat -an | grep 1433

# 檢查連線字串
# 確保密碼正確，網路可達
```

#### 效能問題
```bash
# 檢查系統資源
top                    # Linux
Get-Process | Sort-Object CPU -Descending  # Windows

# 檢查資料庫效能
sqlcmd -S localhost -d GameSpace -Q "SELECT * FROM sys.dm_exec_requests WHERE status = 'running'"

# 檢查慢查詢
sqlcmd -S localhost -d GameSpace -Q "SELECT * FROM sys.dm_exec_query_stats ORDER BY total_elapsed_time DESC"
```

### 2. 緊急處理

#### 系統當機
1. **立即重啟應用程式**
   ```bash
   # Windows
   iisreset
   
   # Linux
   systemctl restart gamespace
   ```

2. **檢查系統資源**
   ```bash
   # 檢查記憶體使用
   free -h
   
   # 檢查磁碟空間
   df -h
   ```

3. **檢查錯誤日誌**
   ```bash
   tail -f logs/gamespace-*.log
   ```

#### 資料庫問題
1. **檢查資料庫狀態**
   ```sql
   SELECT name, state_desc FROM sys.databases WHERE name = 'GameSpace';
   ```

2. **檢查資料庫完整性**
   ```sql
   DBCC CHECKDB('GameSpace') WITH NO_INFOMSGS;
   ```

3. **恢復資料庫**
   ```sql
   RESTORE DATABASE GameSpace FROM DISK = 'C:\Backups\GameSpace_Full.bak' WITH REPLACE;
   ```

### 3. 監控告警

#### 設定監控告警
```bash
# 1. 設定健康檢查監控
# 使用監控工具（如 Nagios, Zabbix）監控 /healthz 端點

# 2. 設定效能監控
# 監控響應時間、吞吐量、錯誤率

# 3. 設定資源監控
# 監控 CPU、記憶體、磁碟使用率

# 4. 設定資料庫監控
# 監控連線數、查詢時間、鎖等待
```

#### 告警規則
- **響應時間 > 2 秒**: 警告
- **響應時間 > 5 秒**: 嚴重
- **錯誤率 > 5%**: 警告
- **錯誤率 > 10%**: 嚴重
- **CPU 使用率 > 80%**: 警告
- **記憶體使用率 > 90%**: 嚴重
- **磁碟使用率 > 85%**: 警告

## 安全維護

### 1. 定期安全檢查

#### 應用程式安全
```bash
# 1. 檢查依賴套件漏洞
dotnet list package --vulnerable

# 2. 更新安全套件
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# 3. 檢查配置檔案權限
ls -la appsettings*.json
```

#### 資料庫安全
```sql
-- 1. 檢查使用者權限
SELECT name, type_desc FROM sys.database_principals;

-- 2. 檢查角色權限
SELECT r.name, p.permission_name 
FROM sys.database_role_members rm
JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
JOIN sys.database_permissions p ON r.principal_id = p.grantee_principal_id;

-- 3. 檢查敏感資料
SELECT TABLE_NAME, COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME LIKE '%password%' OR COLUMN_NAME LIKE '%secret%';
```

### 2. 備份與恢復

#### 備份策略
- **完整備份**: 每日凌晨 2:00
- **差異備份**: 每 6 小時
- **交易日誌備份**: 每小時
- **備份保留**: 30 天

#### 恢復程序
1. **完整恢復**
   ```sql
   RESTORE DATABASE GameSpace FROM DISK = 'C:\Backups\GameSpace_Full.bak' WITH REPLACE;
   ```

2. **時間點恢復**
   ```sql
   RESTORE DATABASE GameSpace FROM DISK = 'C:\Backups\GameSpace_Full.bak' WITH NORECOVERY;
   RESTORE LOG GameSpace FROM DISK = 'C:\Backups\GameSpace_Log.trn' WITH STOPAT = '2025-01-09 10:00:00';
   ```

## 擴展規劃

### 1. 水平擴展

#### 負載平衡
- 使用 Nginx 或 HAProxy 進行負載平衡
- 配置多個應用程式實例
- 使用 Redis 進行會話共享

#### 資料庫分片
- 按使用者 ID 進行水平分片
- 使用讀寫分離
- 配置資料庫複製

### 2. 垂直擴展

#### 硬體升級
- 增加 CPU 核心數
- 增加記憶體容量
- 使用 SSD 儲存

#### 軟體優化
- 調整 .NET 垃圾回收器設定
- 優化資料庫查詢
- 實施快取策略

## 聯絡資訊

### 開發團隊
- **專案負責人**: [姓名] - [email]
- **技術主管**: [姓名] - [email]
- **資料庫管理員**: [姓名] - [email]

### 運維團隊
- **系統管理員**: [姓名] - [email]
- **網路管理員**: [姓名] - [email]
- **安全管理員**: [姓名] - [email]

### 緊急聯絡
- **24/7 支援熱線**: [電話]
- **緊急郵件**: [email]
- **內部通訊**: [頻道]

---

**文件版本**: 1.0  
**建立日期**: 2025-01-09  
**最後更新**: 2025-01-09  
**負責人**: GameSpace 運維團隊