using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 群組成員模型
    /// </summary>
    public partial class GroupMember
    {
        [Key]
        [Column("member_id")]
        public int MemberId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [StringLength(50)]
        [Column("role")]
        public string Role { get; set; } = "Member"; // Owner, Admin, Moderator, Member

        [Column("joined_at")]
        public DateTime JoinedAt { get; set; }

        [Column("last_active_at")]
        public DateTime? LastActiveAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [StringLength(100)]
        [Column("nickname")]
        public string? Nickname { get; set; }

        // 導航屬性
        [ForeignKey("GroupId")]
        public virtual Groups Group { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}