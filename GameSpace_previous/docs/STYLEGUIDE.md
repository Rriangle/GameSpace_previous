# 前端/後端風格指南（不改動核心系統）

## 前端（Public vs Admin）
- Public：遵循 `index.txt` 與 Bootstrap；樣式檔置於 `wwwroot/css`；避免與 Admin 混用。
- Admin：採用 SB Admin；客製化請放 `wwwroot/css/admin-overrides.css`；禁止直接修改 `vendor/sb-admin/*`。

## Razor 命名/結構
- Layout 與 Partial 依領域分層（Public/Areas/Admin）；共用元件以 Partial 命名 `_*.cshtml`。
- 嚴禁在 View 內寫商業邏輯；資料處理放在 Controller/Service。

## 後端 C# 風格
- 檔名 = 型別名；命名採 PascalCase（類/方法）與 camelCase（區域變數）。
- 非同步方法以 `Async` 結尾；使用 `CancellationToken`。
- 早退（guard clause）、淺層巢狀；避免無意義的 try/catch。

## 註釋與文件
- 以繁體中文撰寫人類可讀文件；程式註釋聚焦「為何」。
- 禁止 `TODO/FIXME/TBD` 佔位；若未完成，請開 issue 並在 AUDIT 記錄。

## 資安與設定
- secrets/連線字串不得進入 repo；用環境變數/Secret 管理。
- 對外日誌需過濾機敏資訊。
