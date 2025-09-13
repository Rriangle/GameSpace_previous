using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 郵件隊列資料表
    /// </summary>
    public partial class EmailQueue
    {
        public int QueueId { get; set; }
        public int? UserId { get; set; }
        public string ToEmail { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
        public int? TemplateId { get; set; }
        public int RetryCount { get; set; }
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }

        public virtual Users? User { get; set; }
        public virtual EmailTemplate? Template { get; set; }
    }
}