using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶令牌模型 - 用於OAuth認證和第三方登入
    /// </summary>
    [Table("UserTokens")]
    public class UserToken
    {
        /// <summary>
        /// 令牌ID（主鍵）
        /// </summary>
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }

        /// <summary>
        /// 用戶ID（外鍵）
        /// </summary>
        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// 令牌值
        /// </summary>
        [Required]
        [Column("token")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 令牌類型
        /// </summary>
        [Required]
        [Column("token_type")]
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// 用途
        /// </summary>
        [Column("purpose")]
        public string? Purpose { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 過期時間
        /// </summary>
        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// 使用時間
        /// </summary>
        [Column("used_at")]
        public DateTime? UsedAt { get; set; }

        /// <summary>
        /// 使用IP
        /// </summary>
        [Column("used_ip")]
        public string? UsedIp { get; set; }

        /// <summary>
        /// 使用User Agent
        /// </summary>
        [Column("used_user_agent")]
        public string? UsedUserAgent { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [Column("status")]
        public string? Status { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Column("is_active")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [Column("notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// 元數據
        /// </summary>
        [Column("metadata")]
        public string? Metadata { get; set; }

        /// <summary>
        /// 設置
        /// </summary>
        [Column("settings")]
        public string? Settings { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 是否刪除
        /// </summary>
        [Column("is_deleted")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 刪除時間
        /// </summary>
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// 刪除者
        /// </summary>
        [Column("deleted_by")]
        public string? DeletedBy { get; set; }

        /// <summary>
        /// 刪除原因
        /// </summary>
        [Column("delete_reason")]
        public string? DeleteReason { get; set; }

        /// <summary>
        /// 導航屬性 - 關聯的用戶
        /// </summary>
        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }
    }
}