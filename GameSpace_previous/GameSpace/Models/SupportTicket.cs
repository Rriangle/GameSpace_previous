using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 客服工單模型
    /// </summary>
    [Table("Support_Tickets")]
    public class SupportTicket
    {
        [Key]
        [Column("TicketID")]
        public int TicketId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("TicketCode")]
        public string TicketCode { get; set; } = string.Empty;

        [Required]
        [Column("User_ID")]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column("Description", TypeName = "text")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Open";

        [StringLength(20)]
        [Column("Priority")]
        public string Priority { get; set; } = "Medium";

        [StringLength(50)]
        [Column("Category")]
        public string? Category { get; set; }

        [Column("AssignedManager")]
        public int? AssignedManager { get; set; }

        [Column("ClosedBy")]
        public int? ClosedBy { get; set; }

        [Column("ClosedAt")]
        public DateTime? ClosedAt { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        public virtual ICollection<SupportTicketMessage> Messages { get; set; } = new List<SupportTicketMessage>();
        public virtual ICollection<SupportTicketAssignment> Assignments { get; set; } = new List<SupportTicketAssignment>();
    }
}