using System;

namespace GameSpace.Models
{
    public class Coupon
    {
        public int CouponID { get; set; }
        public int TypeID { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string CouponName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
