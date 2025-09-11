using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Web.Controllers
{
    /// <summary>
    /// 寵物 MVC 控制器
    /// </summary>
    public class PetController : Controller
    {
        private readonly IUserReadOnlyRepository _userRepository;

        public PetController(IUserReadOnlyRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 寵物首頁
        /// </summary>
        public async Task<IActionResult> Index(int userId)
        {
            try
            {
                var pet = await _userRepository.GetPetByUserIdAsync(userId);
                var miniGames = await _userRepository.GetUserMiniGamesAsync(userId, 10);
                var signInStats = await _userRepository.GetUserSignInStatsAsync(userId, 10);
                var wallet = await _userRepository.GetUserWalletByIdAsync(userId);

                ViewBag.Pet = pet;
                ViewBag.MiniGames = miniGames;
                ViewBag.SignInStats = signInStats;
                ViewBag.WalletBalance = wallet?.User_Point ?? 0;
                ViewBag.UserId = userId;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入寵物資訊失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 小遊戲頁
        /// </summary>
        public async Task<IActionResult> MiniGame(int userId, int limit = 20)
        {
            try
            {
                var pet = await _userRepository.GetPetByUserIdAsync(userId);
                var miniGames = await _userRepository.GetUserMiniGamesAsync(userId, limit);

                ViewBag.Pet = pet;
                ViewBag.MiniGames = miniGames;
                ViewBag.MiniGameCount = miniGames.Count;
                ViewBag.UserId = userId;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入小遊戲記錄失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 簽到頁
        /// </summary>
        public async Task<IActionResult> SignIn(int userId, int days = 30)
        {
            try
            {
                var signInStats = await _userRepository.GetUserSignInStatsAsync(userId, days);
                var wallet = await _userRepository.GetUserWalletByIdAsync(userId);

                ViewBag.SignInStats = signInStats;
                ViewBag.SignInCount = signInStats.Count;
                ViewBag.WalletBalance = wallet?.User_Point ?? 0;
                ViewBag.UserId = userId;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入簽到記錄失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 錢包頁
        /// </summary>
        public async Task<IActionResult> Wallet(int userId, int limit = 20)
        {
            try
            {
                var wallet = await _userRepository.GetUserWalletByIdAsync(userId);
                var walletHistory = await _userRepository.GetUserWalletHistoryAsync(userId, limit);
                var coupons = await _userRepository.GetUserCouponsAsync(userId, false);
                var evouchers = await _userRepository.GetUserEVouchersAsync(userId, false);

                ViewBag.WalletBalance = wallet?.User_Point ?? 0;
                ViewBag.WalletHistory = walletHistory;
                ViewBag.Coupons = coupons;
                ViewBag.EVouchers = evouchers;
                ViewBag.UserId = userId;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入錢包資訊失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 優惠券頁
        /// </summary>
        public async Task<IActionResult> Coupons(int userId, bool? isUsed = null)
        {
            try
            {
                var coupons = await _userRepository.GetUserCouponsAsync(userId, isUsed);

                ViewBag.Coupons = coupons;
                ViewBag.UserId = userId;
                ViewBag.IsUsed = isUsed;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入優惠券失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 禮券頁
        /// </summary>
        public async Task<IActionResult> EVouchers(int userId, bool? isUsed = null)
        {
            try
            {
                var evouchers = await _userRepository.GetUserEVouchersAsync(userId, isUsed);

                ViewBag.EVouchers = evouchers;
                ViewBag.UserId = userId;
                ViewBag.IsUsed = isUsed;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入禮券失敗: {ex.Message}";
                return View();
            }
        }
    }
}
