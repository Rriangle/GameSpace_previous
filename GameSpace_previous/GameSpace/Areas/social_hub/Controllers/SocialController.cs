using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Social;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.social_hub.Controllers
{
    [Area("social_hub")]
    public class SocialController : Controller
    {
        private readonly ISocialService _socialService;
        private readonly ILogger<SocialController> _logger;

        public SocialController(ISocialService socialService, ILogger<SocialController> logger)
        {
            _socialService = socialService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var userGroups = await _socialService.GetUserGroupsAsync(userId);
            var publicGroups = await _socialService.GetPublicGroupsAsync(1, 10);
            var friends = await _socialService.GetFriendsAsync(userId);
            var friendRequests = await _socialService.GetFriendRequestsAsync(userId);

            var viewModel = new SocialIndexViewModel
            {
                UserGroups = userGroups,
                PublicGroups = publicGroups,
                Friends = friends,
                FriendRequests = friendRequests
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Groups(int page = 1)
        {
            var publicGroups = await _socialService.GetPublicGroupsAsync(page, 20);
            return View(publicGroups);
        }

        [HttpGet]
        public IActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _socialService.CreateGroupAsync(userId, model.GroupName, model.Description, model.IsPublic);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("GroupDetails", new { id = result.Group!.GroupId });
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GroupDetails(int id)
        {
            var result = await _socialService.GetGroupAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Groups");
            }

            var members = await _socialService.GetGroupMembersAsync(id);
            var messages = await _socialService.GetGroupMessagesAsync(id, 1, 50);

            var viewModel = new GroupDetailsViewModel
            {
                Group = result.Group!,
                Members = members,
                Messages = messages
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _socialService.JoinGroupAsync(groupId, userId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _socialService.LeaveGroupAsync(groupId, userId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int groupId, string content)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _socialService.SendMessageAsync(groupId, userId, content);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> Friends()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var friends = await _socialService.GetFriendsAsync(userId);
            var friendRequests = await _socialService.GetFriendRequestsAsync(userId);
            var sentRequests = await _socialService.GetSentRequestsAsync(userId);

            var viewModel = new FriendsViewModel
            {
                Friends = friends,
                FriendRequests = friendRequests,
                SentRequests = sentRequests
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int toUserId)
        {
            // TODO: Get current user ID from authentication
            int fromUserId = 1; // Placeholder

            var result = await _socialService.SendFriendRequestAsync(fromUserId, toUserId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int relationId)
        {
            var result = await _socialService.AcceptFriendRequestAsync(relationId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(int relationId)
        {
            var result = await _socialService.RejectFriendRequestAsync(relationId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int relationId)
        {
            var result = await _socialService.RemoveFriendAsync(relationId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchUsers(string keyword, int page = 1)
        {
            var result = await _socialService.SearchUsersAsync(keyword, page, 20);
            return Json(new { success = result.Success, users = result.Users, totalCount = result.TotalCount });
        }

        [HttpPost]
        public async Task<IActionResult> SearchGroups(string keyword, int page = 1)
        {
            var result = await _socialService.SearchGroupsAsync(keyword, page, 20);
            return Json(new { success = result.Success, groups = result.Groups, totalCount = result.TotalCount });
        }
    }

    public class SocialIndexViewModel
    {
        public List<Group> UserGroups { get; set; } = new List<Group>();
        public List<Group> PublicGroups { get; set; } = new List<Group>();
        public List<Relation> Friends { get; set; } = new List<Relation>();
        public List<Relation> FriendRequests { get; set; } = new List<Relation>();
    }

    public class GroupDetailsViewModel
    {
        public Group Group { get; set; } = null!;
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    public class CreateGroupViewModel
    {
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = true;
    }

    public class FriendsViewModel
    {
        public List<Relation> Friends { get; set; } = new List<Relation>();
        public List<Relation> FriendRequests { get; set; } = new List<Relation>();
        public List<Relation> SentRequests { get; set; } = new List<Relation>();
    }
}