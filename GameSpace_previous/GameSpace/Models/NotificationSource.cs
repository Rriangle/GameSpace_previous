using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 通知來源模型
    /// </summary>
    public partial class NotificationSource
    {
        [Key]
        [Column("source_id")]
        public int SourceId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("source_name")]
        public string SourceName { get; set; } = null!;

        [StringLength(500)]
        [Column("description")]
        public string? Description { get; set; }

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