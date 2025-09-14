namespace GameSpace.Models
{
    /// <summary>
    /// �u�f�������޲z�ШD�ҫ� - Stage 4 �޲z��x
    /// </summary>
    public class CouponTypeManageRequest
    {
        /// <summary>
        /// �u�f������ ID�]��s�ɨϥΡ^
        /// </summary>
        public int? CouponTypeId { get; set; }

        /// <summary>
        /// �W��
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// �馩�����]percentage, fixed�^
        /// </summary>
        public string DiscountType { get; set; } = string.Empty;

        /// <summary>
        /// �馩��
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// �̧C���O���B
        /// </summary>
        public decimal MinSpend { get; set; }

        /// <summary>
        /// ���Ķ}�l�ɶ�
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// ���ĵ����ɶ�
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// �n������
        /// </summary>
        public int PointsCost { get; set; }

        /// <summary>
        /// �y�z
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// �O�_�ҥ�
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// �޲z�ާ@�T���ҫ�
    /// </summary>
    public class AdminOperationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// �T�ε��L�o�ШD�ҫ� - Stage 4 ���e�v�z
    /// </summary>
    public class ContentFilterRequest
    {
        /// <summary>
        /// �n�ˬd�����e
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// ���e�����]forum_post, comment, etc.�^
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// �Τ� ID
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// ���e�L�o�T���ҫ�
    /// </summary>
    public class ContentFilterResponse
    {
        public bool IsAllowed { get; set; }
        public string FilteredContent { get; set; } = string.Empty;
        public List<string> DetectedWords { get; set; } = new List<string>();
        public string FilterAction { get; set; } = string.Empty; // block, mask, warn
    }

    /// <summary>
    /// �}��ƾڼҫ� - Stage 4 �uŪ�}��
    /// </summary>
    public class InsightsData
    {
        /// <summary>
        /// �`�Τ��
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// ���D�Τ�ơ]�L�h 30 �ѡ^
        /// </summary>
        public int ActiveUsers { get; set; }

        /// <summary>
        /// �`ñ�즸��
        /// </summary>
        public int TotalSignIns { get; set; }

        /// <summary>
        /// �w�o���u�f��ƶq
        /// </summary>
        public int IssuedCoupons { get; set; }

        /// <summary>
        /// �w�I���u�f��ƶq
        /// </summary>
        public int RedeemedCoupons { get; set; }

        /// <summary>
        /// �׾¶K��ƶq
        /// </summary>
        public int ForumPosts { get; set; }

        /// <summary>
        /// �L�o�����e�ƶq
        /// </summary>
        public int FilteredContent { get; set; }

        /// <summary>
        /// �ƾڥͦ��ɶ�
        /// </summary>
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// �޲z�������v���ҫ� - Stage 4 �� RBAC
    /// </summary>
    public class AdminRole
    {
        public string RoleName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
    }

    /// <summary>
    /// �޲z���Τ�ҫ�
    /// </summary>
    public class AdminUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
    }
}
