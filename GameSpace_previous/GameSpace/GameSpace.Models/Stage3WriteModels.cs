namespace GameSpace.Models
{
    /// <summary>
    /// �����ʰO���ҫ� - Stage 3 �g�J�ާ@
    /// �Ω�l�ܾ����ʾާ@
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
    /// ñ��έp�ҫ� - Stage 3 �g�J�ާ@
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
    /// �d���ݩʧ�s�ШD�ҫ� - Stage 3 �d������
    /// </summary>
    public class PetUpdateRequest
    {
        /// <summary>
        /// �d�� ID
        /// </summary>
        public int PetId { get; set; }

        /// <summary>
        /// �Τ� ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// �����ʱK�_
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// �n��s���ݩ�
        /// </summary>
        public PetAttributeUpdate Attributes { get; set; } = new PetAttributeUpdate();
    }

    /// <summary>
    /// �d���ݩʧ�s
    /// </summary>
    public class PetAttributeUpdate
    {
        /// <summary>
        /// �Ⱦj���ܤ�
        /// </summary>
        public int? HungerDelta { get; set; }

        /// <summary>
        /// �߱��ܤ�
        /// </summary>
        public int? MoodDelta { get; set; }

        /// <summary>
        /// ��O�ܤ�
        /// </summary>
        public int? StaminaDelta { get; set; }

        /// <summary>
        /// �M����ܤ�
        /// </summary>
        public int? CleanlinessDelta { get; set; }

        /// <summary>
        /// ���d���ܤ�
        /// </summary>
        public int? HealthDelta { get; set; }

        /// <summary>
        /// �g����ܤ�
        /// </summary>
        public int? ExperienceDelta { get; set; }
    }

    /// <summary>
    /// �d����s�T���ҫ�
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
    /// �u�f��I���ШD�ҫ� - Stage 3 �I������
    /// </summary>
    public class CouponRedeemRequest
    {
        /// <summary>
        /// �u�f��N�X
        /// </summary>
        public string CouponCode { get; set; } = string.Empty;

        /// <summary>
        /// �Τ� ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// �����ʱK�_
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// �q�� ID�]�p�G�Ω�q��^
        /// </summary>
        public int? OrderId { get; set; }
    }

    /// <summary>
    /// �u�f��I���T���ҫ�
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
    /// �f�p��x�ҫ� - Stage 3 �f�p�\��
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
