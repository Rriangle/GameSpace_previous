using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 論壇控制器
    /// </summary>
    public class ForumController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<ForumController> _logger;

        public ForumController(GameSpaceDbContext context, ILogger<ForumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示論壇首頁
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var forums = await _context.Forums
                .Include(f => f.Game)
                .Where(f => f.IsActive)
                .OrderByDescending(f => f.LastActivity)
                .Take(20)
                .ToListAsync();

            return View(forums);
        }

        /// <summary>
        /// 顯示特定論壇的討論串
        /// </summary>
        public async Task<IActionResult> Threads(int id)
        {
            var forum = await _context.Forums
                .Include(f => f.Game)
                .FirstOrDefaultAsync(f => f.ForumId == id);

            if (forum == null)
            {
                return NotFound();
            }

            var threads = await _context.Threads
                .Include(t => t.AuthorUser)
                .Where(t => t.ForumId == id && t.IsActive)
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.LastActivity)
                .Take(50)
                .ToListAsync();

            ViewBag.Forum = forum;
            return View(threads);
        }

        /// <summary>
        /// 顯示討論串詳情
        /// </summary>
        public async Task<IActionResult> Thread(int id)
        {
            var thread = await _context.Threads
                .Include(t => t.AuthorUser)
                .Include(t => t.Forum)
                .FirstOrDefaultAsync(t => t.ThreadId == id);

            if (thread == null)
            {
                return NotFound();
            }

            var posts = await _context.ThreadPosts
                .Include(p => p.AuthorUser)
                .Where(p => p.ThreadId == id && p.IsActive)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.Thread = thread;
            ViewBag.Posts = posts;
            return View();
        }

        /// <summary>
        /// 創建新討論串
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateThread(int forumId)
        {
            var forum = await _context.Forums
                .Include(f => f.Game)
                .FirstOrDefaultAsync(f => f.ForumId == forumId);

            if (forum == null)
            {
                return NotFound();
            }

            ViewBag.Forum = forum;
            return View();
        }

        /// <summary>
        /// 處理創建討論串請求
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThread(CreateThreadViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                var forum = await _context.Forums
                    .Include(f => f.Game)
                    .FirstOrDefaultAsync(f => f.ForumId == model.ForumId);
                ViewBag.Forum = forum;
                return View(model);
            }

            try
            {
                var thread = new Thread
                {
                    ForumId = model.ForumId,
                    AuthorUserId = userId.Value,
                    Title = model.Title,
                    Content = model.Content,
                    IsPinned = false,
                    IsActive = true,
                    ViewCount = 0,
                    ReplyCount = 0,
                    LastActivity = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Threads.Add(thread);
                await _context.SaveChangesAsync();

                // 更新論壇統計
                var forum = await _context.Forums.FindAsync(model.ForumId);
                if (forum != null)
                {
                    forum.ThreadCount++;
                    forum.LastActivity = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("用戶 {UserId} 在論壇 {ForumId} 創建了討論串 {ThreadId}", userId, model.ForumId, thread.ThreadId);

                return RedirectToAction("Thread", new { id = thread.ThreadId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建討論串時發生錯誤");
                ModelState.AddModelError("", "創建討論串時發生錯誤，請稍後再試");
                return View(model);
            }
        }

        /// <summary>
        /// 回覆討論串
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(ReplyViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "請填寫回覆內容" });
            }

            try
            {
                var post = new ThreadPost
                {
                    ThreadId = model.ThreadId,
                    AuthorUserId = userId.Value,
                    Content = model.Content,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ThreadPosts.Add(post);

                // 更新討論串統計
                var thread = await _context.Threads.FindAsync(model.ThreadId);
                if (thread != null)
                {
                    thread.ReplyCount++;
                    thread.LastActivity = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("用戶 {UserId} 回覆了討論串 {ThreadId}", userId, model.ThreadId);

                return Json(new { success = true, message = "回覆成功！" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "回覆討論串時發生錯誤");
                return Json(new { success = false, message = "回覆時發生錯誤，請稍後再試" });
            }
        }

        /// <summary>
        /// 獲取當前用戶ID
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// 創建討論串視圖模型
    /// </summary>
    public class CreateThreadViewModel
    {
        public int ForumId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }

    /// <summary>
    /// 回覆視圖模型
    /// </summary>
    public class ReplyViewModel
    {
        public int ThreadId { get; set; }
        public string Content { get; set; } = null!;
    }
}