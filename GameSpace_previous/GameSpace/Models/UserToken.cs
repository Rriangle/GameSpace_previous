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
        [Column("Token_ID")]
        public int TokenId { get; set; }

        /// <summary>
        /// 用戶ID（外鍵）
        /// </summary>
        [Required]
        [Column("User_ID")]
        public int UserId { get; set; }

        /// <summary>
        /// 提供者名稱（如Google、Facebook、Discord）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// 令牌名稱（如access_token、refresh_token）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 令牌值
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 過期時間
        /// </summary>
        [Required]
        [Column("ExpireAt")]
        public DateTime ExpireAt { get; set; }

        /// <summary>
        /// 導航屬性 - 關聯的用戶
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}