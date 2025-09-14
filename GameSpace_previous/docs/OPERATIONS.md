# 營運手冊（只讀）

## 健康檢查
- /health（JSON）
- /healthz（純文字）
- /healthz/db（JSON：{ status: "ok" } 代表成功）

## 手動初始化（依 `database.json`）
- 僅允許以 SSMS 在 SQL Server 匯入 `My_Own_Source_Of_Data/database.json` 指令
- 禁用 EF Migrations；任何結構變更需更新 `database.json`

## 種子/資料維運
- 以管理端或手動入口觸發 idempotent seeding（批次 ≤ 1000）
- 失敗可重試；記錄繁中結構化日誌

## 監控與日誌
- Serilog：Console + 檔案（依環境）
- Cloud：Cloud Logging / Metrics（依部署）

## 變更管理
- 所有提交直推 `main`（本專案政策）
- 四大系統（Wallet/Sign-in/Pet/MiniGame）不可變；若發現缺陷，先文件化於 `docs/AUDIT.md`
