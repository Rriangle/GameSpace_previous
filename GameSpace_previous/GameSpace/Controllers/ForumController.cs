using Microsoft.AspNetCore.Mvc;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    public class ForumController : Controller
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<ForumController> _logger;

        public ForumController(GameSpacedatabaseContext context, ILogger<ForumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Forum
        public async Task<IActionResult> Index()
        {
            var forums = await _context.Forums
                .Include(f => f.Game)
                .Include(f => f.Threads)
                .ToListAsync();

            return View(forums);
        }

        // GET: Forum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .Include(f => f.Game)
                .Include(f => f.Threads)
                    .ThenInclude(t => t.AuthorUser)
                .FirstOrDefaultAsync(m => m.ForumId == id);

            if (forum == null)
            {
                return NotFound();
            }

            return View(forum);
        }

        // GET: Forum/Thread/5
        public async Task<IActionResult> Thread(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Threads
                .Include(t => t.Forum)
                .Include(t => t.AuthorUser)
                .Include(t => t.ThreadPosts)
                    .ThenInclude(p => p.AuthorUser)
                .FirstOrDefaultAsync(m => m.ThreadId == id);

            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }

        // GET: Forum/CreateThread
        public IActionResult CreateThread(int forumId)
        {
            var forum = _context.Forums.Find(forumId);
            if (forum == null)
            {
                return NotFound();
            }

            ViewData["ForumId"] = forumId;
            ViewData["ForumName"] = forum.Name;
            return View();
        }

        // POST: Forum/CreateThread
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThread([Bind("ForumId,Title,Content")] CreateThreadRequest request)
        {
            if (ModelState.IsValid)
            {
                var thread = new Thread
                {
                    ForumId = request.ForumId,
                    Title = request.Title,
                    AuthorUserId = GetCurrentUserId() ?? 1, // Placeholder
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = "active"
                };

                _context.Threads.Add(thread);
                await _context.SaveChangesAsync();

                // Create initial post
                var post = new ThreadPost
                {
                    ThreadId = thread.ThreadId,
                    ContentMd = request.Content,
                    AuthorUserId = GetCurrentUserId() ?? 1, // Placeholder
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = "published"
                };

                _context.ThreadPosts.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction("Thread", new { id = thread.ThreadId });
            }

            ViewData["ForumId"] = request.ForumId;
            return View(request);
        }

        // POST: Forum/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply([Bind("ThreadId,Content,ParentPostId")] ReplyRequest request)
        {
            if (ModelState.IsValid)
            {
                var post = new ThreadPost
                {
                    ThreadId = request.ThreadId,
                    ContentMd = request.Content,
                    ParentPostId = request.ParentPostId,
                    AuthorUserId = GetCurrentUserId() ?? 1, // Placeholder
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = "published"
                };

                _context.ThreadPosts.Add(post);

                // Update thread's updated time
                var thread = await _context.Threads.FindAsync(request.ThreadId);
                if (thread != null)
                {
                    thread.UpdatedAt = DateTime.UtcNow;
                    _context.Update(thread);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Thread", new { id = request.ThreadId });
            }

            return RedirectToAction("Thread", new { id = request.ThreadId });
        }

        // GET: Forum/Search
        public async Task<IActionResult> Search(string? query, int? forumId)
        {
            var threads = _context.Threads
                .Include(t => t.Forum)
                .Include(t => t.AuthorUser)
                .AsQueryable();

            if (forumId.HasValue)
            {
                threads = threads.Where(t => t.ForumId == forumId.Value);
            }

            if (!string.IsNullOrEmpty(query))
            {
                threads = threads.Where(t => t.Title.Contains(query) || 
                    t.ThreadPosts.Any(p => p.ContentMd.Contains(query)));
            }

            var results = await threads
                .OrderByDescending(t => t.UpdatedAt)
                .Take(50)
                .ToListAsync();

            ViewData["Query"] = query;
            ViewData["ForumId"] = forumId;
            ViewData["Forums"] = await _context.Forums.ToListAsync();

            return View(results);
        }

        // GET: Forum/Hot
        public async Task<IActionResult> Hot()
        {
            var hotThreads = await _context.Threads
                .Include(t => t.Forum)
                .Include(t => t.AuthorUser)
                .Include(t => t.ThreadPosts)
                .Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(t => t.ThreadPosts.Count)
                .Take(20)
                .ToListAsync();

            return View(hotThreads);
        }

        // GET: Forum/Latest
        public async Task<IActionResult> Latest()
        {
            var latestThreads = await _context.Threads
                .Include(t => t.Forum)
                .Include(t => t.AuthorUser)
                .OrderByDescending(t => t.CreatedAt)
                .Take(20)
                .ToListAsync();

            return View(latestThreads);
        }

        // POST: Forum/React
        [HttpPost]
        public async Task<IActionResult> React(int targetId, string targetType, string kind)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "User ID not found" });
            }

            // Check if reaction already exists
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.UserId == userId.Value && 
                    r.TargetId == targetId && 
                    r.TargetType == targetType && 
                    r.Kind == kind);

            if (existingReaction != null)
            {
                // Remove existing reaction
                _context.Reactions.Remove(existingReaction);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "removed" });
            }
            else
            {
                // Add new reaction
                var reaction = new Reaction
                {
                    UserId = userId.Value,
                    TargetId = targetId,
                    TargetType = targetType,
                    Kind = kind,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reactions.Add(reaction);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "added" });
            }
        }

        // POST: Forum/Bookmark
        [HttpPost]
        public async Task<IActionResult> Bookmark(int targetId, string targetType)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "User ID not found" });
            }

            // Check if bookmark already exists
            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId.Value && 
                    b.TargetId == targetId && 
                    b.TargetType == targetType);

            if (existingBookmark != null)
            {
                // Remove existing bookmark
                _context.Bookmarks.Remove(existingBookmark);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "removed" });
            }
            else
            {
                // Add new bookmark
                var bookmark = new Bookmark
                {
                    UserId = userId.Value,
                    TargetId = targetId,
                    TargetType = targetType,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookmarks.Add(bookmark);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "added" });
            }
        }

        private int? GetCurrentUserId()
        {
            // This is a placeholder - implement actual user ID retrieval
            // based on your authentication system
            return null;
        }
    }

    public class CreateThreadRequest
    {
        public int ForumId { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
    }

    public class ReplyRequest
    {
        public int ThreadId { get; set; }
        public string Content { get; set; } = "";
        public int? ParentPostId { get; set; }
    }
}