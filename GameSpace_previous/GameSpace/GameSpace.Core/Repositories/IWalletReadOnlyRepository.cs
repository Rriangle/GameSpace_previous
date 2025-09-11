using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 錢包讀取專用存儲庫介面 - Stage 2 廣度切片
    /// 提供錢包總覽聚合查詢功能
    /// </summary>
    public interface IWalletReadOnlyRepository
    {
        /// <summary>
        /// 取得用戶錢包總覽（包含積分、優惠券、電子禮券聚合資訊）
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>錢包總覽讀取模型</returns>
        Task<WalletOverviewReadModel?> GetWalletOverviewAsync(int userId);

        /// <summary>
        /// 取得用戶當前積分餘額
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>積分餘額</returns>
        Task<int> GetUserPointsAsync(int userId);

        /// <summary>
        /// 取得用戶錢包異動歷史（分頁）
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="pageIndex">頁數索引（從 0 開始）</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>錢包異動歷史列表</returns>
        Task<List<WalletHistoryReadModel>> GetWalletHistoryAsync(int userId, int pageIndex = 0, int pageSize = 10);

        /// <summary>
        /// 取得用戶可用優惠券列表
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>可用優惠券列表</returns>
        Task<List<CouponOverviewReadModel>> GetAvailableCouponsAsync(int userId);

        /// <summary>
        /// 取得用戶可用電子禮券列表
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>可用電子禮券列表</returns>
        Task<List<EVoucherOverviewReadModel>> GetAvailableEVouchersAsync(int userId);
    }
}
