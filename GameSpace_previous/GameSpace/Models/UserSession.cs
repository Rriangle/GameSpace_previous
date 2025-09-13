using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶會話資料表
    /// </summary>
    public partial class UserSession
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string SessionToken { get; set; } = null!;
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public virtual Users User { get; set; } = null!;
    }
}