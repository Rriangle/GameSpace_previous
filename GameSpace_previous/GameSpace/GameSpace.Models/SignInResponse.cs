namespace GameSpace.Models
{
    /// <summary>
    /// 簽到回應模型 - 已擴展為 Stage 3 需求
    /// </summary>
    public class SignInResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 獲得的點數
        /// </summary>
        public int PointsEarned { get; set; }

        /// <summary>
        /// 獲得的經驗值
        /// </summary>
        public int ExpEarned { get; set; }

        /// <summary>
        /// 連續簽到天數
        /// </summary>
        public int ConsecutiveDays { get; set; }

        /// <summary>
        /// 是否獲得額外獎勵
        /// </summary>
        public bool HasBonusReward { get; set; }

        /// <summary>
        /// 獎勵描述
        /// </summary>
        public string BonusDescription { get; set; } = string.Empty;

        // ===== Stage 3 擴展欄位 =====

        /// <summary>
        /// 簽到時間（Stage 3 新增）
        /// </summary>
        public DateTime SignInTime { get; set; }

        /// <summary>
        /// 獲得的積分（Stage 3 新增，與 PointsEarned 同義但可寫入）
        /// </summary>
        public int PointsGained { get; set; }

        /// <summary>
        /// 獲得的經驗值（Stage 3 新增，與 ExpEarned 同義但可寫入）
        /// </summary>
        public int ExpGained { get; set; }

        /// <summary>
        /// 獲得的優惠券代碼（如果有）（Stage 3 新增）
        /// </summary>
        public string? CouponGained { get; set; }

        /// <summary>
        /// 簽到後的總積分（Stage 3 新增）
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// 冪等性密鑰（Stage 3 新增）
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}
