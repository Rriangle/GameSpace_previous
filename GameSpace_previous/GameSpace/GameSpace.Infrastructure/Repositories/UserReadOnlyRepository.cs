using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// 使用者唯讀專用倉儲實作 - Stage 2 實作階段
    /// 目前回傳假資料，待後續完整實作
    /// </summary>
    public class UserReadOnlyRepository : IUserReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public UserReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 透過用戶編號取得使用者基本資料
        /// </summary>
        public async Task<UserReadModel?> GetUserByIdAsync(int userId)
        {
            // 目前回傳假資料，待後續完整實作
            await Task.Delay(1); // 模擬異步作業
            
            return new UserReadModel
            {
                User_ID = userId,
                User_name = $"使用者{userId}",
                User_Account = $"user{userId}",
                User_EmailConfirmed = true,
                User_PhoneNumberConfirmed = false
            };
        }

        /// <summary>
        /// 透過帳號取得使用者資料
        /// </summary>
        public async Task<UserReadModel?> GetUserByAccountAsync(string account)
        {
            // 目前回傳假資料，待後續完整實作
            await Task.Delay(1); // 模擬異步作業
            
            return new UserReadModel
            {
                User_ID = 1,
                User_name = "測試使用者",
                User_Account = account,
                User_EmailConfirmed = true,
                User_PhoneNumberConfirmed = false
            };
        }

        /// <summary>
        /// 取得使用者介紹資料
        /// </summary>
        public async Task<UserIntroduceReadModel?> GetUserIntroduceByIdAsync(int userId)
        {
            await Task.Delay(1); // 模擬異步作業
            return null; // 暫時回傳 null
        }

        /// <summary>
        /// 取得使用者權限資料
        /// </summary>
        public async Task<UserRightsReadModel?> GetUserRightsByIdAsync(int userId)
        {
            await Task.Delay(1); // 模擬異步作業
            return null; // 暫時回傳 null
        }

        /// <summary>
        /// 取得使用者錢包資料
        /// </summary>
        public async Task<UserWalletReadModel?> GetUserWalletByIdAsync(int userId)
        {
            await Task.Delay(1); // 模擬異步作業
            
            return new UserWalletReadModel
            {
                WalletID = 1,
                UserID = userId,
                Points = 1000,
                CreatedAt = DateTime.Now.AddDays(-30),
                UpdatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// 取得使用者寵物資料
        /// </summary>
        public async Task<PetReadModel?> GetPetByUserIdAsync(int userId)
        {
            await Task.Delay(1); // 模擬異步作業
            return null; // 暫時回傳 null
        }

        /// <summary>
        /// 取得使用者簽到統計資料
        /// </summary>
        public async Task<List<UserSignInStatsReadModel>> GetUserSignInStatsAsync(int userId, int days = 30)
        {
            await Task.Delay(1); // 模擬異步作業
            return new List<UserSignInStatsReadModel>();
        }

        /// <summary>
        /// 取得使用者小遊戲記錄
        /// </summary>
        public async Task<List<MiniGameReadModel>> GetUserMiniGamesAsync(int userId, int limit = 10)
        {
            await Task.Delay(1); // 模擬異步作業
            return new List<MiniGameReadModel>();
        }

        /// <summary>
        /// 取得使用者優惠券列表
        /// </summary>
        public async Task<List<CouponReadModel>> GetUserCouponsAsync(int userId, bool? isUsed = null)
        {
            await Task.Delay(1); // 模擬異步作業
            return new List<CouponReadModel>();
        }

        /// <summary>
        /// 取得使用者電子禮券列表
        /// </summary>
        public async Task<List<EVoucherReadModel>> GetUserEVouchersAsync(int userId, bool? isUsed = null)
        {
            await Task.Delay(1); // 模擬異步作業
            return new List<EVoucherReadModel>();
        }

        /// <summary>
        /// 取得使用者錢包交易歷史
        /// </summary>
        public async Task<List<WalletHistoryReadModel>> GetUserWalletHistoryAsync(int userId, int limit = 50)
        {
            await Task.Delay(1); // 模擬異步作業
            return new List<WalletHistoryReadModel>();
        }

        /// <summary>
        /// 取得所有使用者列表（分頁）
        /// </summary>
        public async Task<List<UserReadModel>> GetUsersAsync(int page = 1, int pageSize = 20)
        {
            await Task.Delay(1); // 模擬異步作業
            return new List<UserReadModel>();
        }

        /// <summary>
        /// 取得使用者總數
        /// </summary>
        public async Task<int> GetUserCountAsync()
        {
            await Task.Delay(1); // 模擬異步作業
            return 0; // 暫時回傳 0
        }
    }
}