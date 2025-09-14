using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 簽到寫入專用存儲庫介面 - Stage 3 寫入操作
    /// 提供簽到相關的寫入操作，包含交易處理和冪等性
    /// </summary>
    public interface ISignInWriteRepository
    {
        /// <summary>
        /// 執行用戶簽到操作（包含交易處理和冪等性檢查）
        /// </summary>
        /// <param name="request">簽到請求</param>
        /// <returns>簽到響應</returns>
        Task<SignInResponse> ProcessSignInAsync(SignInRequest request);

        /// <summary>
        /// 檢查冪等性密鑰是否已存在
        /// </summary>
        /// <param name="idempotencyKey">冪等性密鑰</param>
        /// <returns>如果存在則返回先前的響應，否則返回 null</returns>
        Task<SignInResponse?> CheckIdempotencyAsync(string idempotencyKey);

        /// <summary>
        /// 保存冪等性記錄
        /// </summary>
        /// <param name="record">冪等性記錄</param>
        Task SaveIdempotencyRecordAsync(IdempotencyRecord record);

        /// <summary>
        /// 更新用戶錢包積分（包含錢包歷史記錄）
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="pointsToAdd">要增加的積分</param>
        /// <param name="description">描述</param>
        /// <param name="itemCode">項目代碼</param>
        /// <returns>更新後的總積分</returns>
        Task<int> UpdateUserPointsAsync(int userId, int pointsToAdd, string description, string? itemCode = null);

        /// <summary>
        /// 更新寵物經驗值
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="expToAdd">要增加的經驗值</param>
        /// <returns>是否升級</returns>
        Task<bool> UpdatePetExpAsync(int userId, int expToAdd);

        /// <summary>
        /// 生成隨機優惠券（如果符合條件）
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="consecutiveDays">連續簽到天數</param>
        /// <returns>優惠券代碼，如果沒有則返回 null</returns>
        Task<string?> GenerateRandomCouponAsync(int userId, int consecutiveDays);

        /// <summary>
        /// 獲取或創建用戶簽到統計
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>簽到統計</returns>
        Task<SignInStats> GetOrCreateSignInStatsAsync(int userId);

        /// <summary>
        /// 更新用戶簽到統計
        /// </summary>
        /// <param name="stats">簽到統計</param>
        Task UpdateSignInStatsAsync(SignInStats stats);
    }
}
