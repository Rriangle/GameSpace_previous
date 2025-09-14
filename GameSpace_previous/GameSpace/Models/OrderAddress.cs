using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 訂單地址模型
    /// </summary>
    [Table("OrderAddresses")]
    public class OrderAddress
    {
        [Key]
        [Column("AddressID")]
        public int AddressId { get; set; }

        [Required]
        [Column("OrderID")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("RecipientName")]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Column("Address")]
        public string Address { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("City")]
        public string? City { get; set; }

        [StringLength(20)]
        [Column("District")]
        public string? District { get; set; }

        [StringLength(10)]
        [Column("PostalCode")]
        public string? PostalCode { get; set; }

        [Column("IsDefault")]
        public bool IsDefault { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        // 導航屬性
        [ForeignKey("OrderId")]
        public virtual OrderInfo Order { get; set; } = null!;
    }
}