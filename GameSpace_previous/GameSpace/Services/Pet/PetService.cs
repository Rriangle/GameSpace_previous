using GameSpace.Models;
using GameSpace.Services.Wallet;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GameSpace.Services.Pet
{
    public class PetService : IPetService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly IWalletService _walletService;
        private readonly ILogger<PetService> _logger;

        public PetService(
            GameSpacedatabaseContext context,
            IWalletService walletService,
            ILogger<PetService> logger)
        {
            _context = context;
            _walletService = walletService;
            _logger = logger;
        }

        public async Task<PetResult> GetPetAsync(int userId)
        {
            try
            {
                var pet = await _context.Pets
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                return new PetResult { Success = true, Pet = pet };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to get pet" };
            }
        }

        public async Task<PetResult> CreatePetAsync(int userId, string petName)
        {
            try
            {
                // Check if user already has a pet
                var existingPet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (existingPet != null)
                {
                    return new PetResult { Success = false, ErrorMessage = "User already has a pet" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var pet = new Pet
                {
                    UserId = userId,
                    PetName = petName,
                    Level = 1,
                    LevelUpTime = DateTime.UtcNow,
                    Experience = 0,
                    Hunger = 50,
                    Mood = 50,
                    Stamina = 50,
                    Cleanliness = 50,
                    Health = 100,
                    SkinColor = "#79b7ff",
                    SkinColorChangedTime = DateTime.UtcNow,
                    BackgroundColor = "#91e27c",
                    BackgroundColorChangedTime = DateTime.UtcNow,
                    PointsChangedSkinColor = 0,
                    PointsChangedBackgroundColor = 0,
                    PointsGainedLevelUp = 0,
                    PointsGainedTimeLevelUp = DateTime.UtcNow
                };

                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();

                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet created successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pet for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to create pet" };
            }
        }

        public async Task<PetResult> FeedPetAsync(int userId)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                if (pet.Health <= 0)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet is too sick to eat" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Feed pet
                pet.Hunger = Math.Max(0, pet.Hunger - 30);
                pet.Mood = Math.Min(100, pet.Mood + 10);
                pet.Health = Math.Min(100, pet.Health + 5);

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet has been fed!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error feeding pet for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to feed pet" };
            }
        }

        public async Task<PetResult> PlayWithPetAsync(int userId)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                if (pet.Health <= 0)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet is too sick to play" };
                }

                if (pet.Stamina < 20)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet is too tired to play" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Play with pet
                pet.Mood = Math.Min(100, pet.Mood + 20);
                pet.Stamina = Math.Max(0, pet.Stamina - 15);
                pet.Experience += 5;

                // Check for level up
                await CheckLevelUpAsync(pet);

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet had fun playing!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error playing with pet for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to play with pet" };
            }
        }

        public async Task<PetResult> BathePetAsync(int userId)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                if (pet.Health <= 0)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet is too sick to bathe" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Bathe pet
                pet.Cleanliness = Math.Min(100, pet.Cleanliness + 40);
                pet.Mood = Math.Min(100, pet.Mood + 5);
                pet.Health = Math.Min(100, pet.Health + 10);

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet is now clean and fresh!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bathing pet for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to bathe pet" };
            }
        }

        public async Task<PetResult> RestPetAsync(int userId)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Rest pet
                pet.Stamina = Math.Min(100, pet.Stamina + 30);
                pet.Health = Math.Min(100, pet.Health + 5);
                pet.Mood = Math.Min(100, pet.Mood + 5);

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet is well rested!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resting pet for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to rest pet" };
            }
        }

        public async Task<PetResult> ChangeSkinColorAsync(int userId, string skinColor)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                const int skinColorCost = 2000;

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Check if user has enough points
                var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
                if (wallet == null || wallet.UserPoint < skinColorCost)
                {
                    return new PetResult { Success = false, ErrorMessage = "Insufficient points" };
                }

                // Deduct points
                await _walletService.DeductPointsAsync(userId, skinColorCost, "Change pet skin color");

                // Change skin color
                pet.SkinColor = skinColor;
                pet.SkinColorChangedTime = DateTime.UtcNow;
                pet.PointsChangedSkinColor = skinColorCost;

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet skin color changed!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing skin color for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to change skin color" };
            }
        }

        public async Task<PetResult> ChangeBackgroundColorAsync(int userId, string backgroundColor)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Change background color (free)
                pet.BackgroundColor = backgroundColor;
                pet.BackgroundColorChangedTime = DateTime.UtcNow;
                pet.PointsChangedBackgroundColor = 0;

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet background color changed!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing background color for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to change background color" };
            }
        }

        public async Task<PetResult> UpdatePetAttributesAsync(int userId)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet == null)
                {
                    return new PetResult { Success = false, ErrorMessage = "Pet not found" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Update attributes based on time passed
                var timeSinceLastUpdate = DateTime.UtcNow - pet.LevelUpTime;
                var hoursPassed = (int)timeSinceLastUpdate.TotalHours;

                if (hoursPassed > 0)
                {
                    // Hunger increases over time
                    pet.Hunger = Math.Min(100, pet.Hunger + (hoursPassed * 2));
                    
                    // Mood decreases if hungry
                    if (pet.Hunger > 80)
                    {
                        pet.Mood = Math.Max(0, pet.Mood - (hoursPassed * 1));
                    }

                    // Cleanliness decreases over time
                    pet.Cleanliness = Math.Max(0, pet.Cleanliness - (hoursPassed * 1));

                    // Health decreases if very hungry or dirty
                    if (pet.Hunger > 90 || pet.Cleanliness < 20)
                    {
                        pet.Health = Math.Max(0, pet.Health - (hoursPassed * 1));
                    }

                    // Stamina decreases if not rested
                    if (pet.Stamina > 0)
                    {
                        pet.Stamina = Math.Max(0, pet.Stamina - (hoursPassed * 1));
                    }
                }

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new PetResult { Success = true, Pet = pet, Message = "Pet attributes updated" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pet attributes for user {UserId}", userId);
                return new PetResult { Success = false, ErrorMessage = "Failed to update pet attributes" };
            }
        }

        public async Task<List<Pet>> GetUserPetsAsync(int userId)
        {
            try
            {
                return await _context.Pets
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pets for user {UserId}", userId);
                return new List<Pet>();
            }
        }

        private async Task CheckLevelUpAsync(Pet pet)
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

        private int CalculateRequiredExp(int level)
        {
            // Simple level up formula: 50 * level
            return 50 * level;
        }
    }
}