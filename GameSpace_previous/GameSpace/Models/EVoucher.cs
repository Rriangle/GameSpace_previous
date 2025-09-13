using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 電子禮券模型
    /// </summary>
    public partial class EVoucher
    {
        [Key]
        [Column("EVoucherID")]
        public int EVoucherId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("EVoucherCode")]
        public string EVoucherCode { get; set; } = null!;

        [Column("EVoucherTypeID")]
        public int EVoucherTypeId { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [Column("IsUsed")]
        public bool IsUsed { get; set; }

        [Column("AcquiredTime")]
        public DateTime AcquiredTime { get; set; }

        [Column("UsedTime")]
        public DateTime? UsedTime { get; set; }

        [Column("UsedInOrderID")]
        public int? UsedInOrderId { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        // 導航屬性
        [ForeignKey("EVoucherTypeId")]
        public virtual EVoucherType EVoucherType { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("UsedInOrderId")]
        public virtual OrderInfo? UsedInOrder { get; set; }

        public virtual ICollection<EVoucherRedeemLog> EVoucherRedeemLogs { get; set; } = new List<EVoucherRedeemLog>();

        public virtual ICollection<EVoucherToken> EVoucherTokens { get; set; } = new List<EVoucherToken>();
    }
}