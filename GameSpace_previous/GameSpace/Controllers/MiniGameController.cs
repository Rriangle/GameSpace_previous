using Microsoft.AspNetCore.Mvc;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    public class MiniGameController : Controller
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<MiniGameController> _logger;

        public MiniGameController(GameSpacedatabaseContext context, ILogger<MiniGameController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: MiniGame
        public async Task<IActionResult> Index()
        {
            var miniGames = await _context.MiniGames
                .Include(m => m.User)
                .OrderByDescending(m => m.StartTime)
                .Take(50)
                .ToListAsync();
            return View(miniGames);
        }

        // GET: MiniGame/Play
        public async Task<IActionResult> Play()
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

            if (userPet == null)
            {
                TempData["Error"] = "You need a pet to play mini games!";
                return RedirectToAction("Index", "Pet");
            }

            return View(userPet);
        }

        // POST: MiniGame/Start
        [HttpPost]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
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

            var pet = await _context.Pets.FindAsync(request.PetId);
            if (pet == null || pet.UserId != userId.Value)
            {
                return Json(new { success = false, message = "Pet not found or not owned by user" });
            }

            // Create new mini game session
            var miniGame = new MiniGame
            {
                UserId = userId.Value,
                PetId = request.PetId,
                Level = request.Level,
                MonsterCount = request.MonsterCount,
                SpeedMultiplier = request.SpeedMultiplier,
                StartTime = DateTime.UtcNow,
                Result = "playing",
                ExpGained = 0,
                ExpGainedTime = DateTime.UtcNow,
                PointsGained = 0,
                PointsGainedTime = DateTime.UtcNow,
                CouponGained = "",
                CouponGainedTime = DateTime.UtcNow,
                HungerDelta = 0,
                MoodDelta = 0,
                StaminaDelta = 0,
                CleanlinessDelta = 0,
                Aborted = false
            };

            _context.MiniGames.Add(miniGame);
            await _context.SaveChangesAsync();

            return Json(new { success = true, gameId = miniGame.PlayId });
        }

        // POST: MiniGame/End
        [HttpPost]
        public async Task<IActionResult> EndGame([FromBody] EndGameRequest request)
        {
            var miniGame = await _context.MiniGames.FindAsync(request.GameId);
            if (miniGame == null)
            {
                return Json(new { success = false, message = "Game not found" });
            }

            // Update game results
            miniGame.EndTime = DateTime.UtcNow;
            miniGame.Result = request.Result;
            miniGame.ExpGained = request.ExpGained;
            miniGame.ExpGainedTime = DateTime.UtcNow;
            miniGame.PointsGained = request.PointsGained;
            miniGame.PointsGainedTime = DateTime.UtcNow;
            miniGame.CouponGained = request.CouponGained ?? "";
            miniGame.CouponGainedTime = DateTime.UtcNow;
            miniGame.HungerDelta = request.HungerDelta;
            miniGame.MoodDelta = request.MoodDelta;
            miniGame.StaminaDelta = request.StaminaDelta;
            miniGame.CleanlinessDelta = request.CleanlinessDelta;
            miniGame.Aborted = request.Aborted;

            // Update pet stats
            var pet = await _context.Pets.FindAsync(miniGame.PetId);
            if (pet != null)
            {
                pet.Experience += request.ExpGained;
                pet.Hunger = Math.Max(0, Math.Min(100, pet.Hunger + request.HungerDelta));
                pet.Mood = Math.Max(0, Math.Min(100, pet.Mood + request.MoodDelta));
                pet.Stamina = Math.Max(0, Math.Min(100, pet.Stamina + request.StaminaDelta));
                pet.Cleanliness = Math.Max(0, Math.Min(100, pet.Cleanliness + request.CleanlinessDelta));

                // Check for level up
                if (pet.Experience >= pet.Level * 100)
                {
                    pet.Level++;
                    pet.Experience = 0;
                    pet.LevelUpTime = DateTime.UtcNow;
                    pet.PointsGainedLevelUp += 100;
                    pet.PointsGainedTimeLevelUp = DateTime.UtcNow;
                }

                _context.Update(pet);
            }

            _context.Update(miniGame);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                expGained = request.ExpGained,
                pointsGained = request.PointsGained,
                leveledUp = pet?.Experience == 0 && pet?.Level > 1
            });
        }

        // GET: MiniGame/Leaderboard
        public async Task<IActionResult> Leaderboard()
        {
            var leaderboard = await _context.MiniGames
                .Include(m => m.User)
                .Where(m => m.Result == "completed" && m.EndTime.HasValue)
                .GroupBy(m => m.UserId)
                .Select(g => new LeaderboardEntry
                {
                    UserId = g.Key,
                    UserName = g.First().User.UserName,
                    TotalGames = g.Count(),
                    TotalExp = g.Sum(m => m.ExpGained),
                    TotalPoints = g.Sum(m => m.PointsGained),
                    BestScore = g.Max(m => m.PointsGained),
                    LastPlayed = g.Max(m => m.EndTime.Value)
                })
                .OrderByDescending(l => l.TotalPoints)
                .Take(20)
                .ToListAsync();

            return View(leaderboard);
        }

        // GET: MiniGame/Stats
        public async Task<IActionResult> Stats()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var stats = await _context.MiniGames
                .Where(m => m.UserId == userId.Value)
                .GroupBy(m => m.Result)
                .Select(g => new GameStats
                {
                    Result = g.Key,
                    Count = g.Count(),
                    TotalExp = g.Sum(m => m.ExpGained),
                    TotalPoints = g.Sum(m => m.PointsGained),
                    AverageScore = g.Average(m => m.PointsGained)
                })
                .ToListAsync();

            var totalGames = await _context.MiniGames
                .Where(m => m.UserId == userId.Value)
                .CountAsync();

            var totalExp = await _context.MiniGames
                .Where(m => m.UserId == userId.Value)
                .SumAsync(m => m.ExpGained);

            var totalPoints = await _context.MiniGames
                .Where(m => m.UserId == userId.Value)
                .SumAsync(m => m.PointsGained);

            var viewModel = new MiniGameStatsViewModel
            {
                TotalGames = totalGames,
                TotalExp = totalExp,
                TotalPoints = totalPoints,
                StatsByResult = stats
            };

            return View(viewModel);
        }

        private int? GetCurrentUserId()
        {
            // This is a placeholder - implement actual user ID retrieval
            // based on your authentication system
            return null;
        }
    }

    public class StartGameRequest
    {
        public int PetId { get; set; }
        public int Level { get; set; }
        public int MonsterCount { get; set; }
        public decimal SpeedMultiplier { get; set; }
    }

    public class EndGameRequest
    {
        public int GameId { get; set; }
        public string Result { get; set; } = "";
        public int ExpGained { get; set; }
        public int PointsGained { get; set; }
        public string? CouponGained { get; set; }
        public int HungerDelta { get; set; }
        public int MoodDelta { get; set; }
        public int StaminaDelta { get; set; }
        public int CleanlinessDelta { get; set; }
        public bool Aborted { get; set; }
    }

    public class LeaderboardEntry
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int TotalGames { get; set; }
        public int TotalExp { get; set; }
        public int TotalPoints { get; set; }
        public int BestScore { get; set; }
        public DateTime LastPlayed { get; set; }
    }

    public class GameStats
    {
        public string Result { get; set; } = "";
        public int Count { get; set; }
        public int TotalExp { get; set; }
        public int TotalPoints { get; set; }
        public double AverageScore { get; set; }
    }

    public class MiniGameStatsViewModel
    {
        public int TotalGames { get; set; }
        public int TotalExp { get; set; }
        public int TotalPoints { get; set; }
        public List<GameStats> StatsByResult { get; set; } = new();
    }
}