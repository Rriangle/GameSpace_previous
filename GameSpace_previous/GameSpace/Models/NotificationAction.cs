using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 通知動作模型
    /// </summary>
    public partial class NotificationAction
    {
        [Key]
        [Column("action_id")]
        public int ActionId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("action_name")]
        public string ActionName { get; set; } = null!;

        [StringLength(500)]
        [Column("description")]
        public string? Description { get; set; }

        [StringLength(200)]
        [Column("action_url")]
        public string? ActionUrl { get; set; }

        [StringLength(50)]
        [Column("action_type")]
        public string ActionType { get; set; } = "Link"; // Link, Button, Modal, Redirect

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}