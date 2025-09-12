using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameSpace.Services.Social
{
    public class SocialService : ISocialService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<SocialService> _logger;

        public SocialService(GameSpacedatabaseContext context, ILogger<SocialService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region 群組管理

        public async Task<GroupResult> CreateGroupAsync(int userId, string groupName, string description, bool isPublic)
        {
            try
            {
                var group = new Group
                {
                    GroupName = groupName,
                    Description = description,
                    IsPublic = isPublic,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                // 創建者自動成為管理員
                var groupMember = new GroupMember
                {
                    GroupId = group.GroupId,
                    UserId = userId,
                    Role = "Admin",
                    JoinedAt = DateTime.UtcNow
                };

                _context.GroupMembers.Add(groupMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group created: {GroupName} (ID: {GroupId}) by user {UserId}", groupName, group.GroupId, userId);
                return new GroupResult { Success = true, Message = "群組創建成功", Group = group };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create group: {GroupName}", groupName);
                return new GroupResult { Success = false, Message = "群組創建失敗" };
            }
        }

        public async Task<GroupResult> GetGroupAsync(int groupId)
        {
            try
            {
                var group = await _context.Groups
                    .Include(g => g.CreatedByNavigation)
                    .Include(g => g.GroupMembers)
                    .FirstOrDefaultAsync(g => g.GroupId == groupId);

                if (group == null)
                {
                    return new GroupResult { Success = false, Message = "群組不存在" };
                }

                return new GroupResult { Success = true, Message = "群組獲取成功", Group = group };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group: {GroupId}", groupId);
                return new GroupResult { Success = false, Message = "群組獲取失敗" };
            }
        }

        public async Task<List<Group>> GetUserGroupsAsync(int userId)
        {
            try
            {
                return await _context.GroupMembers
                    .Where(gm => gm.UserId == userId)
                    .Include(gm => gm.Group)
                    .Select(gm => gm.Group)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user groups for user: {UserId}", userId);
                return new List<Group>();
            }
        }

        public async Task<List<Group>> GetPublicGroupsAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.Groups
                    .Where(g => g.IsPublic)
                    .Include(g => g.CreatedByNavigation)
                    .Include(g => g.GroupMembers)
                    .OrderByDescending(g => g.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get public groups");
                return new List<Group>();
            }
        }

        public async Task<GroupResult> UpdateGroupAsync(int groupId, string groupName, string description, bool isPublic)
        {
            try
            {
                var group = await _context.Groups.FindAsync(groupId);
                if (group == null)
                {
                    return new GroupResult { Success = false, Message = "群組不存在" };
                }

                group.GroupName = groupName;
                group.Description = description;
                group.IsPublic = isPublic;

                _context.Groups.Update(group);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group updated: {GroupId}", groupId);
                return new GroupResult { Success = true, Message = "群組更新成功", Group = group };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update group: {GroupId}", groupId);
                return new GroupResult { Success = false, Message = "群組更新失敗" };
            }
        }

        public async Task<GroupResult> DeleteGroupAsync(int groupId)
        {
            try
            {
                var group = await _context.Groups.FindAsync(groupId);
                if (group == null)
                {
                    return new GroupResult { Success = false, Message = "群組不存在" };
                }

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group deleted: {GroupId}", groupId);
                return new GroupResult { Success = true, Message = "群組刪除成功" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete group: {GroupId}", groupId);
                return new GroupResult { Success = false, Message = "群組刪除失敗" };
            }
        }

        #endregion

        #region 群組成員管理

        public async Task<GroupMemberResult> JoinGroupAsync(int groupId, int userId)
        {
            try
            {
                // 檢查是否已經是成員
                var existingMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

                if (existingMember != null)
                {
                    return new GroupMemberResult { Success = false, Message = "您已經是群組成員" };
                }

                var groupMember = new GroupMember
                {
                    GroupId = groupId,
                    UserId = userId,
                    Role = "Member",
                    JoinedAt = DateTime.UtcNow
                };

                _context.GroupMembers.Add(groupMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} joined group {GroupId}", userId, groupId);
                return new GroupMemberResult { Success = true, Message = "成功加入群組", GroupMember = groupMember };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to join group: {GroupId} by user: {UserId}", groupId, userId);
                return new GroupMemberResult { Success = false, Message = "加入群組失敗" };
            }
        }

        public async Task<GroupMemberResult> LeaveGroupAsync(int groupId, int userId)
        {
            try
            {
                var groupMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

                if (groupMember == null)
                {
                    return new GroupMemberResult { Success = false, Message = "您不是群組成員" };
                }

                _context.GroupMembers.Remove(groupMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} left group {GroupId}", userId, groupId);
                return new GroupMemberResult { Success = true, Message = "成功離開群組" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to leave group: {GroupId} by user: {UserId}", groupId, userId);
                return new GroupMemberResult { Success = false, Message = "離開群組失敗" };
            }
        }

        public async Task<List<GroupMember>> GetGroupMembersAsync(int groupId)
        {
            try
            {
                return await _context.GroupMembers
                    .Where(gm => gm.GroupId == groupId)
                    .Include(gm => gm.User)
                    .OrderBy(gm => gm.Role)
                    .ThenBy(gm => gm.JoinedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group members for group: {GroupId}", groupId);
                return new List<GroupMember>();
            }
        }

        public async Task<GroupMemberResult> PromoteToAdminAsync(int groupId, int userId)
        {
            try
            {
                var groupMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

                if (groupMember == null)
                {
                    return new GroupMemberResult { Success = false, Message = "用戶不是群組成員" };
                }

                groupMember.Role = "Admin";
                _context.GroupMembers.Update(groupMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} promoted to admin in group {GroupId}", userId, groupId);
                return new GroupMemberResult { Success = true, Message = "用戶已提升為管理員", GroupMember = groupMember };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to promote user {UserId} to admin in group {GroupId}", userId, groupId);
                return new GroupMemberResult { Success = false, Message = "提升管理員失敗" };
            }
        }

        public async Task<GroupMemberResult> RemoveMemberAsync(int groupId, int userId)
        {
            try
            {
                var groupMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

                if (groupMember == null)
                {
                    return new GroupMemberResult { Success = false, Message = "用戶不是群組成員" };
                }

                _context.GroupMembers.Remove(groupMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} removed from group {GroupId}", userId, groupId);
                return new GroupMemberResult { Success = true, Message = "用戶已從群組中移除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user {UserId} from group {GroupId}", userId, groupId);
                return new GroupMemberResult { Success = false, Message = "移除成員失敗" };
            }
        }

        #endregion

        #region 聊天功能

        public async Task<ChatResult> SendMessageAsync(int groupId, int userId, string content, string messageType = "text")
        {
            try
            {
                var chatMessage = new ChatMessage
                {
                    GroupId = groupId,
                    SenderId = userId,
                    Content = content,
                    MessageType = messageType,
                    SentAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Message sent to group {GroupId} by user {UserId}", groupId, userId);
                return new ChatResult { Success = true, Message = "訊息發送成功", ChatMessage = chatMessage };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to group {GroupId} by user {UserId}", groupId, userId);
                return new ChatResult { Success = false, Message = "訊息發送失敗" };
            }
        }

        public async Task<List<ChatMessage>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 50)
        {
            try
            {
                return await _context.ChatMessages
                    .Where(cm => cm.GroupId == groupId)
                    .Include(cm => cm.Sender)
                    .OrderByDescending(cm => cm.SentAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group messages for group: {GroupId}", groupId);
                return new List<ChatMessage>();
            }
        }

        public async Task<List<ChatMessage>> GetUserMessagesAsync(int userId, int page = 1, int pageSize = 50)
        {
            try
            {
                return await _context.ChatMessages
                    .Where(cm => cm.SenderId == userId)
                    .Include(cm => cm.Group)
                    .OrderByDescending(cm => cm.SentAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user messages for user: {UserId}", userId);
                return new List<ChatMessage>();
            }
        }

        #endregion

        #region 好友關係

        public async Task<RelationResult> SendFriendRequestAsync(int fromUserId, int toUserId)
        {
            try
            {
                // 檢查是否已經有關係
                var existingRelation = await _context.Relations
                    .FirstOrDefaultAsync(r => (r.UserId == fromUserId && r.FriendId == toUserId) ||
                                             (r.UserId == toUserId && r.FriendId == fromUserId));

                if (existingRelation != null)
                {
                    return new RelationResult { Success = false, Message = "好友關係已存在" };
                }

                var relation = new Relation
                {
                    UserId = fromUserId,
                    FriendId = toUserId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Relations.Add(relation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Friend request sent from {FromUserId} to {ToUserId}", fromUserId, toUserId);
                return new RelationResult { Success = true, Message = "好友請求已發送", Relation = relation };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send friend request from {FromUserId} to {ToUserId}", fromUserId, toUserId);
                return new RelationResult { Success = false, Message = "好友請求發送失敗" };
            }
        }

        public async Task<RelationResult> AcceptFriendRequestAsync(int relationId)
        {
            try
            {
                var relation = await _context.Relations.FindAsync(relationId);
                if (relation == null)
                {
                    return new RelationResult { Success = false, Message = "好友請求不存在" };
                }

                relation.Status = "Accepted";
                relation.UpdatedAt = DateTime.UtcNow;

                _context.Relations.Update(relation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Friend request accepted: {RelationId}", relationId);
                return new RelationResult { Success = true, Message = "好友請求已接受", Relation = relation };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to accept friend request: {RelationId}", relationId);
                return new RelationResult { Success = false, Message = "接受好友請求失敗" };
            }
        }

        public async Task<RelationResult> RejectFriendRequestAsync(int relationId)
        {
            try
            {
                var relation = await _context.Relations.FindAsync(relationId);
                if (relation == null)
                {
                    return new RelationResult { Success = false, Message = "好友請求不存在" };
                }

                relation.Status = "Rejected";
                relation.UpdatedAt = DateTime.UtcNow;

                _context.Relations.Update(relation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Friend request rejected: {RelationId}", relationId);
                return new RelationResult { Success = true, Message = "好友請求已拒絕", Relation = relation };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reject friend request: {RelationId}", relationId);
                return new RelationResult { Success = false, Message = "拒絕好友請求失敗" };
            }
        }

        public async Task<RelationResult> RemoveFriendAsync(int relationId)
        {
            try
            {
                var relation = await _context.Relations.FindAsync(relationId);
                if (relation == null)
                {
                    return new RelationResult { Success = false, Message = "好友關係不存在" };
                }

                _context.Relations.Remove(relation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Friend removed: {RelationId}", relationId);
                return new RelationResult { Success = true, Message = "好友已移除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove friend: {RelationId}", relationId);
                return new RelationResult { Success = false, Message = "移除好友失敗" };
            }
        }

        public async Task<List<Relation>> GetFriendRequestsAsync(int userId)
        {
            try
            {
                return await _context.Relations
                    .Where(r => r.FriendId == userId && r.Status == "Pending")
                    .Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get friend requests for user: {UserId}", userId);
                return new List<Relation>();
            }
        }

        public async Task<List<Relation>> GetFriendsAsync(int userId)
        {
            try
            {
                return await _context.Relations
                    .Where(r => (r.UserId == userId || r.FriendId == userId) && r.Status == "Accepted")
                    .Include(r => r.User)
                    .Include(r => r.Friend)
                    .OrderBy(r => r.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get friends for user: {UserId}", userId);
                return new List<Relation>();
            }
        }

        public async Task<List<Relation>> GetSentRequestsAsync(int userId)
        {
            try
            {
                return await _context.Relations
                    .Where(r => r.UserId == userId && r.Status == "Pending")
                    .Include(r => r.Friend)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sent requests for user: {UserId}", userId);
                return new List<Relation>();
            }
        }

        #endregion

        #region 封鎖功能

        public async Task<BlockResult> BlockUserAsync(int userId, int blockedUserId)
        {
            try
            {
                var groupBlock = new GroupBlock
                {
                    UserId = userId,
                    BlockedUserId = blockedUserId,
                    BlockedAt = DateTime.UtcNow
                };

                _context.GroupBlocks.Add(groupBlock);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} blocked user {BlockedUserId}", userId, blockedUserId);
                return new BlockResult { Success = true, Message = "用戶已封鎖", GroupBlock = groupBlock };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to block user {BlockedUserId} by user {UserId}", blockedUserId, userId);
                return new BlockResult { Success = false, Message = "封鎖用戶失敗" };
            }
        }

        public async Task<BlockResult> UnblockUserAsync(int userId, int blockedUserId)
        {
            try
            {
                var groupBlock = await _context.GroupBlocks
                    .FirstOrDefaultAsync(gb => gb.UserId == userId && gb.BlockedUserId == blockedUserId);

                if (groupBlock == null)
                {
                    return new BlockResult { Success = false, Message = "用戶未被封鎖" };
                }

                _context.GroupBlocks.Remove(groupBlock);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} unblocked user {BlockedUserId}", userId, blockedUserId);
                return new BlockResult { Success = true, Message = "用戶已解除封鎖" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unblock user {BlockedUserId} by user {UserId}", blockedUserId, userId);
                return new BlockResult { Success = false, Message = "解除封鎖失敗" };
            }
        }

        public async Task<List<GroupBlock>> GetBlockedUsersAsync(int userId)
        {
            try
            {
                return await _context.GroupBlocks
                    .Where(gb => gb.UserId == userId)
                    .Include(gb => gb.BlockedByNavigation)
                    .OrderByDescending(gb => gb.BlockedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get blocked users for user: {UserId}", userId);
                return new List<GroupBlock>();
            }
        }

        #endregion

        #region 搜尋功能

        public async Task<SearchResult> SearchUsersAsync(string keyword, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Users
                    .Where(u => u.UserName.Contains(keyword) || u.UserAccount.Contains(keyword));

                var totalCount = await query.CountAsync();
                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new SearchResult
                {
                    Success = true,
                    Message = "搜尋成功",
                    Users = users,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search users with keyword: {Keyword}", keyword);
                return new SearchResult { Success = false, Message = "搜尋失敗" };
            }
        }

        public async Task<SearchResult> SearchGroupsAsync(string keyword, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Groups
                    .Where(g => g.GroupName.Contains(keyword) || g.Description.Contains(keyword))
                    .Include(g => g.CreatedByNavigation);

                var totalCount = await query.CountAsync();
                var groups = await query
                    .OrderByDescending(g => g.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new SearchResult
                {
                    Success = true,
                    Message = "搜尋成功",
                    Groups = groups,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search groups with keyword: {Keyword}", keyword);
                return new SearchResult { Success = false, Message = "搜尋失敗" };
            }
        }

        #endregion
    }
}