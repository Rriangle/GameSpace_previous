# GameSpace 嚴格稽核報告（STRICT）

**稽核日期**: 2025-09-13  
**稽核範圍**: 專案現況與規格/資料來源逐條比對  
**稽核基準**: CONTRIBUTING_AGENT.txt + old_0905.txt + new_0905.txt + database.json + index.txt

## Scope & Sources
- CONTRIBUTING_AGENT.txt（流程、約束、模組覆蓋、交付與語系規則）
- old_0905.txt + new_0905.txt（合計約 90% 規格）
- database.json（其餘 10% 缺口或衝突的最終權威）
- index.txt（前台 UI 規格：Bootstrap／玻璃風）
- SB Admin（後台 UI 模板；僅參考樣式與結構，不修改 vendor 檔）

> 規則（再聲明）: old_0905.txt + new_0905.txt 合計 ≈90% 規格；剩餘 10% 的缺口或衝突，由 database.json 做最終裁決。當出現模糊或命名不一致時，直接以 database.json 為準，並在提交訊息中備註此決策。

> UI 提醒: Public（前臺）遵循 `index.txt`；Admin（後台）採用 SB Admin。兩者樣式與資產不可混用；每個模組需明確標註歸屬 Public 或 Admin。

## Findings（差異與缺漏）

1) Admin 模板資產路徑不一致（可能導致 404）  
- Location: `GameSpace/Areas/Admin/Views/Shared/_Layout.cshtml`  
- Required State: 後台使用 SB Admin 靜態資產，路徑應指向 `~/lib/sb-admin/...`（或實際 vendor 安裝位置）；不得混用 Public 的樣式資產。  
- Current State: 版型引用 `~/vendor/sb-admin/...` 与 `~/vendor/...`；專案實際存在的路徑為 `GameSpace/wwwroot/lib/sb-admin/...`。  
- Resolution Plan: 將 `_Layout.cshtml` 的資產引用由 `~/vendor/sb-admin/...` 調整為 `~/lib/sb-admin/...`，並確認 FontAwesome、jQuery、Bootstrap 等對應來源一致。  
- Status: Fixed（本次修復）

2) 缺少資料庫健康檢查端點  
- Location: `GameSpace/Program.cs`  
- Required State: 依 CONTRIBUTING_AGENT.txt，需提供 `/healthz/db` 端點，連線成功回 `{ status: "ok" }`；失敗應具體回報。  
- Current State: 僅有 `/health` 與 `/healthz`；未提供 `/healthz/db`。  
- Resolution Plan: 於 `Program.cs` 新增 `GET /healthz/db`，使用 `GameSpaceDbContext.Database.CanConnectAsync()` 檢測連線。  
- Status: Open

3) AUDIT 文件位置不符約定（已矯正）  
- Location: 目前存在 `GameSpace/docs/AUDIT.md`；常數要求 `docs/AUDIT.md`（專案根目錄）。  
- Required State: 於專案根目錄維護 `docs/AUDIT.md` 作為全域稽核報告。  
- Current State: 根目錄缺少 `docs/AUDIT.md`。  
- Resolution Plan: 新增本檔（根目錄 `docs/AUDIT.md`）並後續統一維護於此；`GameSpace/docs/AUDIT.md` 視為舊版參考。  
- Status: Fixed（本次修復）

4) UI 歸屬一致性檢查（抽樣 OK，持續監控）  
- Location: `GameSpace/Views/Shared/_Layout.cshtml`（前台），`GameSpace/Areas/Admin/Views/Shared/_Layout.cshtml`（後台）  
- Required State: Public 使用 `index.txt` 規範（Bootstrap/玻璃風）；Admin 使用 SB Admin。  
- Current State: 前台 `_Layout.cshtml` 採 Bootstrap；後台存在 SB Admin 結構，但資產路徑需修正（見 Finding #1）。  
- Resolution Plan: 修正資產路徑後，再次抽樣檢查是否有混用。  
- Status: In-Progress

## Status 總覽
- Open: #1, #2, #4（部分）
- Fixed: #3

## Resolution Plan（近期批次修復）
1. 修正 Admin 版型資產路徑（AUDIT #1）。  
2. 新增 `/healthz/db` 端點並自動化測試（AUDIT #2）。  
3. 二次巡檢 UI 歸屬與資產混用（AUDIT #4 持續）。

## Notes（決策與說明）
- 術語與欄位命名衝突時，以 `database.json` 為最終權威（已在提交訊息中備註此原則）。  
- 不修改任何 vendor 來源檔，Admin 僅參考 SB Admin 的結構與 class 名稱。

