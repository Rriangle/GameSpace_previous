using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 庫存變動模型
    /// </summary>
    [Table("StockMovements")]
    public class StockMovement
    {
        [Key]
        [Column("MovementID")]
        public int MovementId { get; set; }

        [Required]
        [Column("ProductID")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("MovementType")]
        public string MovementType { get; set; } = string.Empty; // IN, OUT, ADJUSTMENT

        [Column("Quantity")]
        public int Quantity { get; set; }

        [Column("PreviousStock")]
        public int PreviousStock { get; set; }

        [Column("NewStock")]
        public int NewStock { get; set; }

        [StringLength(200)]
        [Column("Reason")]
        public string? Reason { get; set; }

        [StringLength(50)]
        [Column("Reference")]
        public string? Reference { get; set; } // 訂單號、調撥單號等

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        // 導航屬性
        [ForeignKey("ProductId")]
        public virtual ProductInfo Product { get; set; } = null!;
    }
}