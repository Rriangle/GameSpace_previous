using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶封鎖資料表
    /// </summary>
    public partial class UserBlock
    {
        public int BlockId { get; set; }
        public int BlockerId { get; set; }
        public int BlockedId { get; set; }
        public string? Reason { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UnblockedAt { get; set; }

        public virtual Users Blocker { get; set; } = null!;
        public virtual Users Blocked { get; set; } = null!;
    }
}