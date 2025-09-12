using GameSpace.Models;

namespace GameSpace.Services.Wallet
{
    public interface IWalletService
    {
        Task<WalletResult> GetWalletAsync(int userId);
        Task<WalletResult> AddPointsAsync(int userId, int points, string description);
        Task<WalletResult> DeductPointsAsync(int userId, int points, string description);
        Task<WalletResult> TransferPointsAsync(int fromUserId, int toUserId, int points, string description);
        Task<List<WalletHistory>> GetWalletHistoryAsync(int userId, int page = 1, int pageSize = 20);
        Task<WalletResult> RedeemCouponAsync(int userId, int couponTypeId);
        Task<WalletResult> RedeemEVoucherAsync(int userId, int evoucherTypeId);
        Task<List<Coupon>> GetUserCouponsAsync(int userId, bool includeUsed = false);
        Task<List<Evoucher>> GetUserEVouchersAsync(int userId, bool includeUsed = false);
        Task<WalletResult> UseCouponAsync(int userId, string couponCode, int orderId);
        Task<WalletResult> UseEVoucherAsync(int userId, string evoucherCode);
    }

    public class WalletResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public UserWallet? Wallet { get; set; }
        public List<WalletHistory>? History { get; set; }
        public Coupon? Coupon { get; set; }
        public Evoucher? EVoucher { get; set; }
    }
}