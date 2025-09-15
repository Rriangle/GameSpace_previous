# C# 程式碼規範（繁體中文）

## 命名
- 類/介面/列舉：PascalCase（介面以 I 前綴）
- 方法/屬性/事件：PascalCase；非同步方法以 Async 結尾
- 參數/區域變數：camelCase；避免 1–2 字母縮寫

## 結構與可讀性
- 使用早退（guard clause），避免深層巢狀
- 例外處理僅包覆可能失敗區塊；不得吞噬例外
- 每個檔一個公開型別，檔名 = 型別名

## 非同步/相依注入
- 優先使用 async/await；傳遞 CancellationToken
- 服務註冊集中於 DependencyInjection 類別

## 註釋與文件
- 註釋描述「為何」而非「如何」；避免冗長
- 公開 API 使用 XML Doc；文件使用繁體中文

## 測試
- 測試命名：MethodName_Condition_Expected
- 僅測可觀察行為，不測內部實作細節

## 安全與設定
- 不將 secrets/連線字串寫入原始碼或提交訊息；以環境變數/Secrets 管理
- 對外日誌需遮罩機敏內容
