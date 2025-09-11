namespace GameSpace.Models
{
    /// <summary>
    /// 冪等性記錄模型 - Stage 3 寫入操作
    /// 用於追蹤冪等性操作
    /// </summary>
    public class IdempotencyRecord
    {
        public string IdempotencyKey { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string? ResponseData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// 簽到統計模型 - Stage 3 寫入操作
    /// </summary>
    public class SignInStats
    {
        public int UserId { get; set; }
        public DateTime LastSignInDate { get; set; }
        public int ConsecutiveDays { get; set; }
        public int TotalSignIns { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// 寵物屬性更新請求模型 - Stage 3 寵物切片
    /// </summary>
    public class PetUpdateRequest
    {
        /// <summary>
        /// 寵物 ID
        /// </summary>
        public int PetId { get; set; }

        /// <summary>
        /// 用戶 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 冪等性密鑰
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// 要更新的屬性
        /// </summary>
        public PetAttributeUpdate Attributes { get; set; } = new PetAttributeUpdate();
    }

    /// <summary>
    /// 寵物屬性更新
    /// </summary>
    public class PetAttributeUpdate
    {
        /// <summary>
        /// 饑餓度變化
        /// </summary>
        public int? HungerDelta { get; set; }

        /// <summary>
        /// 心情變化
        /// </summary>
        public int? MoodDelta { get; set; }

        /// <summary>
        /// 體力變化
        /// </summary>
        public int? StaminaDelta { get; set; }

        /// <summary>
        /// 清潔度變化
        /// </summary>
        public int? CleanlinessDelta { get; set; }

        /// <summary>
        /// 健康度變化
        /// </summary>
        public int? HealthDelta { get; set; }

        /// <summary>
        /// 經驗值變化
        /// </summary>
        public int? ExperienceDelta { get; set; }
    }

    /// <summary>
    /// 寵物更新響應模型
    /// </summary>
    public class PetUpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool LeveledUp { get; set; }
        public int? NewLevel { get; set; }
        public PetReadModel? UpdatedPet { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// 優惠券兌換請求模型 - Stage 3 兌換切片
    /// </summary>
    public class CouponRedeemRequest
    {
        /// <summary>
        /// 優惠券代碼
        /// </summary>
        public string CouponCode { get; set; } = string.Empty;

        /// <summary>
        /// 用戶 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 冪等性密鑰
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// 訂單 ID（如果用於訂單）
        /// </summary>
        public int? OrderId { get; set; }
    }

    /// <summary>
    /// 優惠券兌換響應模型
    /// </summary>
    public class CouponRedeemResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public DateTime RedeemedAt { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// 審計日誌模型 - Stage 3 審計功能
    /// </summary>
    public class AuditLog
    {
        public long AuditId { get; set; }
        public int UserId { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
