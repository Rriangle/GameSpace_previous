using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 錢包服務介面 - 處理積分、優惠券、電子禮券相關業務邏輯
    /// 對應 database.sql User_Wallet, Coupon, CouponType, EVoucher, EVoucherType, WalletHistory 資料表
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// 取得錢包總覽資料
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>錢包總覽顯示模型</returns>
        Task<WalletOverviewDisplayViewModel> GetWalletOverviewAsync(int userId);

        /// <summary>
        /// 使用優惠券 - 將優惠券標記為已使用
        /// </summary>
        /// <param name="couponId">優惠券編號</param>
        /// <param name="userId">使用者編號</param>
        /// <param name="orderId">使用於訂單編號（可選）</param>
        /// <returns>使用結果</returns>
        Task<CouponOperationResultViewModel> UseCouponAsync(int couponId, int userId, int? orderId = null);

        /// <summary>
        /// 兌換新優惠券 - 使用積分兌換指定類型的優惠券
        /// </summary>
        /// <param name="couponTypeId">優惠券類型編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>兌換結果</returns>
        Task<CouponOperationResultViewModel> ExchangeCouponAsync(int couponTypeId, int userId);

        /// <summary>
        /// 使用電子禮券 - 將電子禮券標記為已使用
        /// </summary>
        /// <param name="voucherId">電子禮券編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>使用結果</returns>
        Task<EVoucherOperationResultViewModel> UseEVoucherAsync(int voucherId, int userId);

        /// <summary>
        /// 兌換新電子禮券 - 使用積分兌換指定類型的電子禮券
        /// </summary>
        /// <param name="voucherTypeId">電子禮券類型編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>兌換結果</returns>
        Task<EVoucherOperationResultViewModel> ExchangeEVoucherAsync(int voucherTypeId, int userId);

        /// <summary>
        /// 取得錢包交易歷史
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <param name="pageIndex">頁面索引</param>
        /// <param name="pageSize">每頁記錄數</param>
        /// <returns>交易歷史列表</returns>
        Task<List<WalletHistoryViewModel>> GetWalletHistoryAsync(int userId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// 取得可兌換的優惠券類型列表
        /// </summary>
        /// <returns>優惠券類型列表</returns>
        Task<List<CouponTypeViewModel>> GetAvailableCouponTypesAsync();

        /// <summary>
        /// 取得可兌換的電子禮券類型列表
        /// </summary>
        /// <returns>電子禮券類型列表</returns>
        Task<List<EVoucherTypeViewModel>> GetAvailableEVoucherTypesAsync();

        /// <summary>
        /// 檢查使用者是否擁有指定優惠券
        /// </summary>
        /// <param name="couponId">優惠券編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>是否擁有</returns>
        Task<bool> ValidateCouponOwnershipAsync(int couponId, int userId);

        /// <summary>
        /// 檢查使用者是否擁有指定電子禮券
        /// </summary>
        /// <param name="voucherId">電子禮券編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>是否擁有</returns>
        Task<bool> ValidateEVoucherOwnershipAsync(int voucherId, int userId);
    }

    /// <summary>
    /// 優惠券操作結果視圖模型
    /// </summary>
    public class CouponOperationResultViewModel
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 結果訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 操作類型（使用/兌換）
        /// </summary>
        public string OperationType { get; set; } = string.Empty;

        /// <summary>
        /// 優惠券代碼
        /// </summary>
        public string? CouponCode { get; set; }

        /// <summary>
        /// 消耗的積分（兌換時）
        /// </summary>
        public int PointsCost { get; set; }

        /// <summary>
        /// 使用者剩餘積分
        /// </summary>
        public int RemainingPoints { get; set; }

        /// <summary>
        /// 操作時間
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 相關優惠券詳細資訊
        /// </summary>
        public CouponViewModel? CouponDetails { get; set; }
    }

    /// <summary>
    /// 電子禮券操作結果視圖模型
    /// </summary>
    public class EVoucherOperationResultViewModel
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 結果訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 操作類型（使用/兌換）
        /// </summary>
        public string OperationType { get; set; } = string.Empty;

        /// <summary>
        /// 電子禮券代碼
        /// </summary>
        public string? EVoucherCode { get; set; }

        /// <summary>
        /// 消耗的積分（兌換時）
        /// </summary>
        public int PointsCost { get; set; }

        /// <summary>
        /// 使用者剩餘積分
        /// </summary>
        public int RemainingPoints { get; set; }

        /// <summary>
        /// 操作時間
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 產生的令牌（兌換時）
        /// </summary>
        public string? GeneratedToken { get; set; }

        /// <summary>
        /// 令牌到期時間（兌換時）
        /// </summary>
        public DateTime? TokenExpiresAt { get; set; }

        /// <summary>
        /// 相關電子禮券詳細資訊
        /// </summary>
        public EVoucherViewModel? EVoucherDetails { get; set; }
    }
}