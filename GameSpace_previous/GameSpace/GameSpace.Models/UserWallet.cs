using System;

namespace GameSpace.Models
{
    public class UserWallet
    {
        public int WalletID { get; set; }
        public int UserID { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
