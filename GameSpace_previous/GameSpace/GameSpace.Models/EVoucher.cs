using System;

namespace GameSpace.Models
{
    public class EVoucher
    {
        public int VoucherID { get; set; }
        public int TypeID { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public string VoucherName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
