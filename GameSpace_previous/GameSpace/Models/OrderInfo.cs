using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 訂單資訊模型
    /// </summary>
    [Table("OrderInfo")]
    public class OrderInfo
    {
        [Key]
        [Column("OrderID")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("OrderCode")]
        public string OrderCode { get; set; } = string.Empty;

        [Required]
        [Column("User_ID")]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("OrderStatus")]
        public string OrderStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("PaymentStatus")]
        public string PaymentStatus { get; set; } = string.Empty;

        [Column("OrderTotal", TypeName = "decimal(18,2)")]
        public decimal OrderTotal { get; set; }

        [Column("Paid")]
        public bool Paid { get; set; }

        [Column("PaymentAt")]
        public DateTime? PaymentAt { get; set; }

        [Column("Shipped")]
        public bool Shipped { get; set; }

        [Column("ShippedAt")]
        public DateTime? ShippedAt { get; set; }

        [Column("Completed")]
        public bool Completed { get; set; }

        [Column("CompletedAt")]
        public DateTime? CompletedAt { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<OrderAddress> OrderAddresses { get; set; } = new List<OrderAddress>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    }
}