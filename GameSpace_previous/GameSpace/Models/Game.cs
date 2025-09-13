using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 遊戲資料表
    /// </summary>
    public partial class Game
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = null!;
        public string? GameDescription { get; set; }
        public string? GameImageUrl { get; set; }
        public string? GameGenre { get; set; }
        public string? GamePlatform { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();
        public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();
        public virtual ICollection<GameProductDetail> GameProductDetails { get; set; } = new List<GameProductDetail>();
        public virtual ICollection<GameSourceMap> GameSourceMaps { get; set; } = new List<GameSourceMap>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}