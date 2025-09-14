using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 服務提供商模型
    /// </summary>
    [Table("Provider")]
    public class Provider
    {
        [Key]
        [Column("ProviderID")]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("ProviderName")]
        public string ProviderName { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("ProviderType")]
        public string? ProviderType { get; set; }

        [StringLength(200)]
        [Column("Description")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Column("ApiEndpoint")]
        public string? ApiEndpoint { get; set; }

        [StringLength(200)]
        [Column("ApiKey")]
        public string? ApiKey { get; set; }

        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Active";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}