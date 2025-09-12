using GameSpace.Areas.social_hub.Models;
using GameSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Areas.social_hub.Controllers
{
	[Area("social_hub")]
	public class HomeController : Controller
	{
		private readonly GameSpacedatabaseContext _context;
		public HomeController(GameSpacedatabaseContext context) => _context = context;

		// Home page
		public IActionResult Index() => View();

		// ======== General user login ========
		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = await _context.Users
				.FirstOrDefaultAsync(u =>
					(u.UserAccount != null && u.UserAccount == model.Account)
				 || (u.UserName != null && u.UserName == model.Account));

			if (user == null || user.UserPassword != model.Password)
			{
				ModelState.AddModelError(string.Empty, "Account or password is incorrect");
				return View(model);
			}

			var options = new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Lax,
				Expires = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8),
				Path = "/"
			};
			// Set user Cookie
			Response.Cookies.Append("sh_uid", user.UserId.ToString(), options);
			Response.Cookies.Append("sh_uname", user.UserName ?? user.UserAccount ?? "", options);
			// Avoid identity confusion: clear admin Cookie (can keep or delete, recommended to clear)
			Response.Cookies.Delete("sh_is_admin");
			Response.Cookies.Delete("sh_mid");
			Response.Cookies.Delete("sh_mname");

			if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
				return Redirect(model.ReturnUrl);

			return RedirectToAction(nameof(Index));
		}

		// ======== Admin login ========
		[HttpGet]
		public IActionResult AdminLogin(string? returnUrl = null)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AdminLogin(LoginViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			// Assume your admin main record is ManagerData (EF entity is usually ManagerDatum, DbSet is called ManagerData)
			// Fields: ManagerAccount / ManagerPassword / ManagerName / ManagerId
			var manager = await _context.ManagerData
				.FirstOrDefaultAsync(m => m.ManagerAccount == model.Account);

			if (manager == null || manager.ManagerPassword != model.Password)
			{
				ModelState.AddModelError(string.Empty, "Account or password is incorrect (Admin)");
				return View(model);
			}

			

			

			var options = new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Lax,
				Expires = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8),
				Path = "/"
			};
			// Set admin Cookie
			Response.Cookies.Append("sh_is_admin", "1", options);
			Response.Cookies.Append("sh_mid", manager.ManagerId.ToString(), options);
			Response.Cookies.Append("sh_mname", manager.ManagerName ?? manager.ManagerAccount, options);
			// Avoid identity confusion: clear user Cookie (can keep or delete, recommended to clear)
			Response.Cookies.Delete("sh_uid");
			Response.Cookies.Delete("sh_uname");

			if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
				return Redirect(model.ReturnUrl);

			return RedirectToAction(nameof(Index));
		}

		// ======== Logout (clear both identities) ========
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			// User
			Response.Cookies.Delete("sh_uid");
			Response.Cookies.Delete("sh_uname");
			// Admin
			Response.Cookies.Delete("sh_is_admin");
			Response.Cookies.Delete("sh_mid");
			Response.Cookies.Delete("sh_mname");

			return RedirectToAction(nameof(Login));
		}
	}
}
