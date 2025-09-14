# GameSpace

一個基於 .NET 8.0 的遊戲平台解決方案，提供用戶管理、寵物系統、小遊戲、商城和社群功能。

## 專案結構

- **GameSpace** - 主要MVC Web應用程式
- **GameSpace.Data** - 數據訪問層（Entity Framework Core）
- **GameSpace.Infrastructure** - 基礎設施層
- **GameSpace.Models** - 數據模型
- **GameSpace.Services** - 業務服務層
- **GameSpace.Middleware** - 自定義中介軟體
- **Areas/Admin** - 管理後台區域（SB Admin風格）
- **Areas/Public** - 前台區域（Bootstrap風格）

## 快速開始

### 前置需求

- .NET 8.0 SDK
- Visual Studio 2022 或 VS Code
- SQL Server LocalDB 或 SQL Server

### 本地運行

1. 克隆專案
```bash
git clone <repository-url>
cd GameSpace_previous/GameSpace
```

2. 還原套件
```bash
dotnet restore
```

3. 建置解決方案
```bash
dotnet build
```

4. 運行 Web 應用程式
```bash
dotnet run
```

5. 訪問應用程式
- 前台：http://localhost:5000
- 管理後台：http://localhost:5000/Admin
- API文檔：http://localhost:5000/swagger

### 健康檢查

API 健康檢查端點：
- GET /health - 基本健康檢查
- GET /healthz - 簡單健康檢查

## 功能模組

### 用戶管理
- 用戶註冊和登入
- OAuth認證（Google、Facebook、Microsoft）
- 用戶資料管理
- RBAC權限控制

### 寵物系統
- 寵物養成
- 屬性升級
- 互動功能
- 每日簽到獎勵

### 小遊戲
- 多種小遊戲
- 積分系統
- 排行榜

### 商城系統
- 商品管理
- 訂單處理
- 優惠券系統
- 電子禮券系統

### 社群功能
- 論壇討論
- 文章分享
- 評論系統
- 好友系統
- 群組聊天

### 管理後台
- 用戶管理
- 內容管理
- 系統監控
- RBAC權限管理
- 客服工單系統

## 技術棧

- **後端**: .NET 8.0, ASP.NET Core MVC, Entity Framework Core
- **前端**: ASP.NET Core MVC, Bootstrap, jQuery, SB Admin
- **數據庫**: SQL Server
- **日誌**: Serilog
- **API 文檔**: Swagger/OpenAPI
- **認證**: ASP.NET Core Identity + OAuth

## 開發指南

### 代碼風格
- 使用繁體中文註釋
- 遵循 C# 命名慣例
- 使用 async/await 進行異步操作

### 數據庫
- 使用 Entity Framework Core 進行數據訪問
- 數據庫結構以 `database.json` 為單一來源
- 禁止使用 EF Core 遷移，直接使用 SQL 腳本

### UI 設計
- 前台使用 Bootstrap 玻璃風設計
- 管理後台使用 SB Admin 風格
- 嚴格分離前台和管理後台樣式

### 測試
- 單元測試
- 整合測試
- 端到端測試

## 配置

### OAuth 配置
在 `appsettings.Development.json` 中配置 OAuth 提供商：

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "Facebook": {
      "AppId": "your-facebook-app-id",
      "AppSecret": "your-facebook-app-secret"
    },
    "Microsoft": {
      "ClientId": "your-microsoft-client-id",
      "ClientSecret": "your-microsoft-client-secret"
    }
  }
}
```

## 授權

此專案採用 MIT 授權條款。