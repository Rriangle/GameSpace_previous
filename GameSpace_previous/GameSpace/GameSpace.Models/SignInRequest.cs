using System.ComponentModel.DataAnnotations;

namespace GameSpace.Models
{
    /// <summary>
    /// 簽到請求模型 - 已擴展為 Stage 3 需求
    /// </summary>
    public class SignInRequest
    {
        /// <summary>
        /// 用戶ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 冪等性金鑰
        /// </summary>
        [Required]
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// 簽到類型（每日、每週等）
        /// </summary>
        public string SignInType { get; set; } = "daily";

        // ===== Stage 3 擴展欄位 =====

        /// <summary>
        /// 簽到時間戳（可選，預設為當前時間）（Stage 3 新增）
        /// </summary>
        public DateTime? SignInTime { get; set; }
    }
}
