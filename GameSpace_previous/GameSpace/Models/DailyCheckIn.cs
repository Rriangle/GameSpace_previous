using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 每日簽到資料表
    /// </summary>
    public partial class DailyCheckIn
    {
        public int CheckInId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public int ConsecutiveDays { get; set; }
        public int PointsEarned { get; set; }
        public int PetExpEarned { get; set; }
        public string? CouponEarned { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users User { get; set; } = null!;
    }
}