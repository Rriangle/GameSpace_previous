using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 遊戲每日指標資料表
    /// </summary>
    public partial class GameMetricDaily
    {
        public int MetricId { get; set; }
        public int GameId { get; set; }
        public DateTime MetricDate { get; set; }
        public int? ViewCount { get; set; }
        public int? PostCount { get; set; }
        public int? ThreadCount { get; set; }
        public int? UserCount { get; set; }
        public decimal? AverageRating { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;
    }
}