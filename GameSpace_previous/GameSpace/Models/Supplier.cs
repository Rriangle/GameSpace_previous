using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 供應商模型
    /// </summary>
    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        [Column("SupplierID")]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("SupplierName")]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("ContactPerson")]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        [Column("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [Column("Email")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Column("Address")]
        public string? Address { get; set; }

        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Active";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<ProductInfo> Products { get; set; } = new List<ProductInfo>();
    }
}