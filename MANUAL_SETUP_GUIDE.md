# 手動完成資料庫規範化與 CI 強化

## 📋 已完成的文件

以下文件已經創建，您可以在您的本地環境中檢查：

### 1. 環境變數範本
**文件：** `.env.example`
```bash
# 這裡只放名稱，不放實際值；本機請自行 export 或使用 .env 不入庫
CLIENT_SECRET=
APP_SECRET=
CONNECTION_STRING=
```

### 2. CI 工作流程
**文件：** `.github/workflows/ci.yml`
- 包含 SAFE RESET 檢查
- 建置與測試流程
- 覆蓋率收集
- 報表生成

### 3. 資料庫文件
**文件：** `docs/DATABASE_BOOTSTRAP.md`
- 以 database.json 為權威的建置流程
- 200 筆種子資料規則
- 禁止 EF Migrations 政策

### 4. 必要文件
- `docs/AUDIT.md` (空文件)
- `docs/WIP.md` (空文件)  
- `progress.json` (空 JSON 物件)

## 🔧 需要執行的命令

在您的本地環境中執行以下命令：

```bash
# 1. 確保在正確的目錄
cd /path/to/your/project

# 2. 檢查當前狀態
git status

# 3. 添加所有變更
git add -A

# 4. 提交變更
git commit -m "chore(config): 完成配置容器化、CI 強化與文件更新

- 新增 .env.example 環境變數範本
- 強化 CI 工作流程，加入 SAFE RESET 檢查
- 更新文件樹狀結構
- 確保所有必要文件存在
- 封存 EF Migrations 至 docs/_archive/
- 文件化 database.json 建置流程"

# 5. 生成文件樹
mkdir -p reports
if command -v tree >/dev/null 2>&1; then
  tree -a -I '.git|bin|obj|node_modules|artifacts' > reports/file_tree.txt
else
  find . -not -path '*/.git/*' | sort > reports/file_tree.txt
fi

# 6. 提交文件樹更新
git add reports/file_tree.txt
git commit -m "chore(report): 更新樹狀檔（$(date +%Y%m%d-%H%M%S)）"

# 7. 推送到遠端
git push -u origin $(git rev-parse --abbrev-ref HEAD)
```

## ✅ 驗證完成

執行完成後，您應該看到：

1. **Git 提交記錄**：包含所有變更的提交
2. **文件結構**：所有必要文件都已創建
3. **CI 工作流程**：`.github/workflows/ci.yml` 已就緒
4. **環境配置**：`.env.example` 已創建
5. **資料庫文件**：`docs/DATABASE_BOOTSTRAP.md` 已就緒

## 🚀 後續步驟

1. 檢查 CI 工作流程是否正常運行
2. 根據 `.env.example` 配置實際的環境變數
3. 確保 database.json 是唯一的 schema 來源
4. 驗證 200 筆種子資料規則

## 📞 需要協助

如果您在執行過程中遇到任何問題，請告訴我具體的錯誤訊息，我會協助您解決。