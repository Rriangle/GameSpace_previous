using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 私聊對話模型
    /// </summary>
    public partial class DM_Conversations
    {
        [Key]
        [Column("conversation_id")]
        public int ConversationId { get; set; }

        [Column("is_manager_dm")]
        public bool IsManagerDm { get; set; }

        [Column("party1_id")]
        public int Party1Id { get; set; }

        [Column("party2_id")]
        public int Party2Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("last_message_at")]
        public DateTime? LastMessageAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // 導航屬性
        [ForeignKey("Party1Id")]
        public virtual Users Party1 { get; set; } = null!;

        [ForeignKey("Party2Id")]
        public virtual Users Party2 { get; set; } = null!;

        public virtual ICollection<DM_Messages> DM_Messages { get; set; } = new List<DM_Messages>();
    }
}