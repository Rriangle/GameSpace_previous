using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 優惠券類型模型
    /// </summary>
    public partial class CouponType
    {
        [Key]
        [Column("CouponTypeID")]
        public int CouponTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TypeName")]
        public string TypeName { get; set; } = null!;

        [StringLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("DiscountType")]
        [StringLength(20)]
        public string DiscountType { get; set; } = null!; // "Percentage" 或 "FixedAmount"

        [Column("DiscountValue")]
        public decimal DiscountValue { get; set; }

        [Column("MinOrderAmount")]
        public decimal? MinOrderAmount { get; set; }

        [Column("MaxDiscountAmount")]
        public decimal? MaxDiscountAmount { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("ValidFrom")]
        public DateTime ValidFrom { get; set; }

        [Column("ValidTo")]
        public DateTime ValidTo { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    }
}