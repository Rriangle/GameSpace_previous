using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 電子禮券代幣模型
    /// </summary>
    public partial class EVoucherToken
    {
        [Key]
        [Column("TokenID")]
        public int TokenId { get; set; }

        [Column("EVoucherID")]
        public int EVoucherId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TokenValue")]
        public string TokenValue { get; set; } = null!;

        [Column("IsUsed")]
        public bool IsUsed { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("ExpiresAt")]
        public DateTime? ExpiresAt { get; set; }

        [Column("UsedAt")]
        public DateTime? UsedAt { get; set; }

        // 導航屬性
        [ForeignKey("EVoucherId")]
        public virtual EVoucher EVoucher { get; set; } = null!;
    }
}