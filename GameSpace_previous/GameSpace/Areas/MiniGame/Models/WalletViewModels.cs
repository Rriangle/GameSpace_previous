using System.ComponentModel.DataAnnotations;

namespace GameSpace.Areas.MiniGame.Models
{
    /// <summary>
    /// 錢包交易歷史視圖模型 - 對應 database.sql WalletHistory 資料表
    /// </summary>
    public class WalletHistoryViewModel
    {
        /// <summary>
        /// 紀錄編號
        /// </summary>
        public int LogID { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 變更類型（獲得/消費）
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ChangeType { get; set; } = string.Empty;
        
        /// <summary>
        /// 積分變更數量
        /// </summary>
        public int PointsChanged { get; set; }
        
        /// <summary>
        /// 項目代碼
        /// </summary>
        [StringLength(50)]
        public string? ItemCode { get; set; }
        
        /// <summary>
        /// 變更描述
        /// </summary>
        [StringLength(255)]
        public string? Description { get; set; }
        
        /// <summary>
        /// 變更時間
        /// </summary>
        public DateTime ChangeTime { get; set; }
    }

    /// <summary>
    /// 優惠券類型視圖模型 - 對應 database.sql CouponType 資料表
    /// </summary>
    public class CouponTypeViewModel
    {
        /// <summary>
        /// 優惠券類型編號
        /// </summary>
        public int CouponTypeID { get; set; }
        
        /// <summary>
        /// 優惠券類型名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 折扣類型（percentage/amount）
        /// </summary>
        [Required]
        [StringLength(10)]
        public string DiscountType { get; set; } = string.Empty;
        
        /// <summary>
        /// 折扣數值
        /// </summary>
        public decimal DiscountValue { get; set; }
        
        /// <summary>
        /// 最低消費金額
        /// </summary>
        public decimal MinSpend { get; set; }
        
        /// <summary>
        /// 有效期限開始
        /// </summary>
        public DateTime ValidFrom { get; set; }
        
        /// <summary>
        /// 有效期限結束
        /// </summary>
        public DateTime ValidTo { get; set; }
        
        /// <summary>
        /// 兌換所需積分
        /// </summary>
        public int PointsCost { get; set; }
        
        /// <summary>
        /// 優惠券描述
        /// </summary>
        [StringLength(255)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// 優惠券視圖模型 - 對應 database.sql Coupon 資料表
    /// </summary>
    public class CouponViewModel
    {
        /// <summary>
        /// 優惠券編號
        /// </summary>
        public int CouponID { get; set; }
        
        /// <summary>
        /// 優惠券代碼
        /// </summary>
        [Required]
        [StringLength(20)]
        public string CouponCode { get; set; } = string.Empty;
        
        /// <summary>
        /// 優惠券類型編號
        /// </summary>
        public int CouponTypeID { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; }
        
        /// <summary>
        /// 獲得時間
        /// </summary>
        public DateTime AcquiredTime { get; set; }
        
        /// <summary>
        /// 使用時間
        /// </summary>
        public DateTime? UsedTime { get; set; }
        
        /// <summary>
        /// 使用於訂單編號
        /// </summary>
        public int? UsedInOrderID { get; set; }
        
        /// <summary>
        /// 優惠券類型詳情（Join 查詢用）
        /// </summary>
        public CouponTypeViewModel? CouponType { get; set; }
    }

    /// <summary>
    /// 電子禮券類型視圖模型 - 對應 database.sql EVoucherType 資料表
    /// </summary>
    public class EVoucherTypeViewModel
    {
        /// <summary>
        /// 電子禮券類型編號
        /// </summary>
        public int EVoucherTypeID { get; set; }
        
        /// <summary>
        /// 電子禮券類型名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 面值金額
        /// </summary>
        public decimal ValueAmount { get; set; }
        
        /// <summary>
        /// 有效期限開始
        /// </summary>
        public DateTime ValidFrom { get; set; }
        
        /// <summary>
        /// 有效期限結束
        /// </summary>
        public DateTime ValidTo { get; set; }
        
        /// <summary>
        /// 兌換所需積分
        /// </summary>
        public int PointsCost { get; set; }
        
        /// <summary>
        /// 總可用數量
        /// </summary>
        public int TotalAvailable { get; set; }
        
        /// <summary>
        /// 電子禮券描述
        /// </summary>
        [StringLength(255)]
        public string? Description { get; set; }
    }

    /// <summary>
    /// 電子禮券視圖模型 - 對應 database.sql EVoucher 資料表
    /// </summary>
    public class EVoucherViewModel
    {
        /// <summary>
        /// 電子禮券編號
        /// </summary>
        public int EVoucherID { get; set; }
        
        /// <summary>
        /// 電子禮券代碼
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EVoucherCode { get; set; } = string.Empty;
        
        /// <summary>
        /// 電子禮券類型編號
        /// </summary>
        public int EVoucherTypeID { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; }
        
        /// <summary>
        /// 獲得時間
        /// </summary>
        public DateTime AcquiredTime { get; set; }
        
        /// <summary>
        /// 使用時間
        /// </summary>
        public DateTime? UsedTime { get; set; }
        
        /// <summary>
        /// 電子禮券類型詳情（Join 查詢用）
        /// </summary>
        public EVoucherTypeViewModel? EVoucherType { get; set; }
    }

    /// <summary>
    /// 電子禮券令牌視圖模型 - 對應 database.sql EVoucherToken 資料表
    /// </summary>
    public class EVoucherTokenViewModel
    {
        /// <summary>
        /// 令牌編號
        /// </summary>
        public int TokenID { get; set; }
        
        /// <summary>
        /// 電子禮券編號
        /// </summary>
        public int EVoucherID { get; set; }
        
        /// <summary>
        /// 令牌字串
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// 到期時間
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
        /// 是否已撤銷
        /// </summary>
        public bool IsRevoked { get; set; }
    }

    /// <summary>
    /// 電子禮券兌換記錄視圖模型 - 對應 database.sql EVoucherRedeemLog 資料表
    /// </summary>
    public class EVoucherRedeemLogViewModel
    {
        /// <summary>
        /// 兌換編號
        /// </summary>
        public int RedeemID { get; set; }
        
        /// <summary>
        /// 電子禮券編號
        /// </summary>
        public int EVoucherID { get; set; }
        
        /// <summary>
        /// 令牌編號
        /// </summary>
        public int? TokenID { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 掃描時間
        /// </summary>
        public DateTime ScannedAt { get; set; }
        
        /// <summary>
        /// 兌換狀態
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}