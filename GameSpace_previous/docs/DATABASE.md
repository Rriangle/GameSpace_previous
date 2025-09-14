# 資料庫概要（以 database.json 為單一權威）

> 僅做只讀摘要；任何模糊與衝突，一律以 `My_Own_Source_Of_Data/database.json` 為準。

## 代表性資料表（擇要）
- Users（會員主檔）
- User_Introduce（會員資料）
- User_Rights（權限/狀態）
- User_wallet / WalletHistory（點數與異動）
- UserSignInStats（每日簽到）
- Pet（寵物）
- MiniGame（小遊戲紀錄）
- CouponType / Coupon（優惠券）
- EVoucherType / EVoucher（電子禮券）
- forums / threads / thread_posts（論壇）
- posts（文章）
- ProductInfo / OrderInfo / OrderItems（官方商城）

## 只讀原則
- 禁用 EF Migrations；結構以 `database.json` 為準
- 種子資料/假資料僅透過資料庫層插入（遵循規格）

