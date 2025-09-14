# 快速上傳命令

## 🚀 一鍵打包命令

在您的 VM 終端中執行：

```bash
cd /workspace && chmod +x artifacts/package_and_upload.sh && ./artifacts/package_and_upload.sh
```

## 📦 手動打包命令

如果腳本無法執行，請手動執行：

```bash
# 進入工作區目錄
cd /workspace

# 創建時間戳記
TIMESTAMP=$(date +%Y%m%d-%H%M%S)

# 創建 ZIP 檔案
zip -r "GameSpace-Complete-Workspace-$TIMESTAMP.zip" . \
    -x "*.pyc" "__pycache__/*" "node_modules/*" ".pytest_cache/*" \
    -x "*.pyo" "*.pyd" ".DS_Store" "Thumbs.db" "*.log" "*.tmp" \
    -x "artifacts/workspace-*.zip" "artifacts/project-*.zip"

# 顯示檔案資訊
ls -lh "GameSpace-Complete-Workspace-$TIMESTAMP.zip"
```

## ☁️ 上傳到雲端服務

### Google Drive (推薦)
1. 開啟 https://drive.google.com
2. 點擊「新增」→「檔案上傳」
3. 選擇 `GameSpace-Complete-Workspace-*.zip`
4. 右鍵檔案→「取得連結」→「知道連結的任何人都可以檢視」

### Dropbox
1. 開啟 https://dropbox.com
2. 拖拽 ZIP 檔案到網頁
3. 右鍵檔案→「分享」→「建立連結」

### OneDrive
1. 開啟 https://onedrive.live.com
2. 上傳 ZIP 檔案
3. 右鍵檔案→「分享」→「複製連結」

## 🔗 預期連結格式

- **Google Drive**: `https://drive.google.com/file/d/[FILE_ID]/view?usp=sharing`
- **Dropbox**: `https://www.dropbox.com/s/[FILE_ID]/GameSpace-Complete-Workspace-[TIMESTAMP].zip?dl=0`
- **OneDrive**: `https://1drv.ms/u/s![FILE_ID]!GameSpace-Complete-Workspace-[TIMESTAMP].zip`

## 📊 檔案大小
預估約 80-170 MB（包含完整專案、文件、.git 目錄）

## ⚡ 最快速方法
1. 執行打包命令
2. 開啟 Google Drive
3. 拖拽 ZIP 檔案上傳
4. 取得分享連結
5. 提供連結給我