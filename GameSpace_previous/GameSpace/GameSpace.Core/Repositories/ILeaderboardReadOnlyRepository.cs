using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 排行榜讀取專用存儲庫介面 - Stage 2 廣度切片
    /// 提供排行榜聚合查詢功能
    /// </summary>
    public interface ILeaderboardReadOnlyRepository
    {
        /// <summary>
        /// 取得排行榜總覽（包含每日、每週、每月排行榜）
        /// </summary>
        /// <param name="userId">用戶 ID（可選，用於查詢個人排名）</param>
        /// <returns>排行榜總覽聚合模型</returns>
        Task<LeaderboardOverviewReadModel> GetLeaderboardOverviewAsync(int? userId = null);

        /// <summary>
        /// 取得指定時間週期的排行榜
        /// </summary>
        /// <param name="period">時間週期（daily, weekly, monthly）</param>
        /// <param name="gameId">遊戲 ID（可選）</param>
        /// <param name="limit">取得數量限制（預設 50）</param>
        /// <returns>排行榜條目列表</returns>
        Task<List<LeaderboardEntryReadModel>> GetLeaderboardByPeriodAsync(string period, int? gameId = null, int limit = 50);

        /// <summary>
        /// 取得指定遊戲的排行榜
        /// </summary>
        /// <param name="gameId">遊戲 ID</param>
        /// <param name="period">時間週期（daily, weekly, monthly）</param>
        /// <param name="limit">取得數量限制（預設 50）</param>
        /// <returns>遊戲排行榜聚合模型</returns>
        Task<GameLeaderboardReadModel?> GetGameLeaderboardAsync(int gameId, string period, int limit = 50);

        /// <summary>
        /// 取得用戶在各時間週期的排名資訊
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>用戶排名資訊</returns>
        Task<UserRankingInfo?> GetUserRankingInfoAsync(int userId);

        /// <summary>
        /// 取得用戶在指定遊戲和時間週期的排名
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="gameId">遊戲 ID</param>
        /// <param name="period">時間週期</param>
        /// <returns>排名詳情</returns>
        Task<RankingDetail?> GetUserGameRankingAsync(int userId, int gameId, string period);

        /// <summary>
        /// 取得排行榜歷史趨勢（用戶在指定遊戲的排名變化）
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="gameId">遊戲 ID</param>
        /// <param name="period">時間週期</param>
        /// <param name="days">查詢天數（預設 30 天）</param>
        /// <returns>排名歷史列表</returns>
        Task<List<RankingDetail>> GetUserRankingHistoryAsync(int userId, int gameId, string period, int days = 30);
    }
}
