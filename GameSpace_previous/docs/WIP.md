# GameSpace Repository Cleanup Progress 
2025-09-13 目前進度：

- 已完成：Git 同步；閱讀 core 規格檔；盤點專案結構；建立根目錄 `docs/AUDIT.md`（嚴格）。
- 開始修復（AUDIT #1, #2）：
  - #1 Admin 版型資產路徑不一致（vendor vs lib/sb-admin）。
  - #2 新增 `/healthz/db` 健檢端點（以 `GameSpaceDbContext` 測連線）。
  - /health 與 /healthz 改為 text/plain 以符合測試。
  - 新增前台 index.txt 依賴 API 最小實作：`/api/pet/me`, `/api/pet/care`, `/api/pet/adventure`, `/api/points/balance`。

下一步（本輪預計）：
- 調整 `Areas/Admin/Views/Shared/_Layout.cshtml` 靜態資產路徑至 `~/lib/sb-admin/...` 並校對 Bootstrap/jQuery/FontAwesome 來源一致性。
- 於 `Program.cs` 新增 `/healthz/db`，成功回 `{ status:"ok" }`，失敗回 500 與錯誤描述。
- 提交「repair: Admin 資產路徑；feat: /healthz/db」並在訊息中重申 90%/10% 與 UI 不混用原則。
