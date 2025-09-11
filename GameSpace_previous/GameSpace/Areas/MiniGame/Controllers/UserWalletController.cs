using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 使用者錢包控制器 - 負責處理積分、優惠券、電子禮券相關功能
    /// 對應資料表：User_Wallet, CouponType, Coupon, EVoucherType, EVoucher, EVoucherToken, EVoucherRedeemLog, WalletHistory
    /// </summary>
    [Area("MiniGame")]
    public class UserWalletController : Controller
    {
        /// <summary>
        /// 錢包總覽頁面 - 顯示目前積分、優惠券、電子禮券摘要
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "我的錢包";
            
            // 模擬資料 - 對應 database.sql 架構，實際邏輯將在 Stage 4 實作
            var viewModel = new WalletOverviewDisplayViewModel
            {
                UserId = 1,
                UserName = "測試使用者",
                CurrentPoints = 1250,
                AvailableCouponsCount = 5,
                UsedCouponsCount = 3,
                AvailableEVouchersCount = 2,
                UsedEVouchersCount = 1,
                MonthlyPointsEarned = 480,
                MonthlyPointsSpent = 320,
                RecentTransactions = new List<WalletHistoryViewModel>
                {
                    new WalletHistoryViewModel { LogID = 1, ChangeType = "獲得", PointsChanged = 10, Description = "每日簽到", ChangeTime = DateTime.Now.AddHours(-2) },
                    new WalletHistoryViewModel { LogID = 2, ChangeType = "獲得", PointsChanged = 50, Description = "完成小遊戲", ChangeTime = DateTime.Now.AddHours(-5) },
                    new WalletHistoryViewModel { LogID = 3, ChangeType = "消費", PointsChanged = -30, Description = "兌換優惠券", ChangeTime = DateTime.Now.AddDays(-1) }
                },
                AvailableCoupons = new List<CouponViewModel>
                {
                    new CouponViewModel { CouponID = 1, CouponCode = "GAME50", IsUsed = false, AcquiredTime = DateTime.Now.AddDays(-3) },
                    new CouponViewModel { CouponID = 2, CouponCode = "SHOP20", IsUsed = false, AcquiredTime = DateTime.Now.AddDays(-7) }
                },
                AvailableEVouchers = new List<EVoucherViewModel>
                {
                    new EVoucherViewModel { EVoucherID = 1, EVoucherCode = "EV100NT", IsUsed = false, AcquiredTime = DateTime.Now.AddDays(-5) }
                }
            };
            
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
    }
}