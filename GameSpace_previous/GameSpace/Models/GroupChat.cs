using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 群組聊天模型
    /// </summary>
    public partial class GroupChat
    {
        [Key]
        [Column("message_id")]
        public int MessageId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("sender_user_id")]
        public int SenderUserId { get; set; }

        [Required]
        [StringLength(2000)]
        [Column("message_content")]
        public string MessageContent { get; set; } = null!;

        [StringLength(50)]
        [Column("message_type")]
        public string MessageType { get; set; } = "Text"; // Text, Image, File, Voice, Video

        [Column("sent_at")]
        public DateTime SentAt { get; set; }

        [Column("edited_at")]
        public DateTime? EditedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [StringLength(200)]
        [Column("attachment_url")]
        public string? AttachmentUrl { get; set; }

        [StringLength(100)]
        [Column("attachment_name")]
        public string? AttachmentName { get; set; }

        // 導航屬性
        [ForeignKey("GroupId")]
        public virtual Groups Group { get; set; } = null!;

        [ForeignKey("SenderUserId")]
        public virtual Users SenderUser { get; set; } = null!;
    }
}