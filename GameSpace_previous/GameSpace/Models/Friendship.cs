using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 好友關係模型
    /// </summary>
    public partial class Friendship
    {
        [Key]
        [Column("friendship_id")]
        public int FriendshipId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("friend_id")]
        public int FriendId { get; set; }

        [StringLength(50)]
        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Blocked, Declined

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("accepted_at")]
        public DateTime? AcceptedAt { get; set; }

        [Column("blocked_at")]
        public DateTime? BlockedAt { get; set; }

        [StringLength(200)]
        [Column("note")]
        public string? Note { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // 導航屬性
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("FriendId")]
        public virtual Users Friend { get; set; } = null!;
    }
}