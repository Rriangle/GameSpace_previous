using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 電子禮券類型模型
    /// </summary>
    public partial class EVoucherType
    {
        [Key]
        [Column("EVoucherTypeID")]
        public int EVoucherTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TypeName")]
        public string TypeName { get; set; } = null!;

        [StringLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("Value")]
        public decimal Value { get; set; }

        [Column("Currency")]
        [StringLength(10)]
        public string Currency { get; set; } = "TWD";

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("ValidFrom")]
        public DateTime ValidFrom { get; set; }

        [Column("ValidTo")]
        public DateTime ValidTo { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<EVoucher> EVouchers { get; set; } = new List<EVoucher>();
    }
}