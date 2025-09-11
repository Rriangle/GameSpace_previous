using GameSpace.Areas.MiniGame.Models;
using Microsoft.Extensions.Logging;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 錢包服務實作 - 處理積分、優惠券、電子禮券相關業務邏輯
    /// 對應 database.sql User_Wallet, Coupon, CouponType, EVoucher, EVoucherType, WalletHistory 資料表
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;

        public WalletService(ILogger<WalletService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 取得錢包總覽資料 - 聚合多個資料表的查詢結果
        /// </summary>
        public async Task<WalletOverviewDisplayViewModel> GetWalletOverviewAsync(int userId)
        {
            await Task.Delay(10); // 模擬複合查詢延遲

            // 模擬聚合查詢 - 實際會 JOIN 多個資料表
            var currentPoints = await GetUserPointsAsync(userId);
            var availableCoupons = await GetUserAvailableCouponsAsync(userId, 3);
            var availableEVouchers = await GetUserAvailableEVouchersAsync(userId, 3);
            var recentTransactions = await GetWalletHistoryAsync(userId, 0, 5);
            var monthlyStats = await GetMonthlyStatsAsync(userId);

            return new WalletOverviewDisplayViewModel
            {
                UserId = userId,
                UserName = "測試使用者", // 實際會從 Users 資料表取得
                CurrentPoints = currentPoints,
                AvailableCouponsCount = await CountUserCouponsAsync(userId, false),
                UsedCouponsCount = await CountUserCouponsAsync(userId, true),
                AvailableEVouchersCount = await CountUserEVouchersAsync(userId, false),
                UsedEVouchersCount = await CountUserEVouchersAsync(userId, true),
                MonthlyPointsEarned = monthlyStats.Earned,
                MonthlyPointsSpent = monthlyStats.Spent,
                RecentTransactions = recentTransactions,
                AvailableCoupons = availableCoupons,
                AvailableEVouchers = availableEVouchers
            };
        }

        /// <summary>
        /// 使用優惠券 - 標記為已使用並記錄到 WalletHistory
        /// </summary>
        public async Task<CouponOperationResultViewModel> UseCouponAsync(int couponId, int userId, int? orderId = null)
        {
            try
            {
                // 驗證優惠券擁有權
                if (!await ValidateCouponOwnershipAsync(couponId, userId))
                {
                    return new CouponOperationResultViewModel
                    {
                        Success = false,
                        Message = "無效的優惠券或不屬於您",
                        OperationType = "使用",
                        OperationTime = DateTime.Now
                    };
                }

                // 取得優惠券詳細資料
                var couponDetails = await GetCouponDetailsAsync(couponId);
                if (couponDetails == null || couponDetails.IsUsed)
                {
                    return new CouponOperationResultViewModel
                    {
                        Success = false,
                        Message = "優惠券不存在或已被使用",
                        OperationType = "使用",
                        OperationTime = DateTime.Now
                    };
                }

                var operationTime = DateTime.Now;

                // 模擬資料庫更新操作 - Stage 4 階段
                await SimulateCouponUseAsync(couponId, operationTime, orderId);
                await SimulateWalletHistoryInsertAsync(userId, "消費", 0, couponDetails.CouponCode, $"使用優惠券 {couponDetails.CouponCode}", operationTime);

                var remainingPoints = await GetUserPointsAsync(userId);

                _logger.LogInformation("優惠券使用成功 - CouponId: {CouponId}, UserId: {UserId}, Code: {Code}", 
                    couponId, userId, couponDetails.CouponCode);

                return new CouponOperationResultViewModel
                {
                    Success = true,
                    Message = $"優惠券 {couponDetails.CouponCode} 使用成功",
                    OperationType = "使用",
                    CouponCode = couponDetails.CouponCode,
                    RemainingPoints = remainingPoints,
                    OperationTime = operationTime,
                    CouponDetails = couponDetails
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "優惠券使用失敗 - CouponId: {CouponId}, UserId: {UserId}", couponId, userId);
                
                return new CouponOperationResultViewModel
                {
                    Success = false,
                    Message = "優惠券使用失敗，請稍後再試",
                    OperationType = "使用",
                    OperationTime = DateTime.Now
                };
            }
        }

        /// <summary>
        /// 兌換新優惠券 - 使用積分兌換指定類型的優惠券
        /// </summary>
        public async Task<CouponOperationResultViewModel> ExchangeCouponAsync(int couponTypeId, int userId)
        {
            try
            {
                // 取得優惠券類型資料
                var couponType = await GetCouponTypeAsync(couponTypeId);
                if (couponType == null)
                {
                    return new CouponOperationResultViewModel
                    {
                        Success = false,
                        Message = "無效的優惠券類型",
                        OperationType = "兌換",
                        OperationTime = DateTime.Now
                    };
                }

                // 檢查使用者積分是否足夠
                var userPoints = await GetUserPointsAsync(userId);
                if (userPoints < couponType.PointsCost)
                {
                    return new CouponOperationResultViewModel
                    {
                        Success = false,
                        Message = $"積分不足，需要 {couponType.PointsCost} 積分",
                        OperationType = "兌換",
                        PointsCost = couponType.PointsCost,
                        OperationTime = DateTime.Now
                    };
                }

                var operationTime = DateTime.Now;
                var newCouponCode = GenerateCouponCode(couponType.Name, userId);

                // 模擬資料庫操作 - 建立新優惠券並扣除積分
                var newCouponId = await SimulateCouponCreateAsync(newCouponCode, couponTypeId, userId, operationTime);
                await SimulateUserPointsDeductAsync(userId, couponType.PointsCost);
                await SimulateWalletHistoryInsertAsync(userId, "消費", -couponType.PointsCost, newCouponCode, $"兌換優惠券 {couponType.Name}", operationTime);

                var newCouponDetails = new CouponViewModel
                {
                    CouponID = newCouponId,
                    CouponCode = newCouponCode,
                    CouponTypeID = couponTypeId,
                    UserID = userId,
                    IsUsed = false,
                    AcquiredTime = operationTime,
                    CouponType = couponType
                };

                _logger.LogInformation("優惠券兌換成功 - UserId: {UserId}, TypeId: {TypeId}, Code: {Code}, Cost: {Cost}", 
                    userId, couponTypeId, newCouponCode, couponType.PointsCost);

                return new CouponOperationResultViewModel
                {
                    Success = true,
                    Message = $"兌換成功！獲得 {couponType.Name} 優惠券",
                    OperationType = "兌換",
                    CouponCode = newCouponCode,
                    PointsCost = couponType.PointsCost,
                    RemainingPoints = userPoints - couponType.PointsCost,
                    OperationTime = operationTime,
                    CouponDetails = newCouponDetails
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "優惠券兌換失敗 - TypeId: {TypeId}, UserId: {UserId}", couponTypeId, userId);
                
                return new CouponOperationResultViewModel
                {
                    Success = false,
                    Message = "優惠券兌換失敗，請稍後再試",
                    OperationType = "兌換",
                    OperationTime = DateTime.Now
                };
            }
        }

        /// <summary>
        /// 使用電子禮券
        /// </summary>
        public async Task<EVoucherOperationResultViewModel> UseEVoucherAsync(int voucherId, int userId)
        {
            try
            {
                // 驗證電子禮券擁有權
                if (!await ValidateEVoucherOwnershipAsync(voucherId, userId))
                {
                    return new EVoucherOperationResultViewModel
                    {
                        Success = false,
                        Message = "無效的電子禮券或不屬於您",
                        OperationType = "使用",
                        OperationTime = DateTime.Now
                    };
                }

                var voucherDetails = await GetEVoucherDetailsAsync(voucherId);
                if (voucherDetails == null || voucherDetails.IsUsed)
                {
                    return new EVoucherOperationResultViewModel
                    {
                        Success = false,
                        Message = "電子禮券不存在或已被使用",
                        OperationType = "使用",
                        OperationTime = DateTime.Now
                    };
                }

                var operationTime = DateTime.Now;

                // 模擬電子禮券使用與記錄
                await SimulateEVoucherUseAsync(voucherId, operationTime);
                await SimulateEVoucherRedeemLogAsync(voucherId, userId, operationTime);

                var remainingPoints = await GetUserPointsAsync(userId);

                _logger.LogInformation("電子禮券使用成功 - VoucherId: {VoucherId}, UserId: {UserId}, Code: {Code}", 
                    voucherId, userId, voucherDetails.EVoucherCode);

                return new EVoucherOperationResultViewModel
                {
                    Success = true,
                    Message = $"電子禮券 {voucherDetails.EVoucherCode} 使用成功",
                    OperationType = "使用",
                    EVoucherCode = voucherDetails.EVoucherCode,
                    RemainingPoints = remainingPoints,
                    OperationTime = operationTime,
                    EVoucherDetails = voucherDetails
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "電子禮券使用失敗 - VoucherId: {VoucherId}, UserId: {UserId}", voucherId, userId);
                
                return new EVoucherOperationResultViewModel
                {
                    Success = false,
                    Message = "電子禮券使用失敗，請稍後再試",
                    OperationType = "使用",
                    OperationTime = DateTime.Now
                };
            }
        }

        /// <summary>
        /// 兌換新電子禮券
        /// </summary>
        public async Task<EVoucherOperationResultViewModel> ExchangeEVoucherAsync(int voucherTypeId, int userId)
        {
            try
            {
                var voucherType = await GetEVoucherTypeAsync(voucherTypeId);
                if (voucherType == null)
                {
                    return new EVoucherOperationResultViewModel
                    {
                        Success = false,
                        Message = "無效的電子禮券類型",
                        OperationType = "兌換",
                        OperationTime = DateTime.Now
                    };
                }

                var userPoints = await GetUserPointsAsync(userId);
                if (userPoints < voucherType.PointsCost)
                {
                    return new EVoucherOperationResultViewModel
                    {
                        Success = false,
                        Message = $"積分不足，需要 {voucherType.PointsCost} 積分",
                        OperationType = "兌換",
                        PointsCost = voucherType.PointsCost,
                        OperationTime = DateTime.Now
                    };
                }

                var operationTime = DateTime.Now;
                var newVoucherCode = GenerateEVoucherCode(voucherType.Name, userId);
                var tokenString = GenerateSecureToken();
                var tokenExpiresAt = operationTime.AddDays(30); // 令牌30天後到期

                // 模擬資料庫操作 - 建立電子禮券、令牌並扣除積分
                var newVoucherId = await SimulateEVoucherCreateAsync(newVoucherCode, voucherTypeId, userId, operationTime);
                await SimulateEVoucherTokenCreateAsync(newVoucherId, tokenString, tokenExpiresAt);
                await SimulateUserPointsDeductAsync(userId, voucherType.PointsCost);
                await SimulateWalletHistoryInsertAsync(userId, "消費", -voucherType.PointsCost, newVoucherCode, $"兌換電子禮券 {voucherType.Name}", operationTime);

                var newVoucherDetails = new EVoucherViewModel
                {
                    EVoucherID = newVoucherId,
                    EVoucherCode = newVoucherCode,
                    EVoucherTypeID = voucherTypeId,
                    UserID = userId,
                    IsUsed = false,
                    AcquiredTime = operationTime,
                    EVoucherType = voucherType
                };

                _logger.LogInformation("電子禮券兌換成功 - UserId: {UserId}, TypeId: {TypeId}, Code: {Code}, Cost: {Cost}", 
                    userId, voucherTypeId, newVoucherCode, voucherType.PointsCost);

                return new EVoucherOperationResultViewModel
                {
                    Success = true,
                    Message = $"兌換成功！獲得 {voucherType.Name} 電子禮券",
                    OperationType = "兌換",
                    EVoucherCode = newVoucherCode,
                    PointsCost = voucherType.PointsCost,
                    RemainingPoints = userPoints - voucherType.PointsCost,
                    GeneratedToken = tokenString,
                    TokenExpiresAt = tokenExpiresAt,
                    OperationTime = operationTime,
                    EVoucherDetails = newVoucherDetails
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "電子禮券兌換失敗 - TypeId: {TypeId}, UserId: {UserId}", voucherTypeId, userId);
                
                return new EVoucherOperationResultViewModel
                {
                    Success = false,
                    Message = "電子禮券兌換失敗，請稍後再試",
                    OperationType = "兌換",
                    OperationTime = DateTime.Now
                };
            }
        }

        /// <summary>
        /// 取得錢包交易歷史 - 分頁查詢 WalletHistory
        /// </summary>
        public async Task<List<WalletHistoryViewModel>> GetWalletHistoryAsync(int userId, int pageIndex = 0, int pageSize = 20)
        {
            await Task.Delay(10);
            
            // Stage 4 模擬 - 實際會查詢 WalletHistory 資料表
            return new List<WalletHistoryViewModel>
            {
                new WalletHistoryViewModel { LogID = 1, UserID = userId, ChangeType = "獲得", PointsChanged = 10, Description = "每日簽到", ChangeTime = DateTime.Now.AddHours(-2) },
                new WalletHistoryViewModel { LogID = 2, UserID = userId, ChangeType = "獲得", PointsChanged = 50, Description = "完成小遊戲", ChangeTime = DateTime.Now.AddHours(-5) },
                new WalletHistoryViewModel { LogID = 3, UserID = userId, ChangeType = "消費", PointsChanged = -30, ItemCode = "SHOP20", Description = "兌換優惠券", ChangeTime = DateTime.Now.AddDays(-1) }
            };
        }

        /// <summary>
        /// 取得可兌換的優惠券類型列表
        /// </summary>
        public async Task<List<CouponTypeViewModel>> GetAvailableCouponTypesAsync()
        {
            await Task.Delay(5);
            
            // Stage 4 模擬 - 實際會查詢 CouponType 資料表 WHERE ValidTo > GETDATE()
            return new List<CouponTypeViewModel>
            {
                new CouponTypeViewModel { CouponTypeID = 1, Name = "購物9折券", DiscountType = "percentage", DiscountValue = 0.9m, PointsCost = 100, Description = "適用於所有商品的9折優惠" },
                new CouponTypeViewModel { CouponTypeID = 2, Name = "遊戲雙倍券", DiscountType = "multiplier", DiscountValue = 2.0m, PointsCost = 150, Description = "遊戲積分雙倍獎勵" },
                new CouponTypeViewModel { CouponTypeID = 3, Name = "特殊活動券", DiscountType = "special", DiscountValue = 1.0m, PointsCost = 200, Description = "限時特殊活動參與券" }
            };
        }

        /// <summary>
        /// 取得可兌換的電子禮券類型列表
        /// </summary>
        public async Task<List<EVoucherTypeViewModel>> GetAvailableEVoucherTypesAsync()
        {
            await Task.Delay(5);
            
            // Stage 4 模擬 - 實際會查詢 EVoucherType 資料表
            return new List<EVoucherTypeViewModel>
            {
                new EVoucherTypeViewModel { EVoucherTypeID = 1, Name = "100元電子禮券", ValueAmount = 100m, PointsCost = 500, TotalAvailable = 50, Description = "面值100元的電子禮券" },
                new EVoucherTypeViewModel { EVoucherTypeID = 2, Name = "50元電子禮券", ValueAmount = 50m, PointsCost = 250, TotalAvailable = 100, Description = "面值50元的電子禮券" }
            };
        }

        /// <summary>
        /// 檢查使用者是否擁有指定優惠券
        /// </summary>
        public async Task<bool> ValidateCouponOwnershipAsync(int couponId, int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢 Coupon WHERE CouponID = @couponId AND UserID = @userId
            return true; // 暫時回傳有效
        }

        /// <summary>
        /// 檢查使用者是否擁有指定電子禮券
        /// </summary>
        public async Task<bool> ValidateEVoucherOwnershipAsync(int voucherId, int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢 EVoucher WHERE EVoucherID = @voucherId AND UserID = @userId
            return true; // 暫時回傳有效
        }

        #region 私人輔助方法

        /// <summary>
        /// 取得使用者積分 - 查詢 User_Wallet
        /// </summary>
        private async Task<int> GetUserPointsAsync(int userId)
        {
            await Task.Delay(5);
            return 1250; // Stage 4 模擬積分
        }

        /// <summary>
        /// 取得使用者可用優惠券
        /// </summary>
        private async Task<List<CouponViewModel>> GetUserAvailableCouponsAsync(int userId, int limit)
        {
            await Task.Delay(5);
            return new List<CouponViewModel>
            {
                new CouponViewModel { CouponID = 1, CouponCode = "GAME50", IsUsed = false, AcquiredTime = DateTime.Now.AddDays(-3) },
                new CouponViewModel { CouponID = 2, CouponCode = "SHOP20", IsUsed = false, AcquiredTime = DateTime.Now.AddDays(-7) }
            };
        }

        /// <summary>
        /// 取得使用者可用電子禮券
        /// </summary>
        private async Task<List<EVoucherViewModel>> GetUserAvailableEVouchersAsync(int userId, int limit)
        {
            await Task.Delay(5);
            return new List<EVoucherViewModel>
            {
                new EVoucherViewModel { EVoucherID = 1, EVoucherCode = "EV100NT", IsUsed = false, AcquiredTime = DateTime.Now.AddDays(-5) }
            };
        }

        /// <summary>
        /// 計算使用者優惠券數量
        /// </summary>
        private async Task<int> CountUserCouponsAsync(int userId, bool isUsed)
        {
            await Task.Delay(5);
            return isUsed ? 3 : 5; // Stage 4 模擬數量
        }

        /// <summary>
        /// 計算使用者電子禮券數量
        /// </summary>
        private async Task<int> CountUserEVouchersAsync(int userId, bool isUsed)
        {
            await Task.Delay(5);
            return isUsed ? 1 : 2; // Stage 4 模擬數量
        }

        /// <summary>
        /// 取得本月積分統計
        /// </summary>
        private async Task<(int Earned, int Spent)> GetMonthlyStatsAsync(int userId)
        {
            await Task.Delay(5);
            return (480, 320); // Stage 4 模擬本月收支
        }

        /// <summary>
        /// 取得優惠券詳細資料
        /// </summary>
        private async Task<CouponViewModel?> GetCouponDetailsAsync(int couponId)
        {
            await Task.Delay(5);
            // Stage 4 模擬優惠券資料
            return new CouponViewModel 
            { 
                CouponID = couponId, 
                CouponCode = "TEST50", 
                IsUsed = false, 
                AcquiredTime = DateTime.Now.AddDays(-3) 
            };
        }

        /// <summary>
        /// 取得電子禮券詳細資料
        /// </summary>
        private async Task<EVoucherViewModel?> GetEVoucherDetailsAsync(int voucherId)
        {
            await Task.Delay(5);
            return new EVoucherViewModel 
            { 
                EVoucherID = voucherId, 
                EVoucherCode = "EV100NT", 
                IsUsed = false, 
                AcquiredTime = DateTime.Now.AddDays(-5) 
            };
        }

        /// <summary>
        /// 取得優惠券類型資料
        /// </summary>
        private async Task<CouponTypeViewModel?> GetCouponTypeAsync(int couponTypeId)
        {
            await Task.Delay(5);
            var types = await GetAvailableCouponTypesAsync();
            return types.FirstOrDefault(t => t.CouponTypeID == couponTypeId);
        }

        /// <summary>
        /// 取得電子禮券類型資料
        /// </summary>
        private async Task<EVoucherTypeViewModel?> GetEVoucherTypeAsync(int voucherTypeId)
        {
            await Task.Delay(5);
            var types = await GetAvailableEVoucherTypesAsync();
            return types.FirstOrDefault(t => t.EVoucherTypeID == voucherTypeId);
        }

        /// <summary>
        /// 產生優惠券代碼
        /// </summary>
        private string GenerateCouponCode(string typeName, int userId)
        {
            var prefix = typeName.Length > 4 ? typeName.Substring(0, 4).ToUpper() : typeName.ToUpper();
            var timestamp = DateTime.Now.ToString("MMdd");
            var random = Random.Shared.Next(100, 999);
            return $"{prefix}{timestamp}{random}";
        }

        /// <summary>
        /// 產生電子禮券代碼
        /// </summary>
        private string GenerateEVoucherCode(string typeName, int userId)
        {
            var prefix = "EV";
            var valuePrefix = typeName.Contains("100") ? "100" : "50";
            var timestamp = DateTime.Now.ToString("MMdd");
            var random = Random.Shared.Next(1000, 9999);
            return $"{prefix}{valuePrefix}{timestamp}{random}";
        }

        /// <summary>
        /// 產生安全令牌
        /// </summary>
        private string GenerateSecureToken()
        {
            return Guid.NewGuid().ToString("N")[..32].ToUpper(); // 32位元安全令牌
        }

        #region 模擬資料庫操作方法

        private async Task SimulateCouponUseAsync(int couponId, DateTime usedTime, int? orderId)
        {
            await Task.Delay(5);
            _logger.LogInformation("模擬優惠券使用更新 - CouponId: {CouponId}, UsedTime: {UsedTime}", couponId, usedTime);
        }

        private async Task<int> SimulateCouponCreateAsync(string couponCode, int couponTypeId, int userId, DateTime acquiredTime)
        {
            await Task.Delay(5);
            var newId = Random.Shared.Next(10000, 99999);
            _logger.LogInformation("模擬優惠券建立 - Code: {Code}, UserId: {UserId}", couponCode, userId);
            return newId;
        }

        private async Task SimulateEVoucherUseAsync(int voucherId, DateTime usedTime)
        {
            await Task.Delay(5);
            _logger.LogInformation("模擬電子禮券使用更新 - VoucherId: {VoucherId}, UsedTime: {UsedTime}", voucherId, usedTime);
        }

        private async Task<int> SimulateEVoucherCreateAsync(string voucherCode, int voucherTypeId, int userId, DateTime acquiredTime)
        {
            await Task.Delay(5);
            var newId = Random.Shared.Next(10000, 99999);
            _logger.LogInformation("模擬電子禮券建立 - Code: {Code}, UserId: {UserId}", voucherCode, userId);
            return newId;
        }

        private async Task SimulateEVoucherTokenCreateAsync(int voucherId, string token, DateTime expiresAt)
        {
            await Task.Delay(5);
            _logger.LogInformation("模擬電子禮券令牌建立 - VoucherId: {VoucherId}, Token: {Token}", voucherId, token[..8] + "...");
        }

        private async Task SimulateEVoucherRedeemLogAsync(int voucherId, int userId, DateTime scannedAt)
        {
            await Task.Delay(5);
            _logger.LogInformation("模擬電子禮券兌換記錄 - VoucherId: {VoucherId}, UserId: {UserId}", voucherId, userId);
        }

        private async Task SimulateUserPointsDeductAsync(int userId, int pointsToDeduct)
        {
            await Task.Delay(5);
            _logger.LogInformation("模擬使用者積分扣除 - UserId: {UserId}, PointsDeducted: {Points}", userId, pointsToDeduct);
        }

        private async Task SimulateWalletHistoryInsertAsync(int userId, string changeType, int pointsChanged, string? itemCode, string description, DateTime changeTime)
        {
            await Task.Delay(5);
            _logger.LogInformation("模擬錢包歷史記錄插入 - UserId: {UserId}, ChangeType: {ChangeType}, Points: {Points}", userId, changeType, pointsChanged);
        }

        #endregion
    }
}