using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 通知模型
    /// </summary>
    public partial class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Column("source_id")]
        public int SourceId { get; set; }

        [Column("action_id")]
        public int ActionId { get; set; }

        [Column("group_id")]
        public int? GroupId { get; set; }

        [Column("sender_user_id")]
        public int? SenderUserId { get; set; }

        [Column("sender_manager_id")]
        public int? SenderManagerId { get; set; }

        [Required]
        [StringLength(500)]
        [Column("title")]
        public string Title { get; set; } = null!;

        [StringLength(2000)]
        [Column("content")]
        public string? Content { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("read_at")]
        public DateTime? ReadAt { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [StringLength(50)]
        [Column("priority")]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent

        [StringLength(50)]
        [Column("type")]
        public string Type { get; set; } = "Info"; // Info, Warning, Error, Success

        // 導航屬性
        [ForeignKey("SourceId")]
        public virtual NotificationSource NotificationSource { get; set; } = null!;

        [ForeignKey("ActionId")]
        public virtual NotificationAction NotificationAction { get; set; } = null!;

        [ForeignKey("SenderUserId")]
        public virtual Users? SenderUser { get; set; }

        [ForeignKey("SenderManagerId")]
        public virtual ManagerData? SenderManager { get; set; }

        public virtual ICollection<NotificationRecipient> NotificationRecipients { get; set; } = new List<NotificationRecipient>();
    }
}