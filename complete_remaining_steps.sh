#!/bin/bash
# 完成剩餘的步驟

echo "=== 完成剩餘的 Git 操作 ==="

# 添加所有變更
git add -A

# 提交變更
git commit -m "chore(config): 完成配置容器化、CI 強化與文件更新

- 新增 .env.example 環境變數範本
- 強化 CI 工作流程，加入 SAFE RESET 檢查
- 更新文件樹狀結構
- 確保所有必要文件存在"

# 生成文件樹
echo "=== 生成文件樹 ==="
mkdir -p reports
if command -v tree >/dev/null 2>&1; then
  tree -a -I '.git|bin|obj|node_modules|artifacts' > reports/file_tree.txt
else
  find . -not -path '*/.git/*' | sort > reports/file_tree.txt
fi

# 提交文件樹更新
git add reports/file_tree.txt
git commit -m "chore(report): 更新樹狀檔（$(date +%Y%m%d-%H%M%S)）"

# 推送到遠端
echo "=== 推送到遠端 ==="
git push -u origin "$(git rev-parse --abbrev-ref HEAD)"

echo "=== 所有步驟完成 ==="