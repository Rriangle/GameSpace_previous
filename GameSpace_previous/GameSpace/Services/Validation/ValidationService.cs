using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Services.Validation
{
    /// <summary>
    /// 驗證服務實現
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
                    _logger.LogWarning("用戶不存在: {UserId}", userId);
                    return false;
                }

                var userRights = await _context.UserRights.FindAsync(userId);
                if (userRights?.User_Status != true)
                {
                    _logger.LogWarning("用戶已停權: {UserId}", userId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證用戶失敗: {UserId}", userId);
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
                    _logger.LogWarning("寵物不存在: {PetId}", petId);
                    return false;
                }

                // 檢查寵物健康狀態
                if (pet.Health <= 0)
                {
                    _logger.LogWarning("寵物健康度不足: {PetId}, Health: {Health}", petId, pet.Health);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證寵物失敗: {PetId}", petId);
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
                    _logger.LogWarning("用戶錢包不存在: {UserId}", userId);
                    return false;
                }

                if (wallet.User_Point < points)
                {
                    _logger.LogWarning("用戶點數不足: {UserId}, 需要: {Points}, 現有: {Current}", 
                        userId, points, wallet.User_Point);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證點數失敗: {UserId}, {Points}", userId, points);
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
                    _logger.LogWarning("優惠券不存在: {CouponCode}", couponCode);
                    return false;
                }

                if (coupon.IsUsed)
                {
                    _logger.LogWarning("優惠券已使用: {CouponCode}", couponCode);
                    return false;
                }

                // 檢查有效期
                var couponType = await _context.CouponType
                    .FirstOrDefaultAsync(ct => ct.CouponTypeID == coupon.CouponTypeID);
                
                if (couponType != null && DateTime.UtcNow > couponType.ValidTo)
                {
                    _logger.LogWarning("優惠券已過期: {CouponCode}", couponCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證優惠券失敗: {CouponCode}", couponCode);
                return false;
            }
        }

        public async Task<bool> ValidateGameSessionAsync(int userId, int petId)
        {
            try
            {
                // 檢查用戶和寵物是否有效
                var userValid = await ValidateUserAsync(userId);
                var petValid = await ValidatePetAsync(petId);
                
                if (!userValid || !petValid)
                {
                    return false;
                }

                // 檢查寵物是否屬於該用戶
                var pet = await _context.Pet.FindAsync(petId);
                if (pet?.UserID != userId)
                {
                    _logger.LogWarning("寵物不屬於該用戶: {UserId}, {PetId}", userId, petId);
                    return false;
                }

                // 檢查每日遊戲次數限制
                var today = DateTime.UtcNow.Date;
                var todayGames = await _context.MiniGame
                    .CountAsync(mg => mg.UserID == userId && 
                              mg.StartTime.Date == today && 
                              mg.Result != "Abort");

                if (todayGames >= 3)
                {
                    _logger.LogWarning("用戶已達每日遊戲次數限制: {UserId}", userId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "驗證遊戲會話失敗: {UserId}, {PetId}", userId, petId);
                return false;
            }
        }
    }
}
