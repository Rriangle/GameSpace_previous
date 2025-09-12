using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Admin;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IAdminService adminService, ILogger<DashboardController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stats = await _adminService.GetSystemStatsAsync();
            var newUsers = await _adminService.GetNewUsersAsync(7);
            var recentOrders = await _adminService.GetRecentOrdersAsync(7);
            var recentPosts = await _adminService.GetRecentPostsAsync(7);

            var viewModel = new DashboardViewModel
            {
                SystemStats = stats,
                NewUsers = newUsers,
                RecentOrders = recentOrders,
                RecentPosts = recentPosts
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users(int page = 1)
        {
            var users = await _adminService.GetAllUsersAsync(page, 20);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            var result = await _adminService.GetUserAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Users");
            }

            return View(result.User);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserStatus(int userId, bool isActive)
        {
            var result = await _adminService.UpdateUserStatusAsync(userId, isActive);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> LockUser(int userId, int? lockoutDays)
        {
            DateTime? lockoutEnd = null;
            if (lockoutDays.HasValue)
            {
                lockoutEnd = DateTime.UtcNow.AddDays(lockoutDays.Value);
            }

            var result = await _adminService.LockUserAsync(userId, lockoutEnd);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> UnlockUser(int userId)
        {
            var result = await _adminService.UnlockUserAsync(userId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPassword(int userId, string newPassword)
        {
            var result = await _adminService.ResetUserPasswordAsync(userId, newPassword);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> Content()
        {
            var recentPosts = await _adminService.GetRecentPostsAsync(30);
            return View(recentPosts);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var result = await _adminService.DeletePostAsync(postId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteThread(int threadId)
        {
            var result = await _adminService.DeleteThreadAsync(threadId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(int userId, string reason, int? banDays)
        {
            DateTime? banUntil = null;
            if (banDays.HasValue)
            {
                banUntil = DateTime.UtcNow.AddDays(banDays.Value);
            }

            var result = await _adminService.BanUserAsync(userId, reason, banUntil);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            var result = await _adminService.UnbanUserAsync(userId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var settings = await _adminService.GetAllSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSetting(string key, string value)
        {
            var result = await _adminService.UpdateSystemSettingAsync(key, value);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> Logs(int page = 1)
        {
            var logs = await _adminService.GetSystemLogsAsync(page, 50);
            return View(logs);
        }

        [HttpPost]
        public async Task<IActionResult> ClearOldLogs(int daysToKeep = 30)
        {
            var result = await _adminService.ClearOldLogsAsync(daysToKeep);
            return Json(new { success = result.Success, message = result.Message });
        }
    }

    public class DashboardViewModel
    {
        public SystemStatsResult SystemStats { get; set; } = null!;
        public List<User> NewUsers { get; set; } = new List<User>();
        public List<OrderInfo> RecentOrders { get; set; } = new List<OrderInfo>();
        public List<Post> RecentPosts { get; set; } = new List<Post>();
    }
}