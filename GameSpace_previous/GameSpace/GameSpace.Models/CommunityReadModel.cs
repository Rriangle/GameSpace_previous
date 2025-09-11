using System.ComponentModel.DataAnnotations;

namespace GameSpace.Models
{
    /// <summary>
    /// 評論讀取模型
    /// </summary>
    public class CommentReadModel
    {
        public long CommentID { get; set; }
        public long PostID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public long? ParentCommentID { get; set; }
    }

    /// <summary>
    /// 按讚讀取模型
    /// </summary>
    public class LikeReadModel
    {
        public long LikeID { get; set; }
        public int UserID { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public long TargetID { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 分類讀取模型
    /// </summary>
    public class CategoryReadModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 標籤讀取模型
    /// </summary>
    public class TagReadModel
    {
        public int TagID { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 遊戲讀取模型
    /// </summary>
    public class GameReadModel
    {
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// 論壇讀取模型
    /// </summary>
    public class ForumReadModel
    {
        public int ForumID { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string ForumName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ThreadCount { get; set; }
        public int PostCount { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 討論串讀取模型
    /// </summary>
    public class ThreadReadModel
    {
        public long ThreadID { get; set; }
        public int ForumID { get; set; }
        public string ForumName { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int ReplyCount { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPinned { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 貼文讀取模型
    /// </summary>
    public class PostReadModel
    {
        public long PostID { get; set; }
        public long ThreadID { get; set; }
        public string ThreadTitle { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public long? ParentPostID { get; set; }
    }

    /// <summary>
    /// 反應讀取模型
    /// </summary>
    public class ReactionReadModel
    {
        public long ReactionID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public long TargetID { get; set; }
        public string ReactionType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 書籤讀取模型
    /// </summary>
    public class BookmarkReadModel
    {
        public long BookmarkID { get; set; }
        public int UserID { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public long TargetID { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 用戶關係讀取模型
    /// </summary>
    public class UserRelationReadModel
    {
        public long RelationID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RelatedUserID { get; set; }
        public string RelatedUserName { get; set; } = string.Empty;
        public string RelationType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 用戶群組讀取模型
    /// </summary>
    public class UserGroupReadModel
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 聊天訊息讀取模型
    /// </summary>
    public class ChatMessageReadModel
    {
        public long MessageID { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int? ReceiverID { get; set; }
        public string? ReceiverName { get; set; }
        public int? GroupID { get; set; }
        public string? GroupName { get; set; }
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 通知讀取模型
    /// </summary>
    public class NotificationReadModel
    {
        public long NotificationID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }
    }
}
