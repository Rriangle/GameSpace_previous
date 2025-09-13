using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 好友和群組控制器
    /// </summary>
    public class FriendshipController : Controller
    {
        private readonly GameSpaceDbContext _context;

        public FriendshipController(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 好友列表頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var friendships = await _context.Friendships
                .Include(f => f.Friend)
                .Where(f => f.UserId == userId && f.Status == "Accepted" && f.IsActive)
                .OrderBy(f => f.Friend.Username)
                .ToListAsync();

            return View(friendships);
        }

        /// <summary>
        /// 好友請求頁面
        /// </summary>
        public async Task<IActionResult> Requests()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var pendingRequests = await _context.Friendships
                .Include(f => f.User)
                .Where(f => f.FriendId == userId && f.Status == "Pending" && f.IsActive)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(pendingRequests);
        }

        /// <summary>
        /// 群組列表頁面
        /// </summary>
        public async Task<IActionResult> Groups()
        {
            var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

            var groups = await _context.GroupMembers
                .Include(gm => gm.Group)
                .ThenInclude(g => g.OwnerUser)
                .Where(gm => gm.UserId == userId && gm.IsActive)
                .OrderBy(gm => gm.Group.GroupName)
                .ToListAsync();

            return View(groups);
        }

        /// <summary>
        /// 發送好友請求
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int friendId)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                // 檢查是否已經是好友
                var existingFriendship = await _context.Friendships
                    .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || 
                                            (f.UserId == friendId && f.FriendId == userId));

                if (existingFriendship != null)
                {
                    return Json(new { success = false, message = "已經是好友關係" });
                }

                // 檢查是否已經發送過請求
                var existingRequest = await _context.Friendships
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId && f.Status == "Pending");

                if (existingRequest != null)
                {
                    return Json(new { success = false, message = "已經發送過好友請求" });
                }

                var friendship = new Friendship
                {
                    UserId = userId,
                    FriendId = friendId,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Friendships.Add(friendship);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "好友請求發送成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"發送失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 接受好友請求
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int friendshipId)
        {
            try
            {
                var friendship = await _context.Friendships
                    .FirstOrDefaultAsync(f => f.FriendshipId == friendshipId);

                if (friendship == null)
                {
                    return Json(new { success = false, message = "好友請求不存在" });
                }

                if (friendship.Status != "Pending")
                {
                    return Json(new { success = false, message = "好友請求狀態不正確" });
                }

                friendship.Status = "Accepted";
                friendship.AcceptedAt = DateTime.Now;

                // 創建反向好友關係
                var reverseFriendship = new Friendship
                {
                    UserId = friendship.FriendId,
                    FriendId = friendship.UserId,
                    Status = "Accepted",
                    CreatedAt = DateTime.Now,
                    AcceptedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Friendships.Add(reverseFriendship);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "好友請求已接受" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"接受失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 拒絕好友請求
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeclineFriendRequest(int friendshipId)
        {
            try
            {
                var friendship = await _context.Friendships
                    .FirstOrDefaultAsync(f => f.FriendshipId == friendshipId);

                if (friendship == null)
                {
                    return Json(new { success = false, message = "好友請求不存在" });
                }

                friendship.Status = "Declined";
                friendship.IsActive = false;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "好友請求已拒絕" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"拒絕失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 刪除好友
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                var friendships = await _context.Friendships
                    .Where(f => (f.UserId == userId && f.FriendId == friendId) || 
                               (f.UserId == friendId && f.FriendId == userId))
                    .ToListAsync();

                foreach (var friendship in friendships)
                {
                    friendship.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "好友已刪除" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"刪除失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 創建群組
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                var group = new Groups
                {
                    OwnerUserId = userId,
                    GroupName = request.GroupName,
                    Description = request.Description,
                    IsPrivate = request.IsPrivate,
                    CreatedAt = DateTime.Now,
                    MaxMembers = request.MaxMembers,
                    IsActive = true
                };

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                // 將創建者添加為群組成員
                var groupMember = new GroupMember
                {
                    GroupId = group.GroupId,
                    UserId = userId,
                    Role = "Owner",
                    JoinedAt = DateTime.Now,
                    IsActive = true
                };

                _context.GroupMembers.Add(groupMember);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "群組創建成功", groupId = group.GroupId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"創建失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 加入群組
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                // 檢查是否已經是群組成員
                var existingMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

                if (existingMember != null)
                {
                    return Json(new { success = false, message = "已經是群組成員" });
                }

                var groupMember = new GroupMember
                {
                    GroupId = groupId,
                    UserId = userId,
                    Role = "Member",
                    JoinedAt = DateTime.Now,
                    IsActive = true
                };

                _context.GroupMembers.Add(groupMember);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "成功加入群組" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"加入失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 離開群組
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            try
            {
                var userId = 1; // 暫時使用固定用戶ID，實際應從認證中獲取

                var groupMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

                if (groupMember == null)
                {
                    return Json(new { success = false, message = "不是群組成員" });
                }

                groupMember.IsActive = false;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "已離開群組" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"離開失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 獲取好友統計
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFriendStats(int userId)
        {
            var stats = new
            {
                TotalFriends = await _context.Friendships
                    .CountAsync(f => f.UserId == userId && f.Status == "Accepted" && f.IsActive),
                PendingRequests = await _context.Friendships
                    .CountAsync(f => f.FriendId == userId && f.Status == "Pending" && f.IsActive),
                SentRequests = await _context.Friendships
                    .CountAsync(f => f.UserId == userId && f.Status == "Pending" && f.IsActive)
            };

            return Json(stats);
        }
    }

    /// <summary>
    /// 創建群組請求模型
    /// </summary>
    public class CreateGroupRequest
    {
        public string GroupName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPrivate { get; set; }
        public int? MaxMembers { get; set; }
    }
}