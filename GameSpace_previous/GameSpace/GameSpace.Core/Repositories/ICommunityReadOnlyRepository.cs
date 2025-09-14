using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 社群相關只讀存儲庫
    /// </summary>
    public interface ICommunityReadOnlyRepository
    {
        // 論壇相關
        Task<List<ForumReadModel>> GetForumsAsync();
        Task<ForumReadModel?> GetForumByIdAsync(int forumId);
        Task<List<ThreadReadModel>> GetThreadsByForumAsync(int forumId, int page = 1, int pageSize = 20);
        Task<ThreadReadModel?> GetThreadByIdAsync(long threadId);
        Task<List<ThreadPostReadModel>> GetThreadPostsAsync(long threadId, int page = 1, int pageSize = 20);
        
        // 文章相關
        Task<List<PostReadModel>> GetPostsAsync(string? type = null, int? gameId = null, int page = 1, int pageSize = 20);
        Task<PostReadModel?> GetPostByIdAsync(int postId);
        Task<List<PostMetricSnapshotReadModel>> GetPostMetricSnapshotsAsync(int postId);
        Task<List<PostSourceReadModel>> GetPostSourcesAsync(int postId);
        
        // 反應和收藏
        Task<List<ReactionReadModel>> GetReactionsAsync(string targetType, long targetId);
        Task<List<BookmarkReadModel>> GetUserBookmarksAsync(int userId, string? targetType = null);
        Task<bool> IsBookmarkedAsync(int userId, string targetType, long targetId);
        
        // 通知相關
        Task<List<NotificationReadModel>> GetUserNotificationsAsync(int userId, bool? isRead = null, int limit = 50);
        Task<NotificationReadModel?> GetNotificationByIdAsync(int notificationId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        
        // 聊天相關
        Task<List<ChatMessageReadModel>> GetChatMessagesAsync(int? senderId = null, int? receiverId = null, int limit = 50);
        Task<List<GroupChatReadModel>> GetGroupChatMessagesAsync(int groupId, int limit = 50);
        
        // 群組相關
        Task<List<GroupReadModel>> GetUserGroupsAsync(int userId);
        Task<GroupReadModel?> GetGroupByIdAsync(int groupId);
        Task<List<GroupMemberReadModel>> GetGroupMembersAsync(int groupId);
        Task<List<GroupBlockReadModel>> GetGroupBlocksAsync(int groupId);
        
        // 好友關係
        Task<List<RelationReadModel>> GetUserRelationsAsync(int userId, int? statusId = null);
        Task<RelationReadModel?> GetRelationAsync(int userId, int friendId);
        Task<List<RelationStatusReadModel>> GetRelationStatusesAsync();
        
        // 統計
        Task<int> GetThreadCountAsync(int forumId);
        Task<int> GetPostCountAsync(long threadId);
        Task<int> GetReactionCountAsync(string targetType, long targetId);
    }
}
