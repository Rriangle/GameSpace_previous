using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameSpace.Services.Social
{
    public interface ISocialService
    {
        // 群組管理
        Task<GroupResult> CreateGroupAsync(int userId, string groupName, string description, bool isPublic);
        Task<GroupResult> GetGroupAsync(int groupId);
        Task<List<Group>> GetUserGroupsAsync(int userId);
        Task<List<Group>> GetPublicGroupsAsync(int page = 1, int pageSize = 20);
        Task<GroupResult> UpdateGroupAsync(int groupId, string groupName, string description, bool isPublic);
        Task<GroupResult> DeleteGroupAsync(int groupId);
        
        // 群組成員管理
        Task<GroupMemberResult> JoinGroupAsync(int groupId, int userId);
        Task<GroupMemberResult> LeaveGroupAsync(int groupId, int userId);
        Task<List<GroupMember>> GetGroupMembersAsync(int groupId);
        Task<GroupMemberResult> PromoteToAdminAsync(int groupId, int userId);
        Task<GroupMemberResult> RemoveMemberAsync(int groupId, int userId);
        
        // 聊天功能
        Task<ChatResult> SendMessageAsync(int groupId, int userId, string content, string messageType = "text");
        Task<List<ChatMessage>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 50);
        Task<List<ChatMessage>> GetUserMessagesAsync(int userId, int page = 1, int pageSize = 50);
        
        // 好友關係
        Task<RelationResult> SendFriendRequestAsync(int fromUserId, int toUserId);
        Task<RelationResult> AcceptFriendRequestAsync(int relationId);
        Task<RelationResult> RejectFriendRequestAsync(int relationId);
        Task<RelationResult> RemoveFriendAsync(int relationId);
        Task<List<Relation>> GetFriendRequestsAsync(int userId);
        Task<List<Relation>> GetFriendsAsync(int userId);
        Task<List<Relation>> GetSentRequestsAsync(int userId);
        
        // 封鎖功能
        Task<BlockResult> BlockUserAsync(int userId, int blockedUserId);
        Task<BlockResult> UnblockUserAsync(int userId, int blockedUserId);
        Task<List<GroupBlock>> GetBlockedUsersAsync(int userId);
        
        // 搜尋功能
        Task<SearchResult> SearchUsersAsync(string keyword, int page = 1, int pageSize = 20);
        Task<SearchResult> SearchGroupsAsync(string keyword, int page = 1, int pageSize = 20);
    }

    public class GroupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Group? Group { get; set; }
    }

    public class GroupMemberResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public GroupMember? GroupMember { get; set; }
    }

    public class ChatResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ChatMessage? ChatMessage { get; set; }
    }

    public class RelationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Relation? Relation { get; set; }
    }

    public class BlockResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public GroupBlock? GroupBlock { get; set; }
    }

    public class SearchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<User>? Users { get; set; }
        public List<Group>? Groups { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}