using GameSpace.Models;
using GameSpace.Services.Wallet;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GameSpace.Services.SignIn
{
    public class SignInService : ISignInService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly IWalletService _walletService;
        private readonly ILogger<SignInService> _logger;

        public SignInService(
            GameSpacedatabaseContext context,
            IWalletService walletService,
            ILogger<SignInService> logger)
        {
            _context = context;
            _walletService = walletService;
            _logger = logger;
        }

        public async Task<SignInResult> SignInAsync(int userId)
        {
            try
            {
                // Check if already signed in today
                if (await HasSignedInTodayAsync(userId))
                {
                    return new SignInResult { Success = false, ErrorMessage = "Already signed in today" };
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var consecutiveDays = await GetConsecutiveDaysAsync(userId);
                var rewards = await CalculateRewardsAsync(consecutiveDays + 1);

                // Create sign-in record
                var signInRecord = new UserSignInStat
                {
                    UserId = userId,
                    SignTime = DateTime.UtcNow,
                    PointsGained = rewards.PointsGained,
                    PointsGainedTime = DateTime.UtcNow,
                    ExpGained = rewards.ExpGained,
                    ExpGainedTime = DateTime.UtcNow,
                    CouponGained = rewards.CouponGained ?? "0",
                    CouponGainedTime = DateTime.UtcNow
                };

                _context.UserSignInStats.Add(signInRecord);

                // Add points to wallet
                if (rewards.PointsGained > 0)
                {
                    await _walletService.AddPointsAsync(userId, rewards.PointsGained, "Daily sign-in reward");
                }

                // Add experience to pet
                if (rewards.ExpGained > 0)
                {
                    await AddPetExperienceAsync(userId, rewards.ExpGained);
                }

                // Create coupon if applicable
                if (!string.IsNullOrEmpty(rewards.CouponGained) && rewards.CouponGained != "0")
                {
                    await CreateRewardCouponAsync(userId, rewards.CouponGained);
                }

                await _context.SaveChangesAsync();
                transaction.Complete();

                return new SignInResult
                {
                    Success = true,
                    PointsGained = rewards.PointsGained,
                    ExpGained = rewards.ExpGained,
                    CouponGained = rewards.CouponGained,
                    ConsecutiveDays = consecutiveDays + 1,
                    SignInRecord = signInRecord
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing in user {UserId}", userId);
                return new SignInResult { Success = false, ErrorMessage = "Sign-in failed" };
            }
        }

        public async Task<bool> HasSignedInTodayAsync(int userId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                return await _context.UserSignInStats
                    .AnyAsync(s => s.UserId == userId && s.SignTime.Date == today);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking sign-in status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<List<UserSignInStat>> GetSignInHistoryAsync(int userId, int days = 30)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-days).Date;
                return await _context.UserSignInStats
                    .Where(s => s.UserId == userId && s.SignTime.Date >= startDate)
                    .OrderByDescending(s => s.SignTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sign-in history for user {UserId}", userId);
                return new List<UserSignInStat>();
            }
        }

        public async Task<int> GetConsecutiveDaysAsync(int userId)
        {
            try
            {
                var signIns = await _context.UserSignInStats
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.SignTime)
                    .ToListAsync();

                if (!signIns.Any())
                {
                    return 0;
                }

                var consecutiveDays = 0;
                var currentDate = DateTime.UtcNow.Date;

                foreach (var signIn in signIns)
                {
                    if (signIn.SignTime.Date == currentDate)
                    {
                        consecutiveDays++;
                        currentDate = currentDate.AddDays(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return consecutiveDays;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating consecutive days for user {UserId}", userId);
                return 0;
            }
        }

        public async Task<SignInResult> GetSignInRewardsAsync(int userId)
        {
            try
            {
                var consecutiveDays = await GetConsecutiveDaysAsync(userId);
                var rewards = await CalculateRewardsAsync(consecutiveDays);

                return new SignInResult
                {
                    Success = true,
                    PointsGained = rewards.PointsGained,
                    ExpGained = rewards.ExpGained,
                    CouponGained = rewards.CouponGained,
                    ConsecutiveDays = consecutiveDays
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sign-in rewards for user {UserId}", userId);
                return new SignInResult { Success = false, ErrorMessage = "Failed to get rewards" };
            }
        }

        private async Task<SignInRewards> CalculateRewardsAsync(int consecutiveDays)
        {
            var rewards = new SignInRewards
            {
                PointsGained = 20, // Base points
                ExpGained = 10,    // Base experience
                CouponGained = null
            };

            // Consecutive day bonuses
            if (consecutiveDays >= 7)
            {
                rewards.PointsGained += 100;
                rewards.CouponGained = "DISCOUNT15"; // 15% discount coupon
            }
            else if (consecutiveDays >= 3)
            {
                rewards.PointsGained += 30;
            }

            // Special day bonuses
            var today = DateTime.UtcNow.DayOfWeek;
            if (today == DayOfWeek.Sunday)
            {
                rewards.PointsGained += 50; // Sunday bonus
            }

            return rewards;
        }

        private async Task AddPetExperienceAsync(int userId, int expGained)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
                if (pet != null)
                {
                    pet.Experience += expGained;
                    
                    // Check for level up
                    var requiredExp = CalculateRequiredExp(pet.Level);
                    if (pet.Experience >= requiredExp)
                    {
                        pet.Level++;
                        pet.Experience -= requiredExp;
                        pet.LevelUpTime = DateTime.UtcNow;
                        pet.PointsGainedLevelUp = 50; // Level up bonus
                        pet.PointsGainedTimeLevelUp = DateTime.UtcNow;

                        // Add level up points to wallet
                        await _walletService.AddPointsAsync(userId, 50, "Pet level up bonus");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pet experience for user {UserId}", userId);
            }
        }

        private async Task CreateRewardCouponAsync(int userId, string couponCode)
        {
            try
            {
                // Find or create a discount coupon type
                var couponType = await _context.CouponTypes
                    .FirstOrDefaultAsync(ct => ct.Name.Contains("Sign-in Reward"));

                if (couponType == null)
                {
                    couponType = new CouponType
                    {
                        Name = "Sign-in Reward Coupon",
                        DiscountType = "Percent",
                        DiscountValue = 0.15m, // 15% discount
                        MinSpend = 0,
                        ValidFrom = DateTime.UtcNow,
                        ValidTo = DateTime.UtcNow.AddDays(30),
                        PointsCost = 0,
                        Description = "Reward for consecutive sign-in"
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
            // Simple level up formula: 50 * level
            return 50 * level;
        }

        private class SignInRewards
        {
            public int PointsGained { get; set; }
            public int ExpGained { get; set; }
            public string? CouponGained { get; set; }
        }
    }
}