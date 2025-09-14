# 安全政策（不含任何 secrets）
- 禁止在 repo/提交/報告中洩漏 secrets/token/連線字串
- 機敏參數僅能透過環境變數或 Secrets 管理
- 嚴禁將憑證寫入程式碼或設定檔（改用占位名稱）
- 對外輸出日誌需過濾機敏內容（API Key、Cookie、JWT 等）
