using System;

namespace GameSpace.Models
{
    public class WalletHistory
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public int PointsChanged { get; set; }
        public string? ItemCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ChangeTime { get; set; }
    }
}
