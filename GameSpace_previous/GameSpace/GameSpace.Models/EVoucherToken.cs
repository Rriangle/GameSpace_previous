using System;

namespace GameSpace.Models
{
    public class EVoucherToken
    {
        public int TokenID { get; set; }
        public int VoucherID { get; set; }
        public int UserID { get; set; }
        public string TokenCode { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
