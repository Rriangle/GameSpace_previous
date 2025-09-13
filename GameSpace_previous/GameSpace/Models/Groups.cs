using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 群組模型
    /// </summary>
    public partial class Groups
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("owner_user_id")]
        public int OwnerUserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("group_name")]
        public string GroupName { get; set; } = null!;

        [StringLength(200)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("is_private")]
        public bool IsPrivate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("max_members")]
        public int? MaxMembers { get; set; }

        [StringLength(100)]
        [Column("group_avatar")]
        public string? GroupAvatar { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // 導航屬性
        [ForeignKey("OwnerUserId")]
        public virtual Users OwnerUser { get; set; } = null!;

        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        public virtual ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();
        public virtual ICollection<GroupBlock> GroupBlocks { get; set; } = new List<GroupBlock>();
        public virtual ICollection<GroupReadState> GroupReadStates { get; set; } = new List<GroupReadState>();
    }
}