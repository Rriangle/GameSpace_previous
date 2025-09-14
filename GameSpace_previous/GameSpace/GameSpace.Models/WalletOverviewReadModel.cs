namespace GameSpace.Models
{
    /// <summary>
    /// ���]�`���E�XŪ���ҫ� - Stage 2 �s�פ���
    /// ��X�Τ�n���B�u�f��B�q�l§���T
    /// </summary>
    public class WalletOverviewReadModel
    {
        /// <summary>
        /// �Τ� ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// �Τ�W��
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// ��e�n���l�B
        /// </summary>
        public int CurrentPoints { get; set; }

        /// <summary>
        /// �i���u�f��ƶq
        /// </summary>
        public int AvailableCouponsCount { get; set; }

        /// <summary>
        /// �w�ϥ��u�f��ƶq
        /// </summary>
        public int UsedCouponsCount { get; set; }

        /// <summary>
        /// �i�ιq�l§��ƶq
        /// </summary>
        public int AvailableEVouchersCount { get; set; }

        /// <summary>
        /// �w�ϥιq�l§��ƶq
        /// </summary>
        public int UsedEVouchersCount { get; set; }

        /// <summary>
        /// �̪� 10 �����]���ʰO��
        /// </summary>
        public List<WalletHistoryReadModel> RecentTransactions { get; set; } = new List<WalletHistoryReadModel>();

        /// <summary>
        /// �i���u�f��C��
        /// </summary>
        public List<CouponOverviewReadModel> AvailableCoupons { get; set; } = new List<CouponOverviewReadModel>();

        /// <summary>
        /// �i�ιq�l§��C��
        /// </summary>
        public List<EVoucherOverviewReadModel> AvailableEVouchers { get; set; } = new List<EVoucherOverviewReadModel>();
    }

    /// <summary>
    /// �u�f���`��Ū���ҫ�
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
    /// �q�l§���`��Ū���ҫ�
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
