using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 論壇資料表
    /// </summary>
    public partial class Forum
    {
        public int ForumId { get; set; }
        public int GameId { get; set; }
        public string ForumName { get; set; } = null!;
        public string? Description { get; set; }
        public int ThreadCount { get; set; }
        public int PostCount { get; set; }
        public DateTime? LastActivity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;
        public virtual ICollection<Thread> Threads { get; set; } = new List<Thread>();
    }
}