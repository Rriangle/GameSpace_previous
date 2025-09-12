using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Authentication;
using GameSpace.Models;
using System.Threading.Tasks;
using System;

namespace GameSpace.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string account, string password)
        {
            try
            {
                var result = await _authService.LoginUserAsync(account, password);
                if (result.Success)
                {
                    // TODO: Set authentication cookie or JWT token
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for account: {Account}", account);
                TempData["ErrorMessage"] = "Login failed. Please try again later.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = new User
                {
                    UserAccount = model.Account,
                    UserPassword = model.Password,
                    UserEmailConfirmed = false,
                    UserPhoneNumberConfirmed = false,
                    UserTwoFactorEnabled = false,
                    UserAccessFailedCount = 0,
                    UserLockoutEnabled = false,
                    UserLockoutEnd = null
                };

                var userIntroduce = new UserIntroduce
                {
                    UserNickName = model.NickName,
                    Gender = model.Gender,
                    IdNumber = model.IdNumber,
                    Cellphone = model.Cellphone,
                    Email = model.Email,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    UserIntroduce1 = model.Introduction
                };

                var userRight = new UserRight
                {
                    UserStatus = true,
                    ShoppingPermission = true,
                    MessagePermission = true,
                    SalesAuthority = false
                };

                var result = await _authService.RegisterUserAsync(user, userIntroduce, userRight);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for account: {Account}", model.Account);
                TempData["ErrorMessage"] = "Registration failed. Please try again later.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // TODO: Clear authentication cookie or JWT token
            TempData["SuccessMessage"] = "Successfully logged out.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var user = await _authService.GetUserByAccountAsync(email);
                if (user != null)
                {
                    // TODO: Send password reset email
                    TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Email address not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset failed for email: {Email}", email);
                TempData["ErrorMessage"] = "密碼重設失敗，請稍後再試。";
            }

            return View();
        }
    }

    public class RegisterViewModel
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string? Introduction { get; set; }
    }
}