using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 系統日誌資料表
    /// </summary>
    public partial class SystemLog
    {
        public int LogId { get; set; }
        public string LogLevel { get; set; } = null!; // Info, Warning, Error, Critical
        public string Message { get; set; } = null!;
        public string? Exception { get; set; }
        public string? Source { get; set; }
        public int? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users? User { get; set; }
    }
}