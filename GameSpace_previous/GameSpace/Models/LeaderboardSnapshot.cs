using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 排行榜快照資料表
    /// </summary>
    public partial class LeaderboardSnapshot
    {
        public int SnapshotId { get; set; }
        public int GameId { get; set; }
        public string SnapshotType { get; set; } = null!; // Daily, Weekly, Monthly, AllTime
        public int Rank { get; set; }
        public decimal HeatScore { get; set; }
        public decimal PlayerCount { get; set; }
        public decimal ViewCount { get; set; }
        public decimal LikeCount { get; set; }
        public decimal ShareCount { get; set; }
        public decimal CommentCount { get; set; }
        public decimal PlayTime { get; set; }
        public decimal Revenue { get; set; }
        public DateTime SnapshotDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;
    }
}