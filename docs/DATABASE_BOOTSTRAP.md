# Database Bootstrap（以 database.json 為權威）

## 建置流程
1. 建立資料庫 → 依 database.json 建表/約束 → 匯入種子
2. 不可使用 EF Migrations；若發現任何其他 schema 來源，視為 drift 並優先修復

## 種子規則
- 每張表「剛好」200 筆資料
- 符合 PK/FK/UNIQUE/CHECK/DEFAULT 與真實語料（繁體中文）
- 執行入口：scripts/bootstrap-db.(sh|ps1)
- CI 亦需驗證種子筆數

## 權威來源
- **唯一權威**：`My_Own_Source_Of_Data/database.json`
- **禁止**：EF Migrations、ad-hoc SQL scripts、其他 schema 定義
- **檢測**：任何非 database.json 的 schema 來源視為 drift

## 執行方式
```bash
# 手動執行
./scripts/bootstrap-db.sh

# 或 PowerShell
./scripts/bootstrap-db.ps1
```

## CI 驗證
- 驗證每張表恰好 200 筆資料
- 檢查所有約束條件
- 驗證繁體中文語料品質