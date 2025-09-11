using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 社群相關 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityReadOnlyRepository _communityRepository;

        public CommunityController(ICommunityReadOnlyRepository communityRepository)
        {
            _communityRepository = communityRepository;
        }

        /// <summary>
        /// 取得論壇列表
        /// </summary>
        [HttpGet("forums")]
        public async Task<ActionResult<List<ForumReadModel>>> GetForums()
        {
            try
            {
                var forums = await _communityRepository.GetForumsAsync();
                return Ok(forums);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得論壇列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得論壇詳情
        /// </summary>
        [HttpGet("forums/{forumId}")]
        public async Task<ActionResult<ForumReadModel>> GetForumById(int forumId)
        {
            try
            {
                var forum = await _communityRepository.GetForumByIdAsync(forumId);
                if (forum == null)
                {
                    return NotFound($"找不到論壇 ID: {forumId}");
                }
                return Ok(forum);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得論壇詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得論壇主題列表
        /// </summary>
        [HttpGet("forums/{forumId}/threads")]
        public async Task<ActionResult<List<ThreadReadModel>>> GetThreadsByForum(
            int forumId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var threads = await _communityRepository.GetThreadsByForumAsync(forumId, page, pageSize);
                return Ok(threads);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得主題列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得主題詳情
        /// </summary>
        [HttpGet("threads/{threadId}")]
        public async Task<ActionResult<ThreadReadModel>> GetThreadById(long threadId)
        {
            try
            {
                var thread = await _communityRepository.GetThreadByIdAsync(threadId);
                if (thread == null)
                {
                    return NotFound($"找不到主題 ID: {threadId}");
                }
                return Ok(thread);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得主題詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得主題回覆列表
        /// </summary>
        [HttpGet("threads/{threadId}/posts")]
        public async Task<ActionResult<List<ThreadPostReadModel>>> GetThreadPosts(
            long threadId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var posts = await _communityRepository.GetThreadPostsAsync(threadId, page, pageSize);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得回覆列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得文章列表
        /// </summary>
        [HttpGet("posts")]
        public async Task<ActionResult<List<PostReadModel>>> GetPosts(
            [FromQuery] string? type = null,
            [FromQuery] int? gameId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var posts = await _communityRepository.GetPostsAsync(type, gameId, page, pageSize);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得文章列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得文章詳情
        /// </summary>
        [HttpGet("posts/{postId}")]
        public async Task<ActionResult<PostReadModel>> GetPostById(int postId)
        {
            try
            {
                var post = await _communityRepository.GetPostByIdAsync(postId);
                if (post == null)
                {
                    return NotFound($"找不到文章 ID: {postId}");
                }
                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得文章詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得反應列表
        /// </summary>
        [HttpGet("reactions")]
        public async Task<ActionResult<List<ReactionReadModel>>> GetReactions(
            [FromQuery] string targetType,
            [FromQuery] long targetId)
        {
            try
            {
                var reactions = await _communityRepository.GetReactionsAsync(targetType, targetId);
                return Ok(reactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得反應列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶收藏列表
        /// </summary>
        [HttpGet("users/{userId}/bookmarks")]
        public async Task<ActionResult<List<BookmarkReadModel>>> GetUserBookmarks(
            int userId,
            [FromQuery] string? targetType = null)
        {
            try
            {
                var bookmarks = await _communityRepository.GetUserBookmarksAsync(userId, targetType);
                return Ok(bookmarks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得用戶收藏列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 檢查是否已收藏
        /// </summary>
        [HttpGet("users/{userId}/bookmarks/check")]
        public async Task<ActionResult<bool>> IsBookmarked(
            int userId,
            [FromQuery] string targetType,
            [FromQuery] long targetId)
        {
            try
            {
                var isBookmarked = await _communityRepository.IsBookmarkedAsync(userId, targetType, targetId);
                return Ok(isBookmarked);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"檢查收藏狀態失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶通知列表
        /// </summary>
        [HttpGet("users/{userId}/notifications")]
        public async Task<ActionResult<List<NotificationReadModel>>> GetUserNotifications(
            int userId,
            [FromQuery] bool? isRead = null,
            [FromQuery] int limit = 50)
        {
            try
            {
                var notifications = await _communityRepository.GetUserNotificationsAsync(userId, isRead, limit);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得用戶通知列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得未讀通知數量
        /// </summary>
        [HttpGet("users/{userId}/notifications/unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationCount(int userId)
        {
            try
            {
                var count = await _communityRepository.GetUnreadNotificationCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得未讀通知數量失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得聊天訊息
        /// </summary>
        [HttpGet("chat/messages")]
        public async Task<ActionResult<List<ChatMessageReadModel>>> GetChatMessages(
            [FromQuery] int? senderId = null,
            [FromQuery] int? receiverId = null,
            [FromQuery] int limit = 50)
        {
            try
            {
                var messages = await _communityRepository.GetChatMessagesAsync(senderId, receiverId, limit);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得聊天訊息失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得群組列表
        /// </summary>
        [HttpGet("users/{userId}/groups")]
        public async Task<ActionResult<List<GroupReadModel>>> GetUserGroups(int userId)
        {
            try
            {
                var groups = await _communityRepository.GetUserGroupsAsync(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得用戶群組列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得群組詳情
        /// </summary>
        [HttpGet("groups/{groupId}")]
        public async Task<ActionResult<GroupReadModel>> GetGroupById(int groupId)
        {
            try
            {
                var group = await _communityRepository.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound($"找不到群組 ID: {groupId}");
                }
                return Ok(group);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得群組詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得群組成員列表
        /// </summary>
        [HttpGet("groups/{groupId}/members")]
        public async Task<ActionResult<List<GroupMemberReadModel>>> GetGroupMembers(int groupId)
        {
            try
            {
                var members = await _communityRepository.GetGroupMembersAsync(groupId);
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得群組成員列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶好友關係
        /// </summary>
        [HttpGet("users/{userId}/relations")]
        public async Task<ActionResult<List<RelationReadModel>>> GetUserRelations(
            int userId,
            [FromQuery] int? statusId = null)
        {
            try
            {
                var relations = await _communityRepository.GetUserRelationsAsync(userId, statusId);
                return Ok(relations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得用戶好友關係失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得關係狀態列表
        /// </summary>
        [HttpGet("relation-statuses")]
        public async Task<ActionResult<List<RelationStatusReadModel>>> GetRelationStatuses()
        {
            try
            {
                var statuses = await _communityRepository.GetRelationStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得關係狀態列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得統計資訊
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats(
            [FromQuery] int? forumId = null,
            [FromQuery] long? threadId = null,
            [FromQuery] string? targetType = null,
            [FromQuery] long? targetId = null)
        {
            try
            {
                var stats = new Dictionary<string, object>();

                if (forumId.HasValue)
                {
                    stats["threadCount"] = await _communityRepository.GetThreadCountAsync(forumId.Value);
                }

                if (threadId.HasValue)
                {
                    stats["postCount"] = await _communityRepository.GetPostCountAsync(threadId.Value);
                }

                if (!string.IsNullOrEmpty(targetType) && targetId.HasValue)
                {
                    stats["reactionCount"] = await _communityRepository.GetReactionCountAsync(targetType, targetId.Value);
                }

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得統計資訊失敗: {ex.Message}");
            }
        }
    }
}
