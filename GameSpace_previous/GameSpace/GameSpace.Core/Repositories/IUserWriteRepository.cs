using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 用戶相關寫入存儲庫
    /// </summary>
    public interface IUserWriteRepository
    {
        /// <summary>
        /// 處理用戶簽到
        /// </summary>
        Task<SignInResponse> ProcessSignInAsync(SignInRequest request);

        /// <summary>
        /// 更新用戶錢包
        /// </summary>
        Task<bool> UpdateUserWalletAsync(int userId, int pointsChange, string description);

        /// <summary>
        /// 添加錢包歷史記錄
        /// </summary>
        Task<bool> AddWalletHistoryAsync(int userId, int pointsChange, string description, string transactionType);

        /// <summary>
        /// 更新寵物經驗值和等級
        /// </summary>
        Task<bool> UpdatePetExpAsync(int userId, int expGained);

        /// <summary>
        /// 兌換優惠券
        /// </summary>
        Task<bool> RedeemCouponAsync(int userId, int couponId);

        /// <summary>
        /// 兌換禮券
        /// </summary>
        Task<bool> RedeemEVoucherAsync(int userId, int evoucherId);

        /// <summary>
        /// 檢查冪等性金鑰是否已使用
        /// </summary>
        Task<bool> IsIdempotencyKeyUsedAsync(string idempotencyKey);
    }
}
