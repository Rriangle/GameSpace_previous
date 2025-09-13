using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶活動記錄資料表
    /// </summary>
    public partial class UserActivity
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string ActivityType { get; set; } = null!; // Login, Logout, View, Click, Purchase, etc.
        public string? Description { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users User { get; set; } = null!;
    }
}