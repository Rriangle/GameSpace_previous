using Microsoft.EntityFrameworkCore;
using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;

namespace GameSpace.Infrastructure.Repositories
{
    public class CommunityReadOnlyRepository : ICommunityReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public CommunityReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        // 論壇相關 - 暫時返回空列表
        public async Task<List<ForumReadModel>> GetForumsAsync()
        {
            return await Task.FromResult(new List<ForumReadModel>());
        }

        public async Task<ForumReadModel?> GetForumByIdAsync(int forumId)
        {
            return await Task.FromResult<ForumReadModel?>(null);
        }

        public async Task<List<ThreadReadModel>> GetThreadsByForumAsync(int forumId, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<ThreadReadModel>());
        }

        public async Task<ThreadReadModel?> GetThreadByIdAsync(long threadId)
        {
            return await Task.FromResult<ThreadReadModel?>(null);
        }

        public async Task<List<ThreadPostReadModel>> GetThreadPostsAsync(long threadId, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<ThreadPostReadModel>());
        }

        // 文章相關 - 暫時返回空列表
        public async Task<List<PostReadModel>> GetPostsAsync(string? type = null, int? gameId = null, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<PostReadModel>());
        }

        public async Task<PostReadModel?> GetPostByIdAsync(int postId)
        {
            return await Task.FromResult<PostReadModel?>(null);
        }

        public async Task<List<PostMetricSnapshotReadModel>> GetPostMetricSnapshotsAsync(int postId)
        {
            return await Task.FromResult(new List<PostMetricSnapshotReadModel>());
        }

        public async Task<List<PostSourceReadModel>> GetPostSourcesAsync(int postId)
        {
            return await Task.FromResult(new List<PostSourceReadModel>());
        }

        // 反應和收藏 - 暫時返回空列表
        public async Task<List<ReactionReadModel>> GetReactionsAsync(string targetType, long targetId)
        {
            return await Task.FromResult(new List<ReactionReadModel>());
        }

        public async Task<List<BookmarkReadModel>> GetUserBookmarksAsync(int userId, string? targetType = null)
        {
            return await Task.FromResult(new List<BookmarkReadModel>());
        }

        public async Task<bool> IsBookmarkedAsync(int userId, string targetType, long targetId)
        {
            return await Task.FromResult(false);
        }

        // 通知相關 - 暫時返回空列表
        public async Task<List<NotificationReadModel>> GetUserNotificationsAsync(int userId, bool? isRead = null, int limit = 50)
        {
            return await Task.FromResult(new List<NotificationReadModel>());
        }

        public async Task<NotificationReadModel?> GetNotificationByIdAsync(int notificationId)
        {
            return await Task.FromResult<NotificationReadModel?>(null);
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await Task.FromResult(0);
        }

        // 聊天相關 - 暫時返回空列表
        public async Task<List<ChatMessageReadModel>> GetChatMessagesAsync(int? senderId = null, int? receiverId = null, int limit = 50)
        {
            return await Task.FromResult(new List<ChatMessageReadModel>());
        }

        public async Task<List<GroupChatReadModel>> GetGroupChatMessagesAsync(int groupId, int limit = 50)
        {
            return await Task.FromResult(new List<GroupChatReadModel>());
        }

        // 群組相關 - 暫時返回空列表
        public async Task<List<GroupReadModel>> GetUserGroupsAsync(int userId)
        {
            return await Task.FromResult(new List<GroupReadModel>());
        }

        public async Task<GroupReadModel?> GetGroupByIdAsync(int groupId)
        {
            return await Task.FromResult<GroupReadModel?>(null);
        }

        public async Task<List<GroupMemberReadModel>> GetGroupMembersAsync(int groupId)
        {
            return await Task.FromResult(new List<GroupMemberReadModel>());
        }

        public async Task<List<GroupBlockReadModel>> GetGroupBlocksAsync(int groupId)
        {
            return await Task.FromResult(new List<GroupBlockReadModel>());
        }

        // 好友關係 - 暫時返回空列表
        public async Task<List<RelationReadModel>> GetUserRelationsAsync(int userId, int? statusId = null)
        {
            return await Task.FromResult(new List<RelationReadModel>());
        }

        public async Task<RelationReadModel?> GetRelationAsync(int userId, int friendId)
        {
            return await Task.FromResult<RelationReadModel?>(null);
        }

        public async Task<List<RelationStatusReadModel>> GetRelationStatusesAsync()
        {
            return await Task.FromResult(new List<RelationStatusReadModel>());
        }

        // 統計 - 暫時返回0
        public async Task<int> GetThreadCountAsync(int forumId)
        {
            return await Task.FromResult(0);
        }

        public async Task<int> GetPostCountAsync(long threadId)
        {
            return await Task.FromResult(0);
        }

        public async Task<int> GetReactionCountAsync(string targetType, long targetId)
        {
            return await Task.FromResult(0);
        }
    }
}
