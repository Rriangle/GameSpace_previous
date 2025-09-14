using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 遊戲每日指標資料表
    /// </summary>
    public partial class GameMetricDaily
    {
        public int MetricId { get; set; }
        public int GameId { get; set; }
        public int SourceId { get; set; }
        public DateTime MetricDate { get; set; }
        public decimal PlayerCount { get; set; }
        public decimal ViewCount { get; set; }
        public decimal LikeCount { get; set; }
        public decimal ShareCount { get; set; }
        public decimal CommentCount { get; set; }
        public decimal PlayTime { get; set; } // 總遊戲時間（分鐘）
        public decimal Revenue { get; set; }
        public decimal HeatScore { get; set; } // 計算出的熱度分數
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;
        public virtual MetricSource Source { get; set; } = null!;
    }
}