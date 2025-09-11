using System;

namespace GameSpace.Models
{
    public class EVoucherRedeemLog
    {
        public int LogID { get; set; }
        public int TokenID { get; set; }
        public int UserID { get; set; }
        public DateTime RedeemedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
