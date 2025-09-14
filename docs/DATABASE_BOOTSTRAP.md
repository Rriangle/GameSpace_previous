# Database Bootstrap（以 database.json 為權威）
- 建置流程：建立資料庫 → 依 database.json 建表/約束 → 匯入種子。
- 種子規則：每張表「剛好」200 筆資料，符合 PK/FK/UNIQUE/CHECK/DEFAULT 與真實語料（繁體中文）。
- 不可使用 EF Migrations；若發現任何其他 schema 來源，視為 drift 並優先修復。
- 執行入口：scripts/bootstrap-db.(sh|ps1)；CI 亦需驗證種子筆數。