using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 遊戲來源映射資料表
    /// </summary>
    public partial class GameSourceMap
    {
        public int SourceMapId { get; set; }
        public int GameId { get; set; }
        public string SourceName { get; set; } = null!;
        public string? SourceUrl { get; set; }
        public string? SourceType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;
    }
}