# 工作區打包與上傳完整指南

## 📦 打包步驟

### 1. 創建完整工作區 ZIP 檔案

在您的 VM 中執行以下命令：

```bash
# 進入工作區根目錄
cd /workspace

# 創建時間戳記
TIMESTAMP=$(date +%Y%m%d-%H%M%S)

# 創建 ZIP 檔案（排除不必要的檔案）
zip -r "GameSpace-Complete-Workspace-$TIMESTAMP.zip" . \
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

echo "ZIP 檔案創建完成: GameSpace-Complete-Workspace-$TIMESTAMP.zip"
ls -lh "GameSpace-Complete-Workspace-$TIMESTAMP.zip"
```

### 2. 檢查檔案大小
```bash
du -sh GameSpace-Complete-Workspace-*.zip
```

## ☁️ 上傳到雲端儲存服務

### 選項 1: Google Drive
1. 開啟 [Google Drive](https://drive.google.com)
2. 點擊「新增」→「檔案上傳」
3. 選擇 `GameSpace-Complete-Workspace-*.zip` 檔案
4. 上傳完成後，右鍵點擊檔案→「取得連結」
5. 設定為「知道連結的任何人都可以檢視」

### 選項 2: Dropbox
1. 開啟 [Dropbox](https://dropbox.com)
2. 拖拽 ZIP 檔案到 Dropbox 網頁
3. 右鍵點擊檔案→「分享」→「建立連結」
4. 複製分享連結

### 選項 3: OneDrive
1. 開啟 [OneDrive](https://onedrive.live.com)
2. 上傳 ZIP 檔案
3. 右鍵點擊檔案→「分享」→「複製連結」

### 選項 4: GitHub Releases
```bash
# 在專案根目錄執行
git tag "v1.0-workspace-$TIMESTAMP"
git push origin "v1.0-workspace-$TIMESTAMP"

# 然後在 GitHub 網頁上創建 Release 並上傳 ZIP 檔案
```

## 📋 包含的內容

### 完整專案結構
- `GameSpace_previous/` - 主要 ASP.NET Core 專案
- `docs/` - 所有文件檔案
- `reports/` - 報表和樹狀結構
- `.git/` - 完整版本控制歷史

### 規範修復成果
- ✅ 規範快照：`docs/SPEC_SNAPSHOT_20250914-123000.txt`
- ✅ 資料庫建置文件：`docs/DATABASE_BOOTSTRAP.md`
- ✅ 命名統一：`OtherProductDetail`, `PlayerMarketProductImg`
- ✅ 環境變數容器化：`.env.example`
- ✅ CI/CD 強化：`.github/workflows/ci.yml`
- ✅ 樹狀檔更新：`reports/file_tree.txt`

### 重要檔案
- `GameSpace_previous/GameSpace/GameSpace.sln` - Visual Studio 解決方案
- `GameSpace_previous/GameSpace/GameSpace.csproj` - 主專案檔案
- `GameSpace_previous/My_Own_Source_Of_Data/database.json` - 資料庫規範
- `GameSpace_previous/My_Own_Source_Of_Data/CONTRIBUTING_AGENT.txt` - 開發規範

## 🔗 預期的下載連結格式

上傳完成後，您將獲得類似以下的連結：

### Google Drive
```
https://drive.google.com/file/d/[FILE_ID]/view?usp=sharing
```

### Dropbox
```
https://www.dropbox.com/s/[FILE_ID]/GameSpace-Complete-Workspace-[TIMESTAMP].zip?dl=0
```

### OneDrive
```
https://1drv.ms/u/s![FILE_ID]!GameSpace-Complete-Workspace-[TIMESTAMP].zip
```

### GitHub Releases
```
https://github.com/[OWNER]/[REPO]/releases/download/v1.0-workspace-[TIMESTAMP]/GameSpace-Complete-Workspace-[TIMESTAMP].zip
```

## 📊 檔案大小估算
- 原始碼：約 50-100 MB
- 文件：約 10-20 MB
- .git 目錄：約 20-50 MB
- **總計：約 80-170 MB**

## ⚠️ 注意事項
1. 確保所有敏感資訊已移除
2. 檢查檔案大小是否在雲端服務限制內
3. 設定適當的分享權限
4. 測試下載連結是否正常運作

## 🚀 快速執行腳本

將以下內容儲存為 `package_and_upload.sh`：

```bash
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
```

執行方式：
```bash
chmod +x package_and_upload.sh
./package_and_upload.sh
```