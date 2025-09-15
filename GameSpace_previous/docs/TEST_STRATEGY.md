# 測試策略（繁體中文）

## 範疇
- 單元測試：Service/Helper/Utility（避免觸及四大不可變核心之邏輯變更）
- 整合測試：健康檢查端點 /health, /healthz, /healthz/db（必要時以 InMemory 或 Stub 取代外部依賴）
- 端到端：僅文件化流程（CI 不佈署），人工或 staging 驗收

## 準則
- 測試命名：MethodName_Condition_Expected
- 測試資料：以最小可表述資料為主；敏感資訊一律以環境變數或秘密管理供應
- 狀態隔離：測試間不得共享可變狀態

## 執行
- CI：`dotnet restore && dotnet build -c Release && dotnet test -c Release --no-build`
- 本機：同上；資料庫相關測試改以 InMemory/Stub，使測試可離線重現

## 覆蓋率（選配）
- 使用 coverlet 收集行覆蓋率；核心不可變邏輯不強制覆蓋率門檻
