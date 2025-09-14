# 部署指南（CI/CD 與雲端）

> 不含任何 secrets，僅描述流程與變數名稱；實際值請使用安全管道（GitHub Secrets / GCP Secret Manager）。

## 目標平台
- GitHub Actions（CI）
- Google Cloud Run（容器化 Web）+ Cloud SQL for SQL Server（資料庫）

## 主要流程
1. Build & Test（CI）
   - dotnet restore
   - dotnet build -c Release
   - dotnet test -c Release --no-build
2. Containerize（CD）
   - `Dockerfile` 基於 mcr.microsoft.com/dotnet/aspnet:8.0
   - build/push 至 Artifact Registry
3. Deploy to Cloud Run
   - 指定環境變數（見下）
   - 設定最小/最大實例、可用區域與自動擴縮
4. 資料庫連線（Cloud SQL for SQL Server）
   - 使用 Cloud SQL Auth Proxy 或 Cloud Run 內建連線（推薦）

## 必要環境變數（範例命名）
- ASPNETCORE_ENVIRONMENT=Production
- ConnectionStrings__DefaultConnection=（Cloud SQL 連線字串，不得直接寫入 repo）
- Logging__Level__Default=Information

## GitHub Actions（參考）
```yaml
name: ci
on: [push]
jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: "8.0.x" }
      - run: dotnet restore GameSpace_previous/GameSpace/GameSpace.csproj
      - run: dotnet build -c Release GameSpace_previous/GameSpace/GameSpace.csproj
      - run: dotnet test -c Release --no-build GameSpace_previous/GameSpace/GameSpace.csproj
```

## Cloud Run 參數建議
- 最小實例：0～1（依需求）
- 最大實例：10+（視流量）
- 要求逾時：60s
- 機敏變數：使用 Secret Manager 與 Cloud Run 變數綁定

## 疑難排解
- 500/資料庫連線：先以 `/healthz/db` 驗證連線字串是否正確
- 冷啟動延遲：提升最小實例或採用預熱策略
- 日誌：使用 Serilog 輸出至 Console，搭配 Cloud Logging 檢視
