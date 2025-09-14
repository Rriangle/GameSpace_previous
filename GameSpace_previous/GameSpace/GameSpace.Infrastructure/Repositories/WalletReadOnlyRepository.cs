using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// 錢包讀取專用存儲庫實現 - Stage 2 廣度切片
    /// 實現錢包聚合查詢邏輯
    /// </summary>
    public class WalletReadOnlyRepository : IWalletReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public WalletReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得用戶錢包總覽（聚合查詢：積分 + 優惠券 + 電子禮券）
        /// 目前返回模擬資料，等待後續完整實現
        /// </summary>
        public async Task<WalletOverviewReadModel?> GetWalletOverviewAsync(int userId)
        {
            // 目前返回模擬資料，等待後續完整實現
            // 需要根據實際的資料庫 schema 調整查詢邏輯
            await Task.Delay(1); // 模擬異步操作

            return new WalletOverviewReadModel
            {
                UserId = userId,
                UserName = $"用戶{userId}",
                CurrentPoints = 1000,
                AvailableCouponsCount = 3,
                UsedCouponsCount = 2,
                AvailableEVouchersCount = 1,
                UsedEVouchersCount = 1,
                RecentTransactions = new List<WalletHistoryReadModel>(),
                AvailableCoupons = new List<CouponOverviewReadModel>(),
                AvailableEVouchers = new List<EVoucherOverviewReadModel>()
            };
        }

        /// <summary>
        /// 取得用戶當前積分餘額
        /// </summary>
        public async Task<int> GetUserPointsAsync(int userId)
        {
            // 目前返回模擬資料，等待後續完整實現
            await Task.Delay(1); // 模擬異步操作
            return 1000; // 模擬積分
        }

        /// <summary>
        /// 取得用戶錢包異動歷史（分頁，按時間倒序）
        /// </summary>
        public async Task<List<WalletHistoryReadModel>> GetWalletHistoryAsync(int userId, int pageIndex = 0, int pageSize = 10)
        {
            // 目前返回空列表，等待後續完整實現
            await Task.Delay(1); // 模擬異步操作
            return new List<WalletHistoryReadModel>();
        }

        /// <summary>
        /// 取得用戶可用優惠券列表（未使用且未過期）
        /// </summary>
        public async Task<List<CouponOverviewReadModel>> GetAvailableCouponsAsync(int userId)
        {
            // 目前返回空列表，等待後續完整實現
            await Task.Delay(1); // 模擬異步操作
            return new List<CouponOverviewReadModel>();
        }

        /// <summary>
        /// 取得用戶可用電子禮券列表（未使用且未過期）
        /// </summary>
        public async Task<List<EVoucherOverviewReadModel>> GetAvailableEVouchersAsync(int userId)
        {
            // 目前返回空列表，等待後續完整實現
            await Task.Delay(1); // 模擬異步操作
            return new List<EVoucherOverviewReadModel>();
        }
    }
}
