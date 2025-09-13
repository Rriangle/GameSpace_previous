using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 群組封鎖模型
    /// </summary>
    public partial class GroupBlock
    {
        [Key]
        [Column("block_id")]
        public int BlockId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("blocked_by_user_id")]
        public int BlockedByUserId { get; set; }

        [Column("blocked_at")]
        public DateTime BlockedAt { get; set; }

        [StringLength(200)]
        [Column("reason")]
        public string? Reason { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // 導航屬性
        [ForeignKey("GroupId")]
        public virtual Groups Group { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("BlockedByUserId")]
        public virtual Users BlockedByUser { get; set; } = null!;
    }
}