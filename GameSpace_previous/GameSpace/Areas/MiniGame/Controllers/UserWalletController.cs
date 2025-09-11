using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;
using GameSpace.Areas.MiniGame.Services;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 使用者錢包控制器 - 負責處理積分、優惠券、電子禮券相關功能
    /// 對應資料表：User_Wallet, CouponType, Coupon, EVoucherType, EVoucher, EVoucherToken, EVoucherRedeemLog, WalletHistory
    /// </summary>
    [Area("MiniGame")]
    public class UserWalletController : Controller
    {
        private readonly IWalletService _walletService;

        public UserWalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }
        /// <summary>
        /// 錢包總覽頁面 - 顯示目前積分、優惠券、電子禮券摘要
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "我的錢包";
            
            const int currentUserId = 1; // 實際會從認證系統取得
            
            // 使用錢包服務取得實際總覽資料
            var viewModel = await _walletService.GetWalletOverviewAsync(currentUserId);
            
            return View(viewModel);
        }

        /// <summary>
        /// 積分詳細資訊頁面
        /// </summary>
        public IActionResult Points()
        {
            ViewData["Title"] = "積分明細";
            return View();
        }

        /// <summary>
        /// 優惠券管理頁面
        /// </summary>
        public IActionResult Coupons()
        {
            ViewData["Title"] = "我的優惠券";
            return View();
        }

        /// <summary>
        /// 電子禮券管理頁面
        /// </summary>
        public IActionResult EVouchers()
        {
            ViewData["Title"] = "我的電子禮券";
            return View();
        }

        /// <summary>
        /// 錢包交易歷史頁面
        /// </summary>
        public IActionResult History()
        {
            ViewData["Title"] = "交易紀錄";
            return View();
        }

        /// <summary>
        /// 使用優惠券 - AJAX API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UseCoupon([FromBody] UseCouponRequest request)
        {
            const int currentUserId = 1; // 實際會從認證系統取得

            try
            {
                var result = await _walletService.UseCouponAsync(request.CouponId, currentUserId, request.OrderId);
                
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    couponCode = result.CouponCode,
                    remainingPoints = result.RemainingPoints,
                    operationTime = result.OperationTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "使用優惠券時發生錯誤" });
            }
        }

        /// <summary>
        /// 兌換優惠券 - AJAX API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExchangeCoupon([FromBody] ExchangeCouponRequest request)
        {
            const int currentUserId = 1; // 實際會從認證系統取得

            try
            {
                var result = await _walletService.ExchangeCouponAsync(request.CouponTypeId, currentUserId);
                
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    couponCode = result.CouponCode,
                    pointsCost = result.PointsCost,
                    remainingPoints = result.RemainingPoints,
                    operationTime = result.OperationTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "兌換優惠券時發生錯誤" });
            }
        }

        /// <summary>
        /// 使用電子禮券 - AJAX API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UseEVoucher([FromBody] UseEVoucherRequest request)
        {
            const int currentUserId = 1; // 實際會從認證系統取得

            try
            {
                var result = await _walletService.UseEVoucherAsync(request.EVoucherId, currentUserId);
                
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    voucherCode = result.EVoucherCode,
                    remainingPoints = result.RemainingPoints,
                    operationTime = result.OperationTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "使用電子禮券時發生錯誤" });
            }
        }

        /// <summary>
        /// 兌換電子禮券 - AJAX API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExchangeEVoucher([FromBody] ExchangeEVoucherRequest request)
        {
            const int currentUserId = 1; // 實際會從認證系統取得

            try
            {
                var result = await _walletService.ExchangeEVoucherAsync(request.EVoucherTypeId, currentUserId);
                
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    voucherCode = result.EVoucherCode,
                    pointsCost = result.PointsCost,
                    remainingPoints = result.RemainingPoints,
                    generatedToken = result.GeneratedToken,
                    tokenExpiresAt = result.TokenExpiresAt?.ToString("yyyy-MM-dd HH:mm:ss"),
                    operationTime = result.OperationTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "兌換電子禮券時發生錯誤" });
            }
        }
    }

    /// <summary>
    /// 使用優惠券請求模型
    /// </summary>
    public class UseCouponRequest
    {
        public int CouponId { get; set; }
        public int? OrderId { get; set; }
    }

    /// <summary>
    /// 兌換優惠券請求模型
    /// </summary>
    public class ExchangeCouponRequest
    {
        public int CouponTypeId { get; set; }
    }

    /// <summary>
    /// 使用電子禮券請求模型
    /// </summary>
    public class UseEVoucherRequest
    {
        public int EVoucherId { get; set; }
    }

    /// <summary>
    /// 兌換電子禮券請求模型
    /// </summary>
    public class ExchangeEVoucherRequest
    {
        public int EVoucherTypeId { get; set; }
    }
}