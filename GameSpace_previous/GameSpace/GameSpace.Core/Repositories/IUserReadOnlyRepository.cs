using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 用戶相關只讀存儲庫
    /// </summary>
    public interface IUserReadOnlyRepository
    {
        Task<UserReadModel?> GetUserByIdAsync(int userId);
        Task<UserReadModel?> GetUserByAccountAsync(string account);
        Task<UserIntroduceReadModel?> GetUserIntroduceByIdAsync(int userId);
        Task<UserRightsReadModel?> GetUserRightsByIdAsync(int userId);
        Task<UserWalletReadModel?> GetUserWalletByIdAsync(int userId);
        Task<PetReadModel?> GetPetByUserIdAsync(int userId);
        Task<List<UserSignInStatsReadModel>> GetUserSignInStatsAsync(int userId, int days = 30);
        Task<List<MiniGameReadModel>> GetUserMiniGamesAsync(int userId, int limit = 10);
        Task<List<CouponReadModel>> GetUserCouponsAsync(int userId, bool? isUsed = null);
        Task<List<EVoucherReadModel>> GetUserEVouchersAsync(int userId, bool? isUsed = null);
        Task<List<WalletHistoryReadModel>> GetUserWalletHistoryAsync(int userId, int limit = 50);
        Task<List<UserReadModel>> GetUsersAsync(int page = 1, int pageSize = 20);
        Task<int> GetUserCountAsync();
    }
}
