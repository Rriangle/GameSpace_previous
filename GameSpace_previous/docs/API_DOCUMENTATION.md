# GameSpace API 文檔

## 概述

GameSpace 是一個基於 ASP.NET Core MVC 的遊戲社群平台，提供完整的用戶管理、社群互動、商城購物等功能。

## 基礎資訊

- **Base URL**: `https://gamespace.example.com`
- **API Version**: v1
- **Content Type**: `application/json`
- **Authentication**: JWT Token / Session Cookie

## 認證與授權

### 登入
```http
POST /Identity/Auth/Login
Content-Type: application/json

{
    "account": "string",
    "password": "string"
}
```

**回應:**
```json
{
    "success": true,
    "message": "登入成功",
    "token": "jwt_token_here"
}
```

### 註冊
```http
POST /Identity/Auth/Register
Content-Type: application/json

{
    "account": "string",
    "password": "string",
    "confirmPassword": "string",
    "nickName": "string",
    "gender": "string",
    "idNumber": "string",
    "cellphone": "string",
    "email": "string",
    "address": "string",
    "dateOfBirth": "2024-01-01",
    "introduction": "string"
}
```

## 用戶管理 API

### 獲取用戶資訊
```http
GET /MemberManagement/User/Profile
Authorization: Bearer {token}
```

### 更新用戶資訊
```http
PUT /MemberManagement/User/Profile
Authorization: Bearer {token}
Content-Type: application/json

{
    "nickName": "string",
    "gender": "string",
    "cellphone": "string",
    "email": "string",
    "address": "string",
    "introduction": "string"
}
```

## 錢包系統 API

### 獲取錢包資訊
```http
GET /MemberManagement/Wallet
Authorization: Bearer {token}
```

**回應:**
```json
{
    "wallet": {
        "userId": 1,
        "userPoint": 1000
    },
    "history": [
        {
            "logId": 1,
            "changeType": "Point",
            "pointsChanged": 100,
            "description": "每日簽到獲得點數",
            "changeTime": "2024-12-19T10:00:00Z"
        }
    ],
    "coupons": [],
    "evouchers": []
}
```

### 添加點數
```http
POST /MemberManagement/Wallet/AddPoints
Authorization: Bearer {token}
Content-Type: application/json

{
    "amount": 100,
    "description": "管理員贈送"
}
```

### 兌換優惠券
```http
POST /MemberManagement/Wallet/RedeemCoupon
Authorization: Bearer {token}
Content-Type: application/json

{
    "couponTypeId": 1
}
```

## 每日簽到 API

### 執行簽到
```http
POST /MemberManagement/SignIn/PerformSignIn
Authorization: Bearer {token}
```

**回應:**
```json
{
    "success": true,
    "message": "簽到成功！獲得 20 點積分，寵物經驗 +10。",
    "signInRecord": {
        "logId": 1,
        "userId": 1,
        "signTime": "2024-12-19T10:00:00Z",
        "pointsGained": 20,
        "expGained": 10,
        "couponGained": "0"
    }
}
```

### 獲取簽到狀態
```http
GET /MemberManagement/SignIn
Authorization: Bearer {token}
```

## 寵物系統 API

### 獲取寵物資訊
```http
GET /MemberManagement/Pet
Authorization: Bearer {token}
```

### 創建寵物
```http
POST /MemberManagement/Pet/Create
Authorization: Bearer {token}
Content-Type: application/json

{
    "petName": "我的史萊姆"
}
```

### 寵物照顧
```http
POST /MemberManagement/Pet/CareAction
Authorization: Bearer {token}
Content-Type: application/json

{
    "actionType": "feed" // feed, bath, play, rest
}
```

### 更換寵物外觀
```http
POST /MemberManagement/Pet/ChangeSkinColor
Authorization: Bearer {token}
Content-Type: application/json

{
    "newColorHex": "#ff6b6b"
}
```

## 小遊戲 API

### 開始遊戲
```http
POST /MiniGame/Game/StartGame
Authorization: Bearer {token}
Content-Type: application/json

{
    "petId": 1,
    "level": 1
}
```

### 結束遊戲
```http
POST /MiniGame/Game/EndGame
Authorization: Bearer {token}
Content-Type: application/json

{
    "playId": 1,
    "result": "Win", // Win, Lose, Abort
    "monsterCount": 5,
    "speedMultiplier": 1.0,
    "hungerDelta": -10,
    "moodDelta": 15,
    "staminaDelta": -15,
    "cleanlinessDelta": 5
}
```

### 獲取遊戲歷史
```http
GET /MiniGame/Game/History?page=1
Authorization: Bearer {token}
```

## 論壇系統 API

### 獲取論壇列表
```http
GET /Forum/Forum
```

### 創建論壇
```http
POST /Forum/Forum/Create
Authorization: Bearer {token}
Content-Type: application/json

{
    "name": "遊戲討論",
    "description": "討論各種遊戲話題",
    "gameId": 1
}
```

### 獲取討論串
```http
GET /Forum/Forum/Details/{id}
```

### 創建討論串
```http
POST /Forum/Thread/Create
Authorization: Bearer {token}
Content-Type: application/json

{
    "forumId": 1,
    "title": "討論串標題",
    "content": "討論串內容"
}
```

### 回覆文章
```http
POST /Forum/Thread/Reply
Authorization: Bearer {token}
Content-Type: application/json

{
    "threadId": 1,
    "content": "回覆內容",
    "parentPostId": null
}
```

### 添加反應
```http
POST /Forum/Thread/AddReaction
Authorization: Bearer {token}
Content-Type: application/json

{
    "postId": 1,
    "reactionType": "like" // like, dislike, love, laugh
}
```

## 社群互動 API

### 獲取社群首頁
```http
GET /social_hub/Social
Authorization: Bearer {token}
```

### 創建群組
```http
POST /social_hub/Social/CreateGroup
Authorization: Bearer {token}
Content-Type: application/json

{
    "groupName": "我的群組",
    "description": "群組描述",
    "isPublic": true
}
```

### 加入群組
```http
POST /social_hub/Social/JoinGroup
Authorization: Bearer {token}
Content-Type: application/json

{
    "groupId": 1
}
```

### 發送群組訊息
```http
POST /social_hub/Social/SendMessage
Authorization: Bearer {token}
Content-Type: application/json

{
    "groupId": 1,
    "content": "訊息內容"
}
```

### 發送好友請求
```http
POST /social_hub/Social/SendFriendRequest
Authorization: Bearer {token}
Content-Type: application/json

{
    "toUserId": 2
}
```

### 接受好友請求
```http
POST /social_hub/Social/AcceptFriendRequest
Authorization: Bearer {token}
Content-Type: application/json

{
    "relationId": 1
}
```

## 商城系統 API

### 獲取商品列表
```http
GET /OnlineStore/Store?category=遊戲道具&page=1
```

### 獲取商品詳情
```http
GET /OnlineStore/Store/Details/{id}
```

### 加入購物車
```http
POST /OnlineStore/Store/AddToCart
Authorization: Bearer {token}
Content-Type: application/json

{
    "productId": 1,
    "quantity": 2
}
```

### 獲取購物車
```http
GET /OnlineStore/Store/Cart
Authorization: Bearer {token}
```

### 結帳
```http
POST /OnlineStore/Store/Checkout
Authorization: Bearer {token}
Content-Type: application/json

{
    "shippingAddress": "收貨地址",
    "paymentMethod": "信用卡"
}
```

### 使用優惠券
```http
POST /OnlineStore/Store/UseCoupon
Authorization: Bearer {token}
Content-Type: application/json

{
    "orderId": 1,
    "couponCode": "DISCOUNT10"
}
```

## 管理後台 API

### 獲取系統統計
```http
GET /Admin/Dashboard
Authorization: Bearer {admin_token}
```

### 獲取用戶列表
```http
GET /Admin/Dashboard/Users?page=1
Authorization: Bearer {admin_token}
```

### 更新用戶狀態
```http
POST /Admin/Dashboard/UpdateUserStatus
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "userId": 1,
    "isActive": true
}
```

### 鎖定用戶
```http
POST /Admin/Dashboard/LockUser
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "userId": 1,
    "lockoutDays": 7
}
```

### 刪除文章
```http
POST /Admin/Dashboard/DeletePost
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "postId": 1
}
```

### 封禁用戶
```http
POST /Admin/Dashboard/BanUser
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "userId": 1,
    "reason": "違反社群規範",
    "banDays": 30
}
```

## 錯誤處理

所有 API 都遵循統一的錯誤回應格式：

```json
{
    "success": false,
    "message": "錯誤訊息",
    "errorCode": "ERROR_CODE",
    "details": {
        "field": "具體錯誤詳情"
    }
}
```

### 常見錯誤代碼

- `INVALID_CREDENTIALS`: 認證失敗
- `UNAUTHORIZED`: 未授權
- `FORBIDDEN`: 禁止訪問
- `NOT_FOUND`: 資源不存在
- `VALIDATION_ERROR`: 驗證錯誤
- `INSUFFICIENT_POINTS`: 點數不足
- `INVENTORY_SHORTAGE`: 庫存不足
- `RATE_LIMIT_EXCEEDED`: 請求頻率過高

## 分頁

大部分列表 API 都支援分頁：

```http
GET /api/endpoint?page=1&pageSize=20
```

**回應:**
```json
{
    "success": true,
    "data": [...],
    "pagination": {
        "page": 1,
        "pageSize": 20,
        "totalCount": 100,
        "totalPages": 5
    }
}
```

## 搜尋

支援搜尋的 API 都接受 `keyword` 參數：

```http
GET /api/endpoint?keyword=搜尋關鍵字&page=1
```

## 排序

支援排序的 API 都接受 `sortBy` 和 `sortOrder` 參數：

```http
GET /api/endpoint?sortBy=createdAt&sortOrder=desc&page=1
```

## 篩選

支援篩選的 API 都接受相應的篩選參數：

```http
GET /api/endpoint?category=遊戲道具&minPrice=100&maxPrice=500&page=1
```

## 速率限制

- 一般 API: 100 請求/分鐘
- 認證 API: 10 請求/分鐘
- 上傳 API: 20 請求/分鐘

超過限制時會返回 `429 Too Many Requests` 狀態碼。

## 版本控制

API 版本通過 URL 路徑控制：

- v1: `/api/v1/endpoint`
- v2: `/api/v2/endpoint` (未來版本)

## 更新日誌

### v1.0.0 (2024-12-19)
- 初始版本發布
- 完整的用戶管理系統
- 錢包和簽到系統
- 寵物養成系統
- 小遊戲系統
- 論壇系統
- 社群互動系統
- 商城系統
- 管理後台系統