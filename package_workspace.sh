#!/bin/bash
set -euo pipefail

# 設定時間戳記
TS="$(date +%Y%m%d-%H%M%S)"
ZIP_NAME="workspace-complete-$TS.zip"
ZIP_PATH="/workspace/artifacts/$ZIP_NAME"

# 確保 artifacts 目錄存在
mkdir -p /workspace/artifacts

# 創建 ZIP 檔案
cd /workspace
zip -r "$ZIP_PATH" . \
    -x "*.pyc" \
    -x "__pycache__/*" \
    -x "node_modules/*" \
    -x ".pytest_cache/*" \
    -x "*.pyo" \
    -x "*.pyd" \
    -x ".DS_Store" \
    -x "Thumbs.db" \
    -x "*.log" \
    -x "*.tmp" \
    -x "artifacts/workspace-*.zip" \
    -x "artifacts/project-*.zip"

echo "ZIP 檔案創建完成: $ZIP_PATH"
ls -lh "$ZIP_PATH"