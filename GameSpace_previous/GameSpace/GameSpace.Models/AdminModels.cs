namespace GameSpace.Models
{
    /// <summary>
    /// 優惠券類型管理請求模型 - Stage 4 管理後台
    /// </summary>
    public class CouponTypeManageRequest
    {
        /// <summary>
        /// 優惠券類型 ID（更新時使用）
        /// </summary>
        public int? CouponTypeId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 折扣類型（percentage, fixed）
        /// </summary>
        public string DiscountType { get; set; } = string.Empty;

        /// <summary>
        /// 折扣值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 最低消費金額
        /// </summary>
        public decimal MinSpend { get; set; }

        /// <summary>
        /// 有效開始時間
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// 有效結束時間
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// 積分成本
        /// </summary>
        public int PointsCost { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 管理操作響應模型
    /// </summary>
    public class AdminOperationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// 禁用詞過濾請求模型 - Stage 4 內容治理
    /// </summary>
    public class ContentFilterRequest
    {
        /// <summary>
        /// 要檢查的內容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 內容類型（forum_post, comment, etc.）
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// 用戶 ID
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// 內容過濾響應模型
    /// </summary>
    public class ContentFilterResponse
    {
        public bool IsAllowed { get; set; }
        public string FilteredContent { get; set; } = string.Empty;
        public List<string> DetectedWords { get; set; } = new List<string>();
        public string FilterAction { get; set; } = string.Empty; // block, mask, warn
    }

    /// <summary>
    /// 洞察數據模型 - Stage 4 只讀洞察
    /// </summary>
    public class InsightsData
    {
        /// <summary>
        /// 總用戶數
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// 活躍用戶數（過去 30 天）
        /// </summary>
        public int ActiveUsers { get; set; }

        /// <summary>
        /// 總簽到次數
        /// </summary>
        public int TotalSignIns { get; set; }

        /// <summary>
        /// 已發放優惠券數量
        /// </summary>
        public int IssuedCoupons { get; set; }

        /// <summary>
        /// 已兌換優惠券數量
        /// </summary>
        public int RedeemedCoupons { get; set; }

        /// <summary>
        /// 論壇貼文數量
        /// </summary>
        public int ForumPosts { get; set; }

        /// <summary>
        /// 過濾的內容數量
        /// </summary>
        public int FilteredContent { get; set; }

        /// <summary>
        /// 數據生成時間
        /// </summary>
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// 管理員角色權限模型 - Stage 4 基本 RBAC
    /// </summary>
    public class AdminRole
    {
        public string RoleName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
    }

    /// <summary>
    /// 管理員用戶模型
    /// </summary>
    public class AdminUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
    }
}
