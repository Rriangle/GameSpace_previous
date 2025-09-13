using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 郵件模板資料表
    /// </summary>
    public partial class EmailTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string TemplateType { get; set; } = null!; // Welcome, ResetPassword, Notification, etc.
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}