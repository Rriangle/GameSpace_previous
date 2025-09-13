using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 電子禮券兌換記錄模型
    /// </summary>
    public partial class EVoucherRedeemLog
    {
        [Key]
        [Column("LogID")]
        public int LogId { get; set; }

        [Column("EVoucherID")]
        public int EVoucherId { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [Column("RedeemTime")]
        public DateTime RedeemTime { get; set; }

        [Column("RedeemAmount")]
        public decimal RedeemAmount { get; set; }

        [StringLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [StringLength(100)]
        [Column("Status")]
        public string Status { get; set; } = null!; // "Success", "Failed", "Pending"

        // 導航屬性
        [ForeignKey("EVoucherId")]
        public virtual EVoucher EVoucher { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}