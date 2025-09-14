#!/bin/bash
set -euo pipefail

echo "🚀 開始打包工作區..."

# 進入工作區目錄
cd /workspace

# 創建時間戳記
TIMESTAMP=$(date +%Y%m%d-%H%M%S)
ZIP_NAME="GameSpace-Complete-Workspace-$TIMESTAMP.zip"

echo "📦 創建 ZIP 檔案: $ZIP_NAME"

# 創建 ZIP 檔案
zip -r "$ZIP_NAME" . \
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

echo "✅ ZIP 檔案創建完成"
echo "📊 檔案大小: $(du -sh "$ZIP_NAME" | cut -f1)"
echo "📍 檔案位置: $(pwd)/$ZIP_NAME"
echo ""
echo "🌐 請將此檔案上傳到以下任一雲端服務："
echo "   - Google Drive: https://drive.google.com"
echo "   - Dropbox: https://dropbox.com"
echo "   - OneDrive: https://onedrive.live.com"
echo "   - GitHub Releases: https://github.com/[YOUR_REPO]/releases"
echo ""
echo "📋 上傳完成後，請提供下載連結"