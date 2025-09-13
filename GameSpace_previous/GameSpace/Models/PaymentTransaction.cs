using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 支付交易模型
    /// </summary>
    [Table("PaymentTransactions")]
    public class PaymentTransaction
    {
        [Key]
        [Column("TransactionID")]
        public int TransactionId { get; set; }

        [Required]
        [Column("OrderID")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("TransactionCode")]
        public string TransactionCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column("Amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("Provider")]
        public string? Provider { get; set; }

        [StringLength(200)]
        [Column("ProviderTransactionId")]
        public string? ProviderTransactionId { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("CompletedAt")]
        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        [Column("Note")]
        public string? Note { get; set; }

        // 導航屬性
        [ForeignKey("OrderId")]
        public virtual OrderInfo Order { get; set; } = null!;
    }
}