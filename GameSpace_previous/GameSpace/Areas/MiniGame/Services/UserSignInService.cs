using GameSpace.Areas.MiniGame.Models;
using Microsoft.Extensions.Logging;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 使用者簽到服務實作 - 處理簽到邏輯與獎勵計算
    /// 對應 database.sql UserSignInStats、User_Wallet 資料表操作
    /// </summary>
    public class UserSignInService : IUserSignInService
    {
        private readonly ILogger<UserSignInService> _logger;

        public UserSignInService(ILogger<UserSignInService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 執行使用者簽到作業 - 記錄簽到時間並計算獎勵
        /// 實際實作中會寫入 UserSignInStats 與更新 User_Wallet 資料表
        /// </summary>
        public async Task<SignInResultViewModel> ProcessSignInAsync(int userId)
        {
            try
            {
                // 檢查今日是否已簽到
                var hasSignedToday = await HasSignedTodayAsync(userId);
                if (hasSignedToday)
                {
                    return new SignInResultViewModel
                    {
                        Success = false,
                        Message = "您今日已經簽到過了",
                        SignInTime = DateTime.Now
                    };
                }

                // 取得連續簽到天數
                var consecutiveDays = await GetConsecutiveDaysAsync(userId) + 1;
                var signInTime = DateTime.Now;

                // 計算基礎獎勵 - 基於連續天數
                var basePoints = 10;
                var baseExp = 5;
                var bonusMultiplier = Math.Min(consecutiveDays / 7, 3); // 每週增加獎勵，最多3倍

                var pointsGained = basePoints + (bonusMultiplier * 5);
                var expGained = baseExp + (bonusMultiplier * 2);

                // 特殊獎勵邏輯 - 每7天、30天有額外獎勵
                string? bonusCouponCode = null;
                string? bonusDescription = null;
                var hasBonusReward = false;

                if (consecutiveDays % 30 == 0)
                {
                    bonusCouponCode = $"MONTH30_{DateTime.Now:yyyyMM}";
                    bonusDescription = "連續30天簽到獎勵";
                    pointsGained += 100;
                    hasBonusReward = true;
                }
                else if (consecutiveDays % 7 == 0)
                {
                    bonusCouponCode = $"WEEK7_{DateTime.Now:yyyyMMdd}";
                    bonusDescription = "連續7天簽到獎勵";
                    pointsGained += 25;
                    hasBonusReward = true;
                }

                // 模擬資料庫寫入操作 - Stage 4 階段暫時模擬，實際會使用 DbContext
                await SimulateUserSignInStatsInsertAsync(userId, signInTime, pointsGained, expGained, bonusCouponCode ?? string.Empty);
                await SimulateUserWalletUpdateAsync(userId, pointsGained);

                var totalPoints = await GetUserTotalPointsAsync(userId);

                _logger.LogInformation("使用者簽到成功 UserId: {UserId}, Points: {Points}, Exp: {Exp}, ConsecutiveDays: {ConsecutiveDays}", 
                    userId, pointsGained, expGained, consecutiveDays);

                return new SignInResultViewModel
                {
                    Success = true,
                    Message = $"簽到成功！連續簽到 {consecutiveDays} 天",
                    PointsGained = pointsGained,
                    ExpGained = expGained,
                    ConsecutiveDays = consecutiveDays,
                    BonusCouponCode = bonusCouponCode,
                    HasBonusReward = hasBonusReward,
                    BonusDescription = bonusDescription,
                    SignInTime = signInTime,
                    TotalPoints = totalPoints
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "使用者簽到失敗 UserId: {UserId}", userId);
                
                return new SignInResultViewModel
                {
                    Success = false,
                    Message = "簽到失敗，請稍後再試",
                    SignInTime = DateTime.Now
                };
            }
        }

        /// <summary>
        /// 檢查今日是否已簽到 - 查詢 UserSignInStats 最新記錄
        /// </summary>
        public async Task<bool> HasSignedTodayAsync(int userId)
        {
            // 模擬資料庫查詢 - 實際會檢查 UserSignInStats 資料表
            await Task.Delay(10); // 模擬異步作業
            
            // Stage 4 模擬邏輯 - 實際會查詢今日是否有 SignTime 記錄
            return false; // 暫時回傳未簽到狀態
        }

        /// <summary>
        /// 取得連續簽到天數 - 基於 UserSignInStats 計算
        /// </summary>
        public async Task<int> GetConsecutiveDaysAsync(int userId)
        {
            // 模擬資料庫查詢 - 實際會計算連續的 SignTime 記錄
            await Task.Delay(10); // 模擬異步作業
            
            // Stage 4 模擬邏輯 - 實際會基於日期連續性計算
            return 6; // 模擬目前連續6天
        }

        /// <summary>
        /// 取得使用者簽到統計資料
        /// </summary>
        public async Task<SignInStatsDisplayViewModel> GetSignInStatsAsync(int userId, int days = 30)
        {
            await Task.Delay(10); // 模擬異步作業

            var consecutiveDays = await GetConsecutiveDaysAsync(userId);
            var hasSignedToday = await HasSignedTodayAsync(userId);

            // 模擬統計資料計算 - 實際會從 UserSignInStats 聚合
            return new SignInStatsDisplayViewModel
            {
                UserId = userId,
                HasSignedToday = hasSignedToday,
                ConsecutiveDays = consecutiveDays,
                MonthlySignInDays = Math.Min(consecutiveDays, DateTime.Now.Day),
                TotalSignInDays = consecutiveDays + 100, // 模擬歷史累計
                TodayPointsReward = 10 + (consecutiveDays / 7 * 5),
                TodayExpReward = 5 + (consecutiveDays / 7 * 2),
                MonthlyPointsEarned = (Math.Min(consecutiveDays, DateTime.Now.Day)) * 10,
                MonthlyExpEarned = (Math.Min(consecutiveDays, DateTime.Now.Day)) * 5,
                RecentSignInStats = new List<UserSignInStatsViewModel>(),
                MonthlyCalendar = GenerateMonthlyCalendar(consecutiveDays)
            };
        }

        /// <summary>
        /// 模擬 UserSignInStats 資料表插入操作
        /// </summary>
        private async Task SimulateUserSignInStatsInsertAsync(int userId, DateTime signTime, int pointsGained, int expGained, string couponGained)
        {
            await Task.Delay(5); // 模擬資料庫寫入延遲
            
            // Stage 4 階段模擬 - 實際會執行以下 SQL:
            // INSERT INTO UserSignInStats (LogID, SignTime, UserID, PointsGained, PointsGainedTime, ExpGained, ExpGainedTime, CouponGained, CouponGainedTime)
            // VALUES (NEXT_ID, @signTime, @userId, @pointsGained, @signTime, @expGained, @signTime, @couponGained, @signTime)
            
            _logger.LogInformation("模擬簽到記錄插入 - UserId: {UserId}, SignTime: {SignTime}, Points: {Points}", 
                userId, signTime, pointsGained);
        }

        /// <summary>
        /// 模擬 User_Wallet 資料表更新操作
        /// </summary>
        private async Task SimulateUserWalletUpdateAsync(int userId, int pointsToAdd)
        {
            await Task.Delay(5); // 模擬資料庫寫入延遲
            
            // Stage 4 階段模擬 - 實際會執行以下 SQL:
            // UPDATE User_Wallet SET User_Point = User_Point + @pointsToAdd WHERE User_Id = @userId
            
            _logger.LogInformation("模擬錢包積分更新 - UserId: {UserId}, PointsAdded: {Points}", 
                userId, pointsToAdd);
        }

        /// <summary>
        /// 取得使用者總積分 - 查詢 User_Wallet 資料表
        /// </summary>
        private async Task<int> GetUserTotalPointsAsync(int userId)
        {
            await Task.Delay(5); // 模擬資料庫查詢延遲
            
            // Stage 4 階段模擬 - 實際會查詢 User_Wallet.User_Point
            return 1250; // 模擬目前積分
        }

        /// <summary>
        /// 產生月曆資料 - 基於連續簽到天數
        /// </summary>
        private Dictionary<int, bool> GenerateMonthlyCalendar(int consecutiveDays)
        {
            var calendar = new Dictionary<int, bool>();
            var today = DateTime.Now.Day;
            
            for (int day = 1; day <= today; day++)
            {
                // 模擬簽到狀態 - 實際會基於 UserSignInStats 查詢結果
                calendar[day] = day <= consecutiveDays && day <= today;
            }
            
            return calendar;
        }
    }
}