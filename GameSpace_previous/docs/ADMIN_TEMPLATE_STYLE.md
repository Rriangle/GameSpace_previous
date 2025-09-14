# Admin 樣式規範（SB Admin）

> 本文件補齊 Admin 端樣式規範，嚴禁直接修改 vendor 檔案。

## 基本原則
- 後台（Admin）一律採用 SB Admin 樣式，禁止與 Public（前台/Bootstrap）樣式混用。
- vendor 檔案唯讀：`wwwroot/lib/sb-admin/*`、`wwwroot/lib/bootstrap/*` 等第三方資產不可修改。
- 自訂樣式與版型請放在 Admin Area 專屬路徑：
  - 版面：`Areas/Admin/Views/Shared/_Layout.cshtml`
  - 側邊欄：`Areas/Admin/Views/Shared/_Sidebar.cshtml`
  - 頂欄：`Areas/Admin/Views/Shared/_Topbar.cshtml`
  - 視覺覆寫（建議）：`wwwroot/css/admin-overrides.css`

## 檔案組織
- Admin 與 Public 分離：不得在相同頁面混用兩者樣式或資產。
- Admin 頁面中的元件請優先使用 SB Admin 現成樣式；若需擴充，於 `admin-overrides.css` 寫覆寫規則，避免動到 vendor。
- 共用元件請以 Partial 分離，維持可重用與清晰邏輯。

## 前端資產
- 僅以 `<link>` / `<script>` 引用 vendor 檔案；禁止在 vendor 目錄下新增或修改檔案。
- 若需客製化配色或排版，請在 `admin-overrides.css` 以選擇器權重覆寫，或新增 Admin 專用 class。

## 可接受的客製化方式
- 在 Admin Area 的 Razor 檢視中增修 DOM 結構（不觸及 vendor js/css）
- 以 CSS 覆寫（`admin-overrides.css`）
- 新增 Admin 專用的 JS（不修改 vendor js），檔案放在 `wwwroot/js/admin-*` 前綴

## 禁止事項
- 修改 `wwwroot/lib/sb-admin/*`、`wwwroot/lib/bootstrap/*`、`wwwroot/lib/jquery/*` 等第三方資產
- 在 Public 與 Admin 間互相引用彼此的樣式或 layout

## 驗收檢核
- 任一 Admin 頁面僅引用 Admin 專屬資產與 SB Admin vendor；未混用 Public 樣式
- vendor 目錄無任何變更（Git diff 應為空）
- 覆寫均集中於 `admin-overrides.css`，並在 `_Layout.cshtml` 單點註冊

