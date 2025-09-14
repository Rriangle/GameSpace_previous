using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶舉報資料表
    /// </summary>
    public partial class UserReport
    {
        public int ReportId { get; set; }
        public int ReporterId { get; set; }
        public int? ReportedUserId { get; set; }
        public string? ReportedEntityType { get; set; }
        public int? ReportedEntityId { get; set; }
        public string ReportType { get; set; } = null!; // Spam, Harassment, Inappropriate, etc.
        public string Description { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Resolved, Dismissed
        public int? ReviewedBy { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public virtual Users Reporter { get; set; } = null!;
        public virtual Users? ReportedUser { get; set; }
        public virtual Users? Reviewer { get; set; }
    }
}