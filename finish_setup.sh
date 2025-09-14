#!/bin/bash
# 完成資料庫規範化與 CI 強化腳本
# 執行時間：$(date)

set -euo pipefail

echo "=== 開始完成剩餘步驟 ==="

# 檢查當前狀態
echo "當前目錄：$(pwd)"
echo "當前分支：$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo 'main')"

# 1. 添加所有變更
echo "=== 添加所有變更到 Git ==="
git add -A

# 2. 檢查變更狀態
echo "=== 檢查變更狀態 ==="
git status --short

# 3. 提交變更
echo "=== 提交變更 ==="
git commit -m "chore(config): 完成配置容器化、CI 強化與文件更新

- 新增 .env.example 環境變數範本
- 強化 CI 工作流程，加入 SAFE RESET 檢查
- 更新文件樹狀結構
- 確保所有必要文件存在
- 封存 EF Migrations 至 docs/_archive/
- 文件化 database.json 建置流程"

# 4. 生成文件樹
echo "=== 生成文件樹 ==="
mkdir -p reports
if command -v tree >/dev/null 2>&1; then
  echo "使用 tree 命令生成文件樹"
  tree -a -I '.git|bin|obj|node_modules|artifacts' > reports/file_tree.txt
else
  echo "使用 find 命令生成文件樹"
  find . -not -path '*/.git/*' | sort > reports/file_tree.txt
fi

# 5. 提交文件樹更新
echo "=== 提交文件樹更新 ==="
git add reports/file_tree.txt
git commit -m "chore(report): 更新樹狀檔（$(date +%Y%m%d-%H%M%S)）"

# 6. 推送到遠端
echo "=== 推送到遠端 ==="
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo 'main')
echo "推送到分支：$CURRENT_BRANCH"
git push -u origin "$CURRENT_BRANCH"

echo "=== 所有步驟完成 ==="
echo "✅ 資料庫規範化完成"
echo "✅ CI 強化完成"
echo "✅ 文件更新完成"
echo "✅ 變更已推送到遠端"