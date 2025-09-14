using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 通知接收者模型
    /// </summary>
    public partial class NotificationRecipient
    {
        [Key]
        [Column("recipient_id")]
        public int RecipientId { get; set; }

        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("manager_id")]
        public int? ManagerId { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("read_at")]
        public DateTime? ReadAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // 導航屬性
        [ForeignKey("NotificationId")]
        public virtual Notification Notification { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        [ForeignKey("ManagerId")]
        public virtual ManagerData? Manager { get; set; }
    }
}