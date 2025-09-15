# 貢獻指南（繁體中文）

## 分支與提交規範
- 僅允許使用單一分支：main（禁止建立任何其他本地/遠端分支、禁止開 PR）
- 直接提交到 main（遵守已安裝的 pre-commit / pre-push hooks）
- 人類可讀內容（提交訊息、文件、日誌）一律使用繁體中文（程式識別符維持原語言）
- 微批次原則：每次 ≤ 3 檔或 ≤ 400 行，通過建置/測試後再推送

## 規格來源（資料/介面/安全）
- old_0905.txt + new_0905.txt ≈ 90% 規格；其餘 10% 以 My_Own_Source_Of_Data/database.json 為最終權威
- Public（前台）依 index.txt；Admin（後台）採 SB Admin，兩者風格嚴禁混用；vendor/sb-admin/* 僅可參考不可修改
- 嚴禁在 repo/提交/報告中洩漏 secrets/token/連線字串；請使用環境變數或 Secret 管理

## 四大不可變系統（禁止修改邏輯）
- Wallet / Sign-in / Pet / MiniGame 為嚴格不可變核心；若有問題，僅可修正文檔或非邏輯的格式/跳脫

## 健康檢查與資料庫
- 健康檢查：/health（JSON）、/healthz（純文字）、/healthz/db（JSON：{ "status": "ok" }）
- 資料庫以 database.json 為單一權威；不使用 EF Migrations；本機請以 SQL Server 匯入/同步

## 提交訊息建議
- 類型：docs/audit/ops/chore/fix/feat 等 + 簡述（繁體中文）
- 若牽涉規格衝突處理，請在訊息中註明「以 database.json 為最終權威」及原因
