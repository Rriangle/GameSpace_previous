using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 遊戲相關 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IUserReadOnlyRepository _userRepository;

        public GameController(IUserReadOnlyRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 取得用戶寵物資訊
        /// </summary>
        [HttpGet("users/{userId}/pet")]
        public async Task<ActionResult<PetReadModel>> GetUserPet(int userId)
        {
            try
            {
                var pet = await _userRepository.GetPetByUserIdAsync(userId);
                if (pet == null)
                {
                    return NotFound($"用戶 {userId} 沒有寵物");
                }
                return Ok(pet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得寵物資訊失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶小遊戲記錄
        /// </summary>
        [HttpGet("users/{userId}/minigames")]
        public async Task<ActionResult<List<MiniGameReadModel>>> GetUserMiniGames(
            int userId,
            [FromQuery] int limit = 10)
        {
            try
            {
                var miniGames = await _userRepository.GetUserMiniGamesAsync(userId, limit);
                return Ok(miniGames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得小遊戲記錄失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶簽到記錄
        /// </summary>
        [HttpGet("users/{userId}/signin-stats")]
        public async Task<ActionResult<List<UserSignInStatsReadModel>>> GetUserSignInStats(
            int userId,
            [FromQuery] int days = 30)
        {
            try
            {
                var signInStats = await _userRepository.GetUserSignInStatsAsync(userId, days);
                return Ok(signInStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得簽到記錄失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶錢包歷史
        /// </summary>
        [HttpGet("users/{userId}/wallet-history")]
        public async Task<ActionResult<List<WalletHistoryReadModel>>> GetUserWalletHistory(
            int userId,
            [FromQuery] int limit = 50)
        {
            try
            {
                var walletHistory = await _userRepository.GetUserWalletHistoryAsync(userId, limit);
                return Ok(walletHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得錢包歷史失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶優惠券
        /// </summary>
        [HttpGet("users/{userId}/coupons")]
        public async Task<ActionResult<List<CouponReadModel>>> GetUserCoupons(
            int userId,
            [FromQuery] bool? isUsed = null)
        {
            try
            {
                var coupons = await _userRepository.GetUserCouponsAsync(userId, isUsed);
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得優惠券失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶禮券
        /// </summary>
        [HttpGet("users/{userId}/evouchers")]
        public async Task<ActionResult<List<EVoucherReadModel>>> GetUserEVouchers(
            int userId,
            [FromQuery] bool? isUsed = null)
        {
            try
            {
                var evouchers = await _userRepository.GetUserEVouchersAsync(userId, isUsed);
                return Ok(evouchers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得禮券失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶錢包餘額
        /// </summary>
        [HttpGet("users/{userId}/wallet")]
        public async Task<ActionResult<UserWalletReadModel>> GetUserWallet(int userId)
        {
            try
            {
                var wallet = await _userRepository.GetUserWalletByIdAsync(userId);
                if (wallet == null)
                {
                    return NotFound($"用戶 {userId} 沒有錢包");
                }
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得錢包資訊失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得遊戲統計資訊
        /// </summary>
        [HttpGet("users/{userId}/stats")]
        public async Task<ActionResult<object>> GetUserGameStats(int userId)
        {
            try
            {
                var pet = await _userRepository.GetPetByUserIdAsync(userId);
                var miniGames = await _userRepository.GetUserMiniGamesAsync(userId, 10);
                var signInStats = await _userRepository.GetUserSignInStatsAsync(userId, 30);
                var wallet = await _userRepository.GetUserWalletByIdAsync(userId);

                var stats = new
                {
                    pet = pet != null ? new
                    {
                        pet.PetID,
                        pet.PetName,
                        pet.Level,
                        pet.Experience,
                        pet.Hunger,
                        pet.Mood,
                        pet.Stamina,
                        pet.Cleanliness,
                        pet.Health
                    } : null,
                    miniGameCount = miniGames.Count,
                    signInCount = signInStats.Count,
                    walletBalance = wallet?.User_Point ?? 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得遊戲統計失敗: {ex.Message}");
            }
        }
    }
}
