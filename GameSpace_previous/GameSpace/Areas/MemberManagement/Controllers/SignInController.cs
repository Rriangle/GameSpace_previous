using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.SignIn;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.MemberManagement.Controllers
{
    [Area("MemberManagement")]
    public class SignInController : Controller
    {
        private readonly ISignInService _signInService;
        private readonly ILogger<SignInController> _logger;

        public SignInController(ISignInService signInService, ILogger<SignInController> logger)
        {
            _signInService = signInService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var hasSignedInToday = await _signInService.HasUserSignedInTodayAsync(userId);
            var consecutiveDays = await _signInService.GetConsecutiveSignInDaysAsync(userId);
            var signInHistory = await _signInService.GetUserSignInHistoryAsync(userId, 1, 10);

            var viewModel = new SignInViewModel
            {
                HasSignedInToday = hasSignedInToday,
                ConsecutiveDays = consecutiveDays,
                SignInHistory = signInHistory
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformSignIn()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _signInService.PerformDailySignInAsync(userId);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Daily sign-in failed for user {UserId}", userId);
                TempData["ErrorMessage"] = "簽到失敗，請稍後再試。";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> History(int page = 1)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var signInHistory = await _signInService.GetUserSignInHistoryAsync(userId, page, 20);
            return View(signInHistory);
        }
    }

    public class SignInViewModel
    {
        public bool HasSignedInToday { get; set; }
        public int ConsecutiveDays { get; set; }
        public IEnumerable<UserSignInStat> SignInHistory { get; set; } = new List<UserSignInStat>();
    }
}