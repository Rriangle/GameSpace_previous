using GameSpace.Models;

namespace GameSpace.Areas.MiniGame.ViewModels
{
    /// <summary>
    /// 錢包列表項目 ViewModel
    /// </summary>
    public class WalletListItemViewModel
    {
        public int UserId { get; set; }
        public string UserAccount { get; set; } = string.Empty;
        public string UserNickName { get; set; } = string.Empty;
        public int UserPoint { get; set; }
        public int CouponCount { get; set; }
        public int EVoucherCount { get; set; }
    }

    /// <summary>
    /// 錢包列表頁 ViewModel
    /// </summary>
    public class WalletIndexViewModel
    {
        public List<WalletListItemViewModel> Wallets { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public int? MinPoints { get; set; }
        public int? MaxPoints { get; set; }
    }

    /// <summary>
    /// 錢包詳細頁 ViewModel
    /// </summary>
    public class WalletDetailsViewModel
    {
        public UserWallet Wallet { get; set; } = new();
        public List<WalletHistory> Histories { get; set; } = new();
        public List<Coupon> AvailableCoupons { get; set; } = new();
        public List<Evoucher> AvailableEVouchers { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalHistoryCount { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 優惠券列表頁 ViewModel
    /// </summary>
    public class CouponIndexViewModel
    {
        public List<Coupon> Coupons { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsUsed { get; set; }
    }

    /// <summary>
    /// 禮券列表頁 ViewModel
    /// </summary>
    public class EVoucherIndexViewModel
    {
        public List<Evoucher> EVouchers { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsUsed { get; set; }
    }

    /// <summary>
    /// 錢包類型管理首頁 ViewModel
    /// </summary>
    public class WalletTypesIndexViewModel
    {
        public int CouponTypesCount { get; set; }
        public int EVoucherTypesCount { get; set; }
    }

    /// <summary>
    /// 優惠券類型列表頁 ViewModel
    /// </summary>
    public class CouponTypeIndexViewModel
    {
        public List<CouponType> CouponTypes { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 優惠券類型詳細頁 ViewModel
    /// </summary>
    public class CouponTypeDetailsViewModel
    {
        public CouponType CouponType { get; set; } = new();
        public int TotalIssued { get; set; }
        public int UsedCount { get; set; }
        public int UnusedCount { get; set; }
    }

    /// <summary>
    /// 禮券類型列表頁 ViewModel
    /// </summary>
    public class EVoucherTypeIndexViewModel
    {
        public List<EvoucherType> EVoucherTypes { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 禮券類型詳細頁 ViewModel
    /// </summary>
    public class EVoucherTypeDetailsViewModel
    {
        public EvoucherType EVoucherType { get; set; } = new();
        public int TotalIssued { get; set; }
        public int UsedCount { get; set; }
        public int UnusedCount { get; set; }
    }
}