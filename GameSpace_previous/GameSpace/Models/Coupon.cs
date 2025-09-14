using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 優惠券模型
    /// </summary>
    public partial class Coupon
    {
        [Key]
        [Column("CouponID")]
        public int CouponId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("CouponCode")]
        public string CouponCode { get; set; } = null!;

        [Column("CouponTypeID")]
        public int CouponTypeId { get; set; }

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

        // 導航屬性
        [ForeignKey("CouponTypeId")]
        public virtual CouponType CouponType { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("UsedInOrderId")]
        public virtual OrderInfo? UsedInOrder { get; set; }
    }
}