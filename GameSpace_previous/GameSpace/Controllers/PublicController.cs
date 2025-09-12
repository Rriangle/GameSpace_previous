using Microsoft.AspNetCore.Mvc;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    public class PublicController : Controller
    {
        private readonly GameSpacedatabaseContext _context;

        public PublicController(GameSpacedatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

        public async Task<IActionResult> Forum()
        {
            var forums = await _context.Forums
                .Include(f => f.Game)
                .ToListAsync();

            return View(forums);
        }

        public async Task<IActionResult> MiniGame()
        {
            // Get user's pet for mini games
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

            return View(userPet);
        }

        public async Task<IActionResult> Store()
        {
            var products = await _context.ProductInfos
                .Include(p => p.ProductImages)
                .Where(p => p.ProductType == "Game")
                .Take(12)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Social()
        {
            return RedirectToAction("Index", "Home", new { area = "social_hub" });
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