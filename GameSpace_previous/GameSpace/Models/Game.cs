using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 遊戲清單資料表
    /// </summary>
    public partial class Game
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = null!;
        public string? GameDescription { get; set; }
        public string? GameUrl { get; set; }
        public string? GameImageUrl { get; set; }
        public string? GameCategory { get; set; }
        public string? GamePlatform { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();
        public virtual ICollection<LeaderboardSnapshot> LeaderboardSnapshots { get; set; } = new List<LeaderboardSnapshot>();
    }
}