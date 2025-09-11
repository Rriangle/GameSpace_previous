namespace GameSpace.Models
{
    /// <summary>
    /// 排行榜總覽聚合讀取模型 - Stage 2 廣度切片
    /// 提供排行榜頁面所需的聚合資訊
    /// </summary>
    public class LeaderboardOverviewReadModel
    {
        /// <summary>
        /// 每日排行榜（前 10 名）
        /// </summary>
        public List<LeaderboardEntryReadModel> DailyLeaderboard { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// 每週排行榜（前 10 名）
        /// </summary>
        public List<LeaderboardEntryReadModel> WeeklyLeaderboard { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// 每月排行榜（前 10 名）
        /// </summary>
        public List<LeaderboardEntryReadModel> MonthlyLeaderboard { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// 我的排名資訊（如果用戶有登入）
        /// </summary>
        public UserRankingInfo? MyRanking { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// 排行榜條目讀取模型
    /// </summary>
    public class LeaderboardEntryReadModel
    {
        /// <summary>
        /// 排名
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 用戶 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用戶名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 用戶暱稱
        /// </summary>
        public string? UserNickName { get; set; }

        /// <summary>
        /// 遊戲 ID
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// 遊戲名稱
        /// </summary>
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// 指標數值（分數、經驗值等）
        /// </summary>
        public decimal IndexValue { get; set; }

        /// <summary>
        /// 時間週期
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// 快照時間
        /// </summary>
        public DateTime SnapshotTime { get; set; }
    }

    /// <summary>
    /// 用戶排名資訊
    /// </summary>
    public class UserRankingInfo
    {
        /// <summary>
        /// 用戶 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 每日排名
        /// </summary>
        public RankingDetail? DailyRank { get; set; }

        /// <summary>
        /// 每週排名
        /// </summary>
        public RankingDetail? WeeklyRank { get; set; }

        /// <summary>
        /// 每月排名
        /// </summary>
        public RankingDetail? MonthlyRank { get; set; }
    }

    /// <summary>
    /// 排名詳情
    /// </summary>
    public class RankingDetail
    {
        public int Rank { get; set; }
        public decimal IndexValue { get; set; }
        public DateTime SnapshotTime { get; set; }
    }

    /// <summary>
    /// 遊戲排行榜聚合讀取模型
    /// 提供特定遊戲的排行榜資訊
    /// </summary>
    public class GameLeaderboardReadModel
    {
        /// <summary>
        /// 遊戲 ID
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// 遊戲名稱
        /// </summary>
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// 遊戲描述
        /// </summary>
        public string? GameDescription { get; set; }

        /// <summary>
        /// 排行榜條目列表
        /// </summary>
        public List<LeaderboardEntryReadModel> Entries { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// 時間週期
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// 總參與人數
        /// </summary>
        public int TotalParticipants { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
