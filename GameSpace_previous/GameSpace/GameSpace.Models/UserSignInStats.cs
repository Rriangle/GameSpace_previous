using System;

namespace GameSpace.Models
{
    public class UserSignInStats
    {
        public int StatID { get; set; }
        public DateTime SignInDate { get; set; }
        public int UserID { get; set; }
        public int PointsEarned { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ConsecutiveDays { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}
