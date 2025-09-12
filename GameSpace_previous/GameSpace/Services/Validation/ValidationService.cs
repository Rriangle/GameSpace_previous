using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Services.Validation
{
    /// <summary>
    /// Validation service implementation
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(GameSpaceDbContext context, ILogger<ValidationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ValidateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User does not exist: {UserId}", userId);
                    return false;
                }

                var userRights = await _context.UserRights.FindAsync(userId);
                if (userRights?.User_Status != true)
                {
                    _logger.LogWarning("User is suspended: {UserId}", userId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User validation failed: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ValidatePetAsync(int petId)
        {
            try
            {
                var pet = await _context.Pet.FindAsync(petId);
                if (pet == null)
                {
                    _logger.LogWarning("Pet does not exist: {PetId}", petId);
                    return false;
                }

                // Check pet health status
                if (pet.Health <= 0)
                {
                    _logger.LogWarning("Pet health insufficient: {PetId}, Health: {Health}", petId, pet.Health);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pet validation failed: {PetId}", petId);
                return false;
            }
        }

        public async Task<bool> ValidatePointsAsync(int userId, int points)
        {
            try
            {
                var wallet = await _context.UserWallet.FindAsync(userId);
                if (wallet == null)
                {
                    _logger.LogWarning("User wallet does not exist: {UserId}", userId);
                    return false;
                }

                if (wallet.User_Point < points)
                {
                    _logger.LogWarning("Insufficient user points: {UserId}, Required: {Points}, Current: {Current}", 
                        userId, points, wallet.User_Point);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Points validation failed: {UserId}, {Points}", userId, points);
                return false;
            }
        }

        public async Task<bool> ValidateCouponAsync(string couponCode)
        {
            try
            {
                var coupon = await _context.Coupon
                    .FirstOrDefaultAsync(c => c.CouponCode == couponCode);
                
                if (coupon == null)
                {
                    _logger.LogWarning("Coupon does not exist: {CouponCode}", couponCode);
                    return false;
                }

                if (coupon.IsUsed)
                {
                    _logger.LogWarning("Coupon already used: {CouponCode}", couponCode);
                    return false;
                }

                // Check validity period
                var couponType = await _context.CouponType
                    .FirstOrDefaultAsync(ct => ct.CouponTypeID == coupon.CouponTypeID);
                
                if (couponType != null && DateTime.UtcNow > couponType.ValidTo)
                {
                    _logger.LogWarning("Coupon expired: {CouponCode}", couponCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Coupon validation failed: {CouponCode}", couponCode);
                return false;
            }
        }

        public async Task<bool> ValidateGameSessionAsync(int userId, int petId)
        {
            try
            {
                // Check if user and pet are valid
                var userValid = await ValidateUserAsync(userId);
                var petValid = await ValidatePetAsync(petId);
                
                if (!userValid || !petValid)
                {
                    return false;
                }

                // Check if pet belongs to the user
                var pet = await _context.Pet.FindAsync(petId);
                if (pet?.UserID != userId)
                {
                    _logger.LogWarning("Pet does not belong to user: {UserId}, {PetId}", userId, petId);
                    return false;
                }

                // Check daily game count limit
                var today = DateTime.UtcNow.Date;
                var todayGames = await _context.MiniGame
                    .CountAsync(mg => mg.UserID == userId && 
                              mg.StartTime.Date == today && 
                              mg.Result != "Abort");

                if (todayGames >= 3)
                {
                    _logger.LogWarning("User has reached daily game limit: {UserId}", userId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Game session validation failed: {UserId}, {PetId}", userId, petId);
                return false;
            }
        }
    }
}
