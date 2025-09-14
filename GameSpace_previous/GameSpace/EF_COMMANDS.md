# Entity Framework 指令說明

## 預設專案與 Context

- **Identity Context**: `ApplicationDbContext` (位於 Data 專案)
- **Domain Context**: `GameSpaceDbContext` (位於 GameSpace.Data 專案)

## 常用指令

### Identity Migrations
```bash
# 新增 Identity migration
dotnet ef migrations add <Name> -c ApplicationDbContext -s GameSpace -p Data

# 更新資料庫 (Identity)
dotnet ef database update -c ApplicationDbContext -s GameSpace -p Data
```

### Domain Migrations
```bash
# 新增 Domain migration
dotnet ef migrations add <Name> -c GameSpaceDbContext -s GameSpace -p GameSpace.Data

# 更新資料庫 (Domain)
dotnet ef database update -c GameSpaceDbContext -s GameSpace -p GameSpace.Data
```

### 移除 Migration
```bash
# 移除最後一個 migration
dotnet ef migrations remove -c <ContextName> -s GameSpace -p <ProjectName>
```

## 注意事項

- 每次 migration 操作前請確認 Context 名稱
- Identity 相關使用 ApplicationDbContext
- 業務邏輯相關使用 GameSpaceDbContext
- 確保資料庫連線字串正確設定