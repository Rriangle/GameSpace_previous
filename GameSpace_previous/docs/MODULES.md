# 模組與路由對照（Public/Admin 標註）

> 依 `old_0905.txt`、`new_0905.txt` 彙整；模糊處以 `database.json` 為最終權威。

## Public（前台，遵循 index.txt/Bootstrap）
- Forum：/forum, /forum/category/{id}, /forum/thread/{id}
- Auth：/auth/login, /auth/register
- Profile：/profile
- Pet：/pet
- Sign-in：/signin
- MiniGame：/minigame
- Shop（官方商城）：/shop
- Chat：/chat
- Groups：/groups
- Health 檢查：/health, /healthz, /healthz/db

## Admin（後台，SB Admin，不得混用 Public 樣式）
- 使用者管理：/Admin/Users
- 寵物管理：/Admin/Pets
- 簽到統計：/Admin/CheckIn
- 遊戲記錄：/Admin/Games
- 會員點數管理：/Admin/Wallet
- 商城管理：/Admin/Store
- 系統設定：/Admin/System

## 檔案歸置建議
- Public Razor：`Views/*` 或 Areas/Public
- Admin Razor：`Areas/Admin/Views/*`
- API：對應 Controllers 與路由前綴 `/api/*`

