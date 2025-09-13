using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 群組已讀狀態模型
    /// </summary>
    public partial class GroupReadState
    {
        [Key]
        [Column("read_state_id")]
        public int ReadStateId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("last_read_message_id")]
        public int? LastReadMessageId { get; set; }

        [Column("last_read_at")]
        public DateTime? LastReadAt { get; set; }

        [Column("unread_count")]
        public int UnreadCount { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        [ForeignKey("GroupId")]
        public virtual Groups Group { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("LastReadMessageId")]
        public virtual GroupChat? LastReadMessage { get; set; }
    }
}