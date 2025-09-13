using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶錢包資料表
    /// </summary>
    public partial class UserWallet
    {
        public int UserId { get; set; }
        public decimal UserPoint { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Users User { get; set; } = null!;
        public virtual ICollection<WalletHistory> WalletHistories { get; set; } = new List<WalletHistory>();
    }
}