using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Web.Controllers
{
    /// <summary>
    /// �d�� MVC ���
    /// </summary>
    public class PetController : Controller
    {
        private readonly IUserReadOnlyRepository _userRepository;

        public PetController(IUserReadOnlyRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// �d������
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
                ViewBag.Error = $"���J�d����T����: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// �p�C����
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
                ViewBag.Error = $"���J�p�C���O������: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// ñ�쭶
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
                ViewBag.Error = $"���Jñ��O������: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// ���]��
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
                ViewBag.Error = $"���J���]��T����: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// �u�f�魶
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
                ViewBag.Error = $"���J�u�f�饢��: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// §�魶
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
                ViewBag.Error = $"���J§�饢��: {ex.Message}";
                return View();
            }
        }
    }
}
