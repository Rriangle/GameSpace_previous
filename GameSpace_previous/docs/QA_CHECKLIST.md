# QA 檢查清單（只讀流程、不得改核心系統）

## 一般
- [ ] 所有人類可讀內容為繁體中文
- [ ] 僅使用 `main` 分支；無 PR、無其他分支
- [ ] vendor/sb-admin/* 未被修改

## 健康檢查
- [ ] GET /health 回傳 JSON 且 200
- [ ] GET /healthz 回傳 text/plain 且 200
- [ ] GET /healthz/db 回傳 { "status": "ok" } 或等價 JSON

## 文件一致性
- [ ] README 與 docs/* 與 `database.json` 一致（以其為最終權威）
- [ ] Public 依 index.txt；Admin 採 SB Admin，未混用

## 安全
- [ ] 無 secrets/token/連線字串出現在原始碼/報告/提交
- [ ] 日誌與報告已遮罩敏感資訊

## 建置/測試
- [ ] dotnet build -c Release 成功（或錯誤已於報告登記）
- [ ] dotnet test -c Release 成功（或失敗已於報告登記）
