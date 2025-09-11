using System;

namespace GameSpace.Models
{
    public class MiniGame
    {
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string GameType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsReward { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
