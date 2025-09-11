using Microsoft.AspNetCore.Mvc;

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
            return View();
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