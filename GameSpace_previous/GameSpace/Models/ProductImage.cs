using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 產品圖片模型
    /// </summary>
    [Table("ProductImages")]
    public class ProductImage
    {
        [Key]
        [Column("ImageID")]
        public int ImageId { get; set; }

        [Required]
        [Column("ProductID")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(500)]
        [Column("ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("AltText")]
        public string? AltText { get; set; }

        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("IsPrimary")]
        public bool IsPrimary { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        // 導航屬性
        [ForeignKey("ProductId")]
        public virtual ProductInfo Product { get; set; } = null!;
    }
}