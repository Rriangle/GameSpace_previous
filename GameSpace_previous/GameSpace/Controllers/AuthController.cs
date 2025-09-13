using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;
using System.Security.Cryptography;
using System.Text;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 會員認證控制器
    /// </summary>
    public class AuthController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(GameSpaceDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示登入頁面
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 處理登入請求
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 查找用戶
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserAccount == model.Account || u.UserEmail == model.Account);

                if (user == null)
                {
                    ModelState.AddModelError("", "帳號或密碼錯誤");
                    return View(model);
                }

                // 驗證密碼
                if (!VerifyPassword(model.Password, user.UserPassword))
                {
                    ModelState.AddModelError("", "帳號或密碼錯誤");
                    return View(model);
                }

                // 檢查帳號狀態
                if (user.UserStatus != "Active")
                {
                    ModelState.AddModelError("", "帳號已被停用");
                    return View(model);
                }

                // 設置會話
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("UserAccount", user.UserAccount);

                _logger.LogInformation("用戶 {UserAccount} 登入成功", user.UserAccount);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登入過程中發生錯誤");
                ModelState.AddModelError("", "登入過程中發生錯誤，請稍後再試");
                return View(model);
            }
        }

        /// <summary>
        /// 顯示註冊頁面
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 處理註冊請求
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 檢查帳號是否已存在
                if (await _context.Users.AnyAsync(u => u.UserAccount == model.Account))
                {
                    ModelState.AddModelError("Account", "此帳號已被使用");
                    return View(model);
                }

                // 檢查Email是否已存在
                if (await _context.Users.AnyAsync(u => u.UserEmail == model.Email))
                {
                    ModelState.AddModelError("Email", "此Email已被使用");
                    return View(model);
                }

                // 創建新用戶
                var user = new Users
                {
                    UserAccount = model.Account,
                    UserPassword = HashPassword(model.Password),
                    UserName = model.UserName,
                    UserEmail = model.Email,
                    UserPhoneNumber = model.Phone,
                    UserAvatar = "https://example.com/default-avatar.jpg",
                    UserStatus = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("新用戶 {UserAccount} 註冊成功", user.UserAccount);

                TempData["SuccessMessage"] = "註冊成功！請登入您的帳號";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "註冊過程中發生錯誤");
                ModelState.AddModelError("", "註冊過程中發生錯誤，請稍後再試");
                return View(model);
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("用戶已登出");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 雜湊密碼
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// 驗證密碼
        /// </summary>
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }

    /// <summary>
    /// 登入視圖模型
    /// </summary>
    public class LoginViewModel
    {
        public string Account { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 註冊視圖模型
    /// </summary>
    public class RegisterViewModel
    {
        public string Account { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}