# GameSpace

一個基於 .NET 8.0 的遊戲平台解決方案，提供用戶管理、寵物系統、小遊戲、商城和社群功能。

## 專案結構

- **GameSpace.Api** - Web API 專案
- **GameSpace.Core** - 核心業務邏輯
- **GameSpace.Infrastructure** - 基礎設施層
- **GameSpace.Web** - MVC Web 應用程式

## 快速開始

### 前置需求

- .NET 8.0 SDK
- Visual Studio 2022 或 VS Code

### 本地運行

1. 克隆專案
`ash
git clone <repository-url>
cd GameSpace
`

2. 還原套件
`ash
dotnet restore
`

3. 建置解決方案
`ash
dotnet build
`

4. 運行 API
`ash
cd GameSpace.Api
dotnet run
`

5. 運行 Web 應用程式
`ash
cd GameSpace.Web
dotnet run
`

### 健康檢查

API 健康檢查端點：
- GET /api/health - 基本健康檢查
- GET /api/health/detailed - 詳細健康檢查

## 功能模組

### 用戶管理
- 用戶註冊和登入
- 用戶資料管理
- 權限控制

### 寵物系統
- 寵物養成
- 屬性升級
- 互動功能

### 小遊戲
- 多種小遊戲
- 積分系統
- 排行榜

### 商城系統
- 商品管理
- 訂單處理
- 優惠券系統

### 社群功能
- 論壇討論
- 文章分享
- 評論系統

## 技術棧

- **後端**: .NET 8.0, ASP.NET Core, Entity Framework Core
- **前端**: ASP.NET Core MVC, Bootstrap, jQuery
- **日誌**: Serilog
- **API 文檔**: Swagger/OpenAPI

## 開發指南

### 代碼風格
- 使用繁體中文註釋
- 遵循 C# 命名慣例
- 使用 async/await 進行異步操作

### 測試
- 單元測試
- 整合測試
- 端到端測試

## 授權

此專案採用 MIT 授權條款。
