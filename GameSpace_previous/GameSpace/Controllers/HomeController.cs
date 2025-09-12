using System.Diagnostics;
using GameSpace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GameSpacedatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, GameSpacedatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Check if user is admin - show admin dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard");
            }

            // Show public frontend for regular users
            return await ShowPublicIndex();
        }

        private async Task<IActionResult> ShowPublicIndex()
        {
            // Get hot threads data
            var hotThreads = await _context.Threads
                .Include(t => t.AuthorUser)
                .Include(t => t.Forum)
                .OrderByDescending(t => t.UpdatedAt)
                .Take(5)
                .ToListAsync();

            // Get popular games
            var popularGames = await _context.Games
                .Include(g => g.Forum)
                .OrderByDescending(g => g.CreatedAt)
                .Take(6)
                .ToListAsync();

            // Get recent posts
            var recentPosts = await _context.Posts
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.Game)
                .Where(p => p.Status == "published")
                .OrderByDescending(p => p.PublishedAt)
                .Take(10)
                .ToListAsync();

            // Get user pet data (if user is logged in)
            Pet? userPet = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    userPet = await _context.Pets
                        .FirstOrDefaultAsync(p => p.UserId == userId.Value);
                }
            }

            var viewModel = new PublicIndexViewModel
            {
                HotThreads = hotThreads,
                PopularGames = popularGames,
                RecentPosts = recentPosts,
                UserPet = userPet
            };

            return View("PublicIndex", viewModel);
        }

        public IActionResult Dashboard()
        {
            // Admin dashboard - use admin layout
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int? GetCurrentUserId()
        {
            // This is a placeholder - implement actual user ID retrieval
            // based on your authentication system
            return null;
        }
    }

    public class PublicIndexViewModel
    {
        public List<Thread> HotThreads { get; set; } = new();
        public List<Game> PopularGames { get; set; } = new();
        public List<Post> RecentPosts { get; set; } = new();
        public Pet? UserPet { get; set; }
    }
}