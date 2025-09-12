# GameSpace 部署指南

## 概述

本指南提供了 GameSpace 應用程式的完整部署說明，包括 Docker、Kubernetes 和雲端部署選項。

## 系統需求

### 最低要求
- **CPU**: 2 核心
- **記憶體**: 4GB RAM
- **儲存空間**: 20GB 可用空間
- **作業系統**: Linux (Ubuntu 20.04+ 推薦) 或 Windows Server 2019+

### 推薦配置
- **CPU**: 4 核心
- **記憶體**: 8GB RAM
- **儲存空間**: 50GB 可用空間
- **作業系統**: Linux (Ubuntu 22.04 LTS)

## 依賴服務

- **SQL Server**: 2019 或更高版本
- **Redis**: 6.0 或更高版本
- **Nginx**: 1.18 或更高版本 (可選)

## 部署選項

### 1. Docker Compose 部署 (推薦用於開發和測試)

#### 步驟 1: 準備環境
```bash
# 克隆專案
git clone https://github.com/your-org/gamespace.git
cd gamespace

# 創建必要的目錄
mkdir -p logs ssl
```

#### 步驟 2: 配置環境變數
```bash
# 複製環境變數範本
cp .env.example .env

# 編輯環境變數
nano .env
```

#### 步驟 3: 啟動服務
```bash
# 啟動所有服務
docker-compose up -d

# 查看日誌
docker-compose logs -f

# 檢查服務狀態
docker-compose ps
```

#### 步驟 4: 初始化資料庫
```bash
# 等待資料庫啟動
sleep 30

# 執行資料庫初始化腳本
docker-compose exec gamespace-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -i /docker-entrypoint-initdb.d/database.sql
```

### 2. Kubernetes 部署 (推薦用於生產環境)

#### 步驟 1: 準備 Kubernetes 集群
```bash
# 確保 kubectl 已安裝並配置
kubectl version --client

# 創建命名空間
kubectl apply -f k8s/namespace.yaml
```

#### 步驟 2: 配置密鑰和配置
```bash
# 創建密鑰
kubectl apply -f k8s/secret.yaml

# 創建配置映射
kubectl apply -f k8s/configmap.yaml
```

#### 步驟 3: 部署應用程式
```bash
# 部署資料庫
kubectl apply -f k8s/database-deployment.yaml

# 部署應用程式
kubectl apply -f k8s/deployment.yaml

# 創建服務
kubectl apply -f k8s/service.yaml

# 配置入口
kubectl apply -f k8s/ingress.yaml
```

#### 步驟 4: 驗證部署
```bash
# 檢查 Pod 狀態
kubectl get pods -n gamespace

# 檢查服務狀態
kubectl get services -n gamespace

# 檢查入口狀態
kubectl get ingress -n gamespace
```

### 3. 雲端部署

#### Azure Container Instances (ACI)
```bash
# 創建資源組
az group create --name gamespace-rg --location eastus

# 創建容器實例
az container create \
  --resource-group gamespace-rg \
  --name gamespace-app \
  --image your-registry/gamespace:latest \
  --cpu 2 \
  --memory 4 \
  --ports 80 443 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Server=your-db-server;Database=GameSpace;User Id=sa;Password=YourPassword;"
```

#### Google Cloud Run
```bash
# 構建並推送映像
gcloud builds submit --tag gcr.io/your-project/gamespace

# 部署到 Cloud Run
gcloud run deploy gamespace \
  --image gcr.io/your-project/gamespace \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --memory 2Gi \
  --cpu 2
```

#### AWS ECS
```bash
# 創建 ECS 集群
aws ecs create-cluster --cluster-name gamespace-cluster

# 創建任務定義
aws ecs register-task-definition --cli-input-json file://ecs-task-definition.json

# 創建服務
aws ecs create-service \
  --cluster gamespace-cluster \
  --service-name gamespace-service \
  --task-definition gamespace:1 \
  --desired-count 2
```

## 環境配置

### 開發環境
```json
{
  "ASPNETCORE_ENVIRONMENT": "Development",
  "Logging__LogLevel__Default": "Debug",
  "ConnectionStrings__DefaultConnection": "Server=localhost;Database=GameSpace_Dev;Trusted_Connection=true;"
}
```

### 測試環境
```json
{
  "ASPNETCORE_ENVIRONMENT": "Staging",
  "Logging__LogLevel__Default": "Information",
  "ConnectionStrings__DefaultConnection": "Server=test-db;Database=GameSpace_Test;User Id=sa;Password=TestPassword;"
}
```

### 生產環境
```json
{
  "ASPNETCORE_ENVIRONMENT": "Production",
  "Logging__LogLevel__Default": "Warning",
  "ConnectionStrings__DefaultConnection": "Server=prod-db;Database=GameSpace;User Id=sa;Password=SecurePassword;",
  "Serilog__MinimumLevel__Override__Microsoft": "Warning"
}
```

## 監控和日誌

### Prometheus 監控
```bash
# 啟動 Prometheus
docker run -d \
  --name prometheus \
  -p 9090:9090 \
  -v $(pwd)/monitoring/prometheus.yml:/etc/prometheus/prometheus.yml \
  prom/prometheus
```

### Grafana 儀表板
```bash
# 啟動 Grafana
docker run -d \
  --name grafana \
  -p 3000:3000 \
  grafana/grafana
```

### ELK Stack 日誌
```bash
# 啟動 ELK Stack
docker-compose -f docker-compose.logging.yml up -d
```

## 安全配置

### SSL/TLS 證書
```bash
# 使用 Let's Encrypt
certbot certonly --standalone -d gamespace.com -d www.gamespace.com

# 或使用自簽名證書
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes
```

### 防火牆配置
```bash
# 開放必要端口
ufw allow 80
ufw allow 443
ufw allow 22
ufw enable
```

### 資料庫安全
```sql
-- 創建專用用戶
CREATE LOGIN gamespace_user WITH PASSWORD = 'StrongPassword123!';
CREATE USER gamespace_user FOR LOGIN gamespace_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON GameSpace TO gamespace_user;
```

## 備份和恢復

### 資料庫備份
```bash
# 創建備份
docker-compose exec gamespace-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Passw0rd' \
  -Q "BACKUP DATABASE GameSpace TO DISK = '/var/opt/mssql/backup/GameSpace.bak'"

# 恢復備份
docker-compose exec gamespace-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Passw0rd' \
  -Q "RESTORE DATABASE GameSpace FROM DISK = '/var/opt/mssql/backup/GameSpace.bak'"
```

### 應用程式備份
```bash
# 備份配置和日誌
tar -czf gamespace-backup-$(date +%Y%m%d).tar.gz \
  logs/ \
  ssl/ \
  .env \
  docker-compose.yml
```

## 故障排除

### 常見問題

#### 1. 資料庫連接失敗
```bash
# 檢查資料庫狀態
docker-compose logs gamespace-db

# 測試連接
docker-compose exec gamespace-app dotnet ef database update
```

#### 2. 記憶體不足
```bash
# 檢查記憶體使用
docker stats

# 增加記憶體限制
# 在 docker-compose.yml 中調整 memory 限制
```

#### 3. 端口衝突
```bash
# 檢查端口使用
netstat -tulpn | grep :80

# 修改端口映射
# 在 docker-compose.yml 中修改 ports 配置
```

### 日誌分析
```bash
# 查看應用程式日誌
docker-compose logs gamespace-app

# 查看資料庫日誌
docker-compose logs gamespace-db

# 查看 Nginx 日誌
docker-compose logs gamespace-nginx
```

## 性能優化

### 資料庫優化
```sql
-- 創建索引
CREATE INDEX IX_Users_UserAccount ON Users(UserAccount);
CREATE INDEX IX_Posts_CreatedAt ON Posts(CreatedAt);
CREATE INDEX IX_Orders_OrderDate ON OrderInfos(OrderDate);
```

### 應用程式優化
```json
{
  "Kestrel": {
    "Limits": {
      "MaxConcurrentConnections": 100,
      "MaxConcurrentUpgradedConnections": 100
    }
  },
  "Caching": {
    "DefaultExpiration": "00:30:00",
    "MaxSize": 1000
  }
}
```

## 維護和更新

### 應用程式更新
```bash
# 拉取最新代碼
git pull origin main

# 重新構建映像
docker-compose build

# 重啟服務
docker-compose up -d
```

### 資料庫遷移
```bash
# 執行遷移
docker-compose exec gamespace-app dotnet ef database update

# 回滾遷移
docker-compose exec gamespace-app dotnet ef database update PreviousMigration
```

## 支援和聯繫

- **文檔**: [GitHub Wiki](https://github.com/your-org/gamespace/wiki)
- **問題回報**: [GitHub Issues](https://github.com/your-org/gamespace/issues)
- **討論**: [GitHub Discussions](https://github.com/your-org/gamespace/discussions)
- **郵件**: support@gamespace.com

---

**最後更新**: 2024-12-19  
**版本**: 1.0.0