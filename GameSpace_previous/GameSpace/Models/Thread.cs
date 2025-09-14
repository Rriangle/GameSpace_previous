using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class Thread
{
    public long ThreadId { get; set; }

    public int? ForumId { get; set; }

    public int? AuthorUserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public bool IsPinned { get; set; }

    public bool IsActive { get; set; }

    public int ViewCount { get; set; }

    public int ReplyCount { get; set; }

    public DateTime? LastActivity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Users? AuthorUser { get; set; }

    public virtual Forum? Forum { get; set; }

    public virtual ICollection<ThreadPost> ThreadPosts { get; set; } = new List<ThreadPost>();
}
