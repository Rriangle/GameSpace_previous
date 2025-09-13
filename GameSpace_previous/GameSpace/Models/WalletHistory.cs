using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 錢包交易歷史資料表
    /// </summary>
    public partial class WalletHistory
    {
        public int HistoryId { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Users User { get; set; } = null!;
        public virtual UserWallet UserWallet { get; set; } = null!;
    }
}