namespace GameSpace.Models
{
    /// <summary>
    /// 錢包總覽聚合讀取模型 - Stage 2 廣度切片
    /// 整合用戶積分、優惠券、電子禮券資訊
    /// </summary>
    public class WalletOverviewReadModel
    {
        /// <summary>
        /// 用戶 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用戶名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 當前積分餘額
        /// </summary>
        public int CurrentPoints { get; set; }

        /// <summary>
        /// 可用優惠券數量
        /// </summary>
        public int AvailableCouponsCount { get; set; }

        /// <summary>
        /// 已使用優惠券數量
        /// </summary>
        public int UsedCouponsCount { get; set; }

        /// <summary>
        /// 可用電子禮券數量
        /// </summary>
        public int AvailableEVouchersCount { get; set; }

        /// <summary>
        /// 已使用電子禮券數量
        /// </summary>
        public int UsedEVouchersCount { get; set; }

        /// <summary>
        /// 最近 10 筆錢包異動記錄
        /// </summary>
        public List<WalletHistoryReadModel> RecentTransactions { get; set; } = new List<WalletHistoryReadModel>();

        /// <summary>
        /// 可用優惠券列表
        /// </summary>
        public List<CouponOverviewReadModel> AvailableCoupons { get; set; } = new List<CouponOverviewReadModel>();

        /// <summary>
        /// 可用電子禮券列表
        /// </summary>
        public List<EVoucherOverviewReadModel> AvailableEVouchers { get; set; } = new List<EVoucherOverviewReadModel>();
    }

    /// <summary>
    /// 優惠券總覽讀取模型
    /// </summary>
    public class CouponOverviewReadModel
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string CouponTypeName { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinSpend { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime AcquiredTime { get; set; }
    }

    /// <summary>
    /// 電子禮券總覽讀取模型
    /// </summary>
    public class EVoucherOverviewReadModel
    {
        public int EVoucherId { get; set; }
        public string EVoucherCode { get; set; } = string.Empty;
        public string EVoucherTypeName { get; set; } = string.Empty;
        public decimal ValueAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime AcquiredTime { get; set; }
    }
}
