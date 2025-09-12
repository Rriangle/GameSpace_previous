using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Wallet;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.MemberManagement.Controllers
{
    [Area("MemberManagement")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var wallet = await _walletService.GetUserWalletAsync(userId);
            var history = await _walletService.GetWalletHistoryAsync(userId, 1, 20);
            var coupons = await _walletService.GetUserCouponsAsync(userId, false);
            var evouchers = await _walletService.GetUserEvouchersAsync(userId, false);

            var viewModel = new WalletViewModel
            {
                Wallet = wallet,
                History = history,
                Coupons = coupons,
                Evouchers = evouchers
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddPoints(int amount, string description)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _walletService.AddPointsAsync(userId, amount, description);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add points for user {UserId}", userId);
                TempData["ErrorMessage"] = "Failed to add points. Please try again later.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RedeemCoupon(int couponTypeId)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _walletService.RedeemCouponAsync(userId, couponTypeId);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Coupon redeemed successfully! Code: {result.CouponCode}";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to redeem coupon for user {UserId}", userId);
                TempData["ErrorMessage"] = "優惠券兌換失敗，請稍後再試。";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RedeemEvoucher(int evoucherTypeId)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _walletService.RedeemEvoucherAsync(userId, evoucherTypeId);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"電子禮券兌換成功！券號：{result.EvoucherCode}";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to redeem evoucher for user {UserId}", userId);
                TempData["ErrorMessage"] = "電子禮券兌換失敗，請稍後再試。";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> History(int page = 1)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var history = await _walletService.GetWalletHistoryAsync(userId, page, 20);
            return View(history);
        }
    }

    public class WalletViewModel
    {
        public UserWallet? Wallet { get; set; }
        public IEnumerable<WalletHistory> History { get; set; } = new List<WalletHistory>();
        public IEnumerable<Coupon> Coupons { get; set; } = new List<Coupon>();
        public IEnumerable<Evoucher> Evouchers { get; set; } = new List<Evoucher>();
    }
}