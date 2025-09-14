using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 產品資訊模型
    /// </summary>
    [Table("ProductInfo")]
    public class ProductInfo
    {
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("ProductCode")]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Column("ProductName")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column("Cost", TypeName = "decimal(18,2)")]
        public decimal? Cost { get; set; }

        [Column("Stock")]
        public int Stock { get; set; }

        [Column("MinStock")]
        public int? MinStock { get; set; }

        [Column("MaxStock")]
        public int? MaxStock { get; set; }

        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Active";

        [StringLength(50)]
        [Column("Category")]
        public string? Category { get; set; }

        [StringLength(50)]
        [Column("Brand")]
        public string? Brand { get; set; }

        [Column("Weight", TypeName = "decimal(10,3)")]
        public decimal? Weight { get; set; }

        [StringLength(20)]
        [Column("Unit")]
        public string? Unit { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("UpdatedBy")]
        public int? UpdatedBy { get; set; }

        // 導航屬性
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}