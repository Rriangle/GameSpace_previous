using GameSpace.Data;
using GameSpace.Models;
using GameSpace.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Microsoft;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameSpace.Controllers
{
    /// <summary>
    /// OAuth認證控制器
    /// </summary>
    public class OAuthController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly OAuthService _oauthService;
        private readonly ILogger<OAuthController> _logger;

        public OAuthController(GameSpaceDbContext context, OAuthService oauthService, ILogger<OAuthController> logger)
        {
            _context = context;
            _oauthService = oauthService;
            _logger = logger;
        }

        /// <summary>
        /// 開始Google OAuth登入
        /// </summary>
        /// <param name="returnUrl">登入後重定向URL</param>
        /// <returns>重定向到Google OAuth</returns>
        [HttpGet]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "OAuth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Google OAuth回調
        /// </summary>
        /// <param name="returnUrl">登入後重定向URL</param>
        /// <returns>重定向到指定頁面</returns>
        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Google OAuth認證失敗");
                return RedirectToAction("Login", "Account", new { error = "google_auth_failed" });
            }

            var claims = result.Principal?.Claims;
            if (claims == null)
            {
                _logger.LogWarning("Google OAuth未返回用戶資訊");
                return RedirectToAction("Login", "Account", new { error = "no_user_info" });
            }

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
            {
                _logger.LogWarning("Google OAuth缺少必要資訊：Email或Google ID");
                return RedirectToAction("Login", "Account", new { error = "missing_info" });
            }

            // 查找或創建用戶
            var user = await FindOrCreateUserAsync(email, name, "Google", googleId);
            if (user == null)
            {
                _logger.LogError("無法創建或找到用戶：{Email}", email);
                return RedirectToAction("Login", "Account", new { error = "user_creation_failed" });
            }

            // 儲存OAuth令牌
            var accessToken = claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _oauthService.SaveTokenAsync(user.UserId, "Google", "access_token", accessToken, DateTime.UtcNow.AddHours(1));
            }

            // 登入用戶
            await SignInUserAsync(user);

            _logger.LogInformation("用戶 {UserId} 透過Google OAuth成功登入", user.UserId);
            return Redirect(returnUrl ?? "/");
        }

        /// <summary>
        /// 開始Facebook OAuth登入
        /// </summary>
        /// <param name="returnUrl">登入後重定向URL</param>
        /// <returns>重定向到Facebook OAuth</returns>
        [HttpGet]
        public IActionResult FacebookLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(FacebookCallback), "OAuth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Facebook OAuth回調
        /// </summary>
        /// <param name="returnUrl">登入後重定向URL</param>
        /// <returns>重定向到指定頁面</returns>
        [HttpGet]
        public async Task<IActionResult> FacebookCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Facebook OAuth認證失敗");
                return RedirectToAction("Login", "Account", new { error = "facebook_auth_failed" });
            }

            var claims = result.Principal?.Claims;
            if (claims == null)
            {
                _logger.LogWarning("Facebook OAuth未返回用戶資訊");
                return RedirectToAction("Login", "Account", new { error = "no_user_info" });
            }

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var facebookId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(facebookId))
            {
                _logger.LogWarning("Facebook OAuth缺少必要資訊：Email或Facebook ID");
                return RedirectToAction("Login", "Account", new { error = "missing_info" });
            }

            // 查找或創建用戶
            var user = await FindOrCreateUserAsync(email, name, "Facebook", facebookId);
            if (user == null)
            {
                _logger.LogError("無法創建或找到用戶：{Email}", email);
                return RedirectToAction("Login", "Account", new { error = "user_creation_failed" });
            }

            // 儲存OAuth令牌
            var accessToken = claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _oauthService.SaveTokenAsync(user.UserId, "Facebook", "access_token", accessToken, DateTime.UtcNow.AddHours(2));
            }

            // 登入用戶
            await SignInUserAsync(user);

            _logger.LogInformation("用戶 {UserId} 透過Facebook OAuth成功登入", user.UserId);
            return Redirect(returnUrl ?? "/");
        }

        /// <summary>
        /// 開始Microsoft OAuth登入
        /// </summary>
        /// <param name="returnUrl">登入後重定向URL</param>
        /// <returns>重定向到Microsoft OAuth</returns>
        [HttpGet]
        public IActionResult MicrosoftLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(MicrosoftCallback), "OAuth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Microsoft OAuth回調
        /// </summary>
        /// <param name="returnUrl">登入後重定向URL</param>
        /// <returns>重定向到指定頁面</returns>
        [HttpGet]
        public async Task<IActionResult> MicrosoftCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Microsoft OAuth認證失敗");
                return RedirectToAction("Login", "Account", new { error = "microsoft_auth_failed" });
            }

            var claims = result.Principal?.Claims;
            if (claims == null)
            {
                _logger.LogWarning("Microsoft OAuth未返回用戶資訊");
                return RedirectToAction("Login", "Account", new { error = "no_user_info" });
            }

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var microsoftId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(microsoftId))
            {
                _logger.LogWarning("Microsoft OAuth缺少必要資訊：Email或Microsoft ID");
                return RedirectToAction("Login", "Account", new { error = "missing_info" });
            }

            // 查找或創建用戶
            var user = await FindOrCreateUserAsync(email, name, "Microsoft", microsoftId);
            if (user == null)
            {
                _logger.LogError("無法創建或找到用戶：{Email}", email);
                return RedirectToAction("Login", "Account", new { error = "user_creation_failed" });
            }

            // 儲存OAuth令牌
            var accessToken = claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _oauthService.SaveTokenAsync(user.UserId, "Microsoft", "access_token", accessToken, DateTime.UtcNow.AddHours(1));
            }

            // 登入用戶
            await SignInUserAsync(user);

            _logger.LogInformation("用戶 {UserId} 透過Microsoft OAuth成功登入", user.UserId);
            return Redirect(returnUrl ?? "/");
        }

        /// <summary>
        /// 取消綁定OAuth提供者
        /// </summary>
        /// <param name="provider">提供者名稱</param>
        /// <returns>操作結果</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlinkProvider(string provider)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Json(new { success = false, message = "無效的用戶ID" });
            }

            var success = await _oauthService.UnlinkProviderAsync(userId, provider);
            if (success)
            {
                _logger.LogInformation("用戶 {UserId} 已取消綁定 {Provider}", userId, provider);
                return Json(new { success = true, message = $"已取消綁定{provider}" });
            }

            return Json(new { success = false, message = "取消綁定失敗" });
        }

        /// <summary>
        /// 查找或創建用戶
        /// </summary>
        /// <param name="email">電子郵件</param>
        /// <param name="name">姓名</param>
        /// <param name="provider">提供者</param>
        /// <param name="externalId">外部ID</param>
        /// <returns>用戶對象</returns>
        private async Task<Users?> FindOrCreateUserAsync(string email, string? name, string provider, string externalId)
        {
            try
            {
                // 先嘗試通過email查找現有用戶
                var existingUser = await _context.Users
                    .Include(u => u.UserIntroduce)
                    .Include(u => u.UserRights)
                    .FirstOrDefaultAsync(u => u.UserEmail == email);

                if (existingUser != null)
                {
                    // 儲存外部ID到UserTokens表
                    await _oauthService.SaveTokenAsync(existingUser.UserId, provider, "external_id", externalId, DateTime.UtcNow.AddYears(1));
                    return existingUser;
                }

                // 創建新用戶
                var newUser = new Users
                {
                    UserAccount = email, // 使用email作為帳號
                    UserEmail = email,
                    UserPassword = OAuthService.GenerateRandomString(64), // 生成隨機密碼
                    UserEmailConfirmed = true, // OAuth用戶默認已驗證email
                    UserAccessFailedCount = 0,
                    UserLockoutEnabled = false,
                    UserLockoutEnd = null
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // 創建用戶詳細資料
                var userIntroduce = new UserIntroduce
                {
                    UserId = newUser.UserId,
                    UserNickName = name ?? email.Split('@')[0],
                    Gender = "未設定",
                    IdNumber = "未設定",
                    Cellphone = "未設定",
                    Email = email,
                    Address = "未設定",
                    DateOfBirth = DateTime.Now.AddYears(-18), // 默認18歲
                    CreateAccount = DateTime.UtcNow,
                    UserIntroduce = "透過OAuth註冊的用戶"
                };

                _context.UserIntroduces.Add(userIntroduce);

                // 創建用戶權限
                var userRights = new UserRight
                {
                    UserId = newUser.UserId,
                    UserStatus = true,
                    ShoppingPermission = true,
                    MessagePermission = true,
                    SalesAuthority = false
                };

                _context.UserRights.Add(userRights);

                // 儲存外部ID
                await _oauthService.SaveTokenAsync(newUser.UserId, provider, "external_id", externalId, DateTime.UtcNow.AddYears(1));

                await _context.SaveChangesAsync();

                _logger.LogInformation("已創建新OAuth用戶：{Email}, 提供者：{Provider}", email, provider);
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查找或創建OAuth用戶時發生錯誤：{Email}", email);
                return null;
            }
        }

        /// <summary>
        /// 登入用戶
        /// </summary>
        /// <param name="user">用戶對象</param>
        private async Task SignInUserAsync(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserAccount),
                new Claim(ClaimTypes.Email, user.UserEmail ?? ""),
                new Claim("UserAccount", user.UserAccount)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}