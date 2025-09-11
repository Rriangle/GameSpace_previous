using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 使用者簽到服務介面 - 處理簽到邏輯與獎勵計算
    /// 對應 database.sql UserSignInStats 資料表操作
    /// </summary>
    public interface IUserSignInService
    {
        /// <summary>
        /// 執行使用者簽到作業 - 記錄簽到時間並計算獎勵
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>簽到結果包含獎勵資訊</returns>
        Task<SignInResultViewModel> ProcessSignInAsync(int userId);

        /// <summary>
        /// 檢查今日是否已簽到
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>是否已簽到</returns>
        Task<bool> HasSignedTodayAsync(int userId);

        /// <summary>
        /// 取得連續簽到天數
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>連續簽到天數</returns>
        Task<int> GetConsecutiveDaysAsync(int userId);

        /// <summary>
        /// 取得使用者簽到統計資料
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <param name="days">查詢天數範圍</param>
        /// <returns>簽到統計顯示模型</returns>
        Task<SignInStatsDisplayViewModel> GetSignInStatsAsync(int userId, int days = 30);
    }

    /// <summary>
    /// 簽到結果視圖模型 - 包含簽到成功後的獎勵資訊
    /// </summary>
    public class SignInResultViewModel
    {
        /// <summary>
        /// 是否簽到成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 結果訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 獲得積分
        /// </summary>
        public int PointsGained { get; set; }

        /// <summary>
        /// 獲得經驗值
        /// </summary>
        public int ExpGained { get; set; }

        /// <summary>
        /// 連續簽到天數
        /// </summary>
        public int ConsecutiveDays { get; set; }

        /// <summary>
        /// 獎勵優惠券代碼
        /// </summary>
        public string? BonusCouponCode { get; set; }

        /// <summary>
        /// 是否有特殊獎勵
        /// </summary>
        public bool HasBonusReward { get; set; }

        /// <summary>
        /// 特殊獎勵描述
        /// </summary>
        public string? BonusDescription { get; set; }

        /// <summary>
        /// 簽到時間
        /// </summary>
        public DateTime SignInTime { get; set; }

        /// <summary>
        /// 更新後的總積分
        /// </summary>
        public int TotalPoints { get; set; }
    }
}