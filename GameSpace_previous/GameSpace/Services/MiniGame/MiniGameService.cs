using GameSpace.Models;
using GameSpace.Services.Wallet;
using GameSpace.Services.Pet;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GameSpace.Services.MiniGame
{
    public class MiniGameService : IMiniGameService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly IWalletService _walletService;
        private readonly IPetService _petService;
        private readonly ILogger<MiniGameService> _logger;

        private const int MAX_GAMES_PER_DAY = 3;

        public MiniGameService(
            GameSpacedatabaseContext context,
            IWalletService walletService,
            IPetService petService,
            ILogger<MiniGameService> logger)
        {
            _context = context;
            _walletService = walletService;
            _petService = petService;
            _logger = logger;
        }

        public async Task<MiniGameResult> StartGameAsync(int userId, int petId, int level)
        {
            try
            {
                // Check if user can play
                if (!await CanPlayGameAsync(userId))
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Daily game limit reached" };
                }

                // Check if pet exists and belongs to user
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId && p.UserId == userId);
                if (pet == null)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Pet not found" };
                }

                // Check if pet is healthy enough to play
                if (pet.Health <= 0)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Pet is too sick to play" };
                }

                if (pet.Hunger >= 100 || pet.Mood <= 0 || pet.Stamina <= 0 || pet.Cleanliness <= 0)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Pet is not in good condition to play" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Create game record
                var game = new MiniGame
                {
                    UserId = userId,
                    PetId = petId,
                    Level = level,
                    MonsterCount = CalculateMonsterCount(level),
                    SpeedMultiplier = CalculateSpeedMultiplier(level),
                    Result = "Playing",
                    ExpGained = 0,
                    ExpGainedTime = DateTime.UtcNow,
                    PointsGained = 0,
                    PointsGainedTime = DateTime.UtcNow,
                    CouponGained = "0",
                    CouponGainedTime = DateTime.UtcNow,
                    HungerDelta = 0,
                    MoodDelta = 0,
                    StaminaDelta = 0,
                    CleanlinessDelta = 0,
                    StartTime = DateTime.UtcNow,
                    EndTime = null,
                    Aborted = false
                };

                _context.MiniGames.Add(game);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new MiniGameResult
                {
                    Success = true,
                    Game = game,
                    RemainingGames = await GetRemainingGamesAsync(userId),
                    Message = "Game started successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game for user {UserId}", userId);
                return new MiniGameResult { Success = false, ErrorMessage = "Failed to start game" };
            }
        }

        public async Task<MiniGameResult> EndGameAsync(int playId, string result, int monsterCount, decimal speedMultiplier)
        {
            try
            {
                var game = await _context.MiniGames
                    .Include(mg => mg.Pet)
                    .FirstOrDefaultAsync(mg => mg.PlayId == playId);

                if (game == null)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Game not found" };
                }

                if (game.EndTime.HasValue)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Game already ended" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Update game result
                game.Result = result;
                game.EndTime = DateTime.UtcNow;
                game.MonsterCount = monsterCount;
                game.SpeedMultiplier = speedMultiplier;

                // Calculate rewards based on result
                var rewards = CalculateRewards(game, result);

                game.PointsGained = rewards.PointsGained;
                game.ExpGained = rewards.ExpGained;
                game.CouponGained = rewards.CouponGained ?? "0";
                game.HungerDelta = rewards.HungerDelta;
                game.MoodDelta = rewards.MoodDelta;
                game.StaminaDelta = rewards.StaminaDelta;
                game.CleanlinessDelta = rewards.CleanlinessDelta;

                // Update pet attributes
                if (game.Pet != null)
                {
                    game.Pet.Hunger = Math.Max(0, Math.Min(100, game.Pet.Hunger + rewards.HungerDelta));
                    game.Pet.Mood = Math.Max(0, Math.Min(100, game.Pet.Mood + rewards.MoodDelta));
                    game.Pet.Stamina = Math.Max(0, Math.Min(100, game.Pet.Stamina + rewards.StaminaDelta));
                    game.Pet.Cleanliness = Math.Max(0, Math.Min(100, game.Pet.Cleanliness + rewards.CleanlinessDelta));
                    game.Pet.Experience += rewards.ExpGained;

                    // Check for pet level up
                    await CheckPetLevelUpAsync(game.Pet);
                }

                // Add points to wallet
                if (rewards.PointsGained > 0)
                {
                    await _walletService.AddPointsAsync(game.UserId, rewards.PointsGained, "Mini-game reward");
                }

                // Create coupon if applicable
                if (!string.IsNullOrEmpty(rewards.CouponGained) && rewards.CouponGained != "0")
                {
                    await CreateRewardCouponAsync(game.UserId, rewards.CouponGained);
                }

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new MiniGameResult
                {
                    Success = true,
                    Game = game,
                    PointsGained = rewards.PointsGained,
                    ExpGained = rewards.ExpGained,
                    CouponGained = rewards.CouponGained,
                    RemainingGames = await GetRemainingGamesAsync(game.UserId),
                    Message = GetResultMessage(result, rewards)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending game {PlayId}", playId);
                return new MiniGameResult { Success = false, ErrorMessage = "Failed to end game" };
            }
        }

        public async Task<MiniGameResult> AbortGameAsync(int playId)
        {
            try
            {
                var game = await _context.MiniGames.FindAsync(playId);
                if (game == null)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Game not found" };
                }

                if (game.EndTime.HasValue)
                {
                    return new MiniGameResult { Success = false, ErrorMessage = "Game already ended" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                game.Result = "Abort";
                game.EndTime = DateTime.UtcNow;
                game.Aborted = true;

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new MiniGameResult
                {
                    Success = true,
                    Game = game,
                    RemainingGames = await GetRemainingGamesAsync(game.UserId),
                    Message = "Game aborted"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aborting game {PlayId}", playId);
                return new MiniGameResult { Success = false, ErrorMessage = "Failed to abort game" };
            }
        }

        public async Task<List<MiniGame>> GetUserGamesAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.MiniGames
                    .Where(mg => mg.UserId == userId)
                    .OrderByDescending(mg => mg.StartTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting games for user {UserId}", userId);
                return new List<MiniGame>();
            }
        }

        public async Task<MiniGameResult> GetGameStatsAsync(int userId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var games = await _context.MiniGames
                    .Where(mg => mg.UserId == userId && mg.StartTime.Date == today)
                    .ToListAsync();

                var totalGames = games.Count;
                var wonGames = games.Count(g => g.Result == "Win");
                var totalPoints = games.Sum(g => g.PointsGained);
                var totalExp = games.Sum(g => g.ExpGained);

                return new MiniGameResult
                {
                    Success = true,
                    Message = $"Today: {totalGames} games, {wonGames} wins, {totalPoints} points, {totalExp} exp"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game stats for user {UserId}", userId);
                return new MiniGameResult { Success = false, ErrorMessage = "Failed to get game stats" };
            }
        }

        public async Task<bool> CanPlayGameAsync(int userId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var gamesToday = await _context.MiniGames
                    .CountAsync(mg => mg.UserId == userId && mg.StartTime.Date == today);

                return gamesToday < MAX_GAMES_PER_DAY;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} can play game", userId);
                return false;
            }
        }

        public async Task<int> GetRemainingGamesAsync(int userId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var gamesToday = await _context.MiniGames
                    .CountAsync(mg => mg.UserId == userId && mg.StartTime.Date == today);

                return Math.Max(0, MAX_GAMES_PER_DAY - gamesToday);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting remaining games for user {UserId}", userId);
                return 0;
            }
        }

        private int CalculateMonsterCount(int level)
        {
            return Math.Min(10, 3 + level);
        }

        private decimal CalculateSpeedMultiplier(int level)
        {
            return Math.Min(2.0m, 1.0m + (level * 0.1m));
        }

        private GameRewards CalculateRewards(MiniGame game, string result)
        {
            var rewards = new GameRewards();

            switch (result)
            {
                case "Win":
                    rewards.PointsGained = (int)(10 * game.Level * game.MonsterCount * game.SpeedMultiplier);
                    rewards.ExpGained = 5 * game.Level;
                    rewards.HungerDelta = -20;
                    rewards.MoodDelta = 15;
                    rewards.StaminaDelta = -30;
                    rewards.CleanlinessDelta = -10;
                    
                    // Special rewards for high levels
                    if (game.Level >= 5)
                    {
                        rewards.CouponGained = "ATTACK50";
                    }
                    break;

                case "Lose":
                    rewards.PointsGained = 5; // Participation reward
                    rewards.ExpGained = 2;
                    rewards.HungerDelta = -10;
                    rewards.MoodDelta = -10;
                    rewards.StaminaDelta = -20;
                    rewards.CleanlinessDelta = -5;
                    break;

                case "Abort":
                    rewards.PointsGained = 0;
                    rewards.ExpGained = 0;
                    rewards.HungerDelta = -5;
                    rewards.MoodDelta = -5;
                    rewards.StaminaDelta = -10;
                    rewards.CleanlinessDelta = 0;
                    break;
            }

            return rewards;
        }

        private async Task CheckPetLevelUpAsync(Pet pet)
        {
            var requiredExp = CalculateRequiredExp(pet.Level);
            if (pet.Experience >= requiredExp)
            {
                pet.Level++;
                pet.Experience -= requiredExp;
                pet.LevelUpTime = DateTime.UtcNow;
                pet.PointsGainedLevelUp = 50;
                pet.PointsGainedTimeLevelUp = DateTime.UtcNow;

                // Add level up points to wallet
                await _walletService.AddPointsAsync(pet.UserId, 50, "Pet level up bonus");
            }
        }

        private async Task CreateRewardCouponAsync(int userId, string couponCode)
        {
            try
            {
                // Find or create a game reward coupon type
                var couponType = await _context.CouponTypes
                    .FirstOrDefaultAsync(ct => ct.Name.Contains("Game Reward"));

                if (couponType == null)
                {
                    couponType = new CouponType
                    {
                        Name = "Game Reward Coupon",
                        DiscountType = "Amount",
                        DiscountValue = 50,
                        MinSpend = 100,
                        ValidFrom = DateTime.UtcNow,
                        ValidTo = DateTime.UtcNow.AddDays(7),
                        PointsCost = 0,
                        Description = "Reward for completing mini-game"
                    };

                    _context.CouponTypes.Add(couponType);
                    await _context.SaveChangesAsync();
                }

                // Create the coupon
                var coupon = new Coupon
                {
                    CouponCode = couponCode,
                    CouponTypeId = couponType.CouponTypeId,
                    UserId = userId,
                    IsUsed = false,
                    AcquiredTime = DateTime.UtcNow
                };

                _context.Coupons.Add(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reward coupon for user {UserId}", userId);
            }
        }

        private int CalculateRequiredExp(int level)
        {
            return 50 * level;
        }

        private string GetResultMessage(string result, GameRewards rewards)
        {
            return result switch
            {
                "Win" => $"Victory! Gained {rewards.PointsGained} points and {rewards.ExpGained} exp!",
                "Lose" => $"Defeat! But you still gained {rewards.PointsGained} points for trying!",
                "Abort" => "Game aborted. No rewards this time.",
                _ => "Game completed."
            };
        }

        private class GameRewards
        {
            public int PointsGained { get; set; }
            public int ExpGained { get; set; }
            public string? CouponGained { get; set; }
            public int HungerDelta { get; set; }
            public int MoodDelta { get; set; }
            public int StaminaDelta { get; set; }
            public int CleanlinessDelta { get; set; }
        }
    }
}