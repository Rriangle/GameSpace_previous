using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 訂單狀態歷史模型
    /// </summary>
    [Table("OrderStatusHistory")]
    public class OrderStatusHistory
    {
        [Key]
        [Column("HistoryID")]
        public int HistoryId { get; set; }

        [Required]
        [Column("OrderID")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("Note")]
        public string? Note { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        // 導航屬性
        [ForeignKey("OrderId")]
        public virtual OrderInfo Order { get; set; } = null!;
    }
}