using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 客服工單分配模型
    /// </summary>
    [Table("Support_Ticket_Assignments")]
    public class SupportTicketAssignment
    {
        [Key]
        [Column("AssignmentID")]
        public int AssignmentId { get; set; }

        [Required]
        [Column("TicketID")]
        public int TicketId { get; set; }

        [Required]
        [Column("AssignedTo")]
        public int AssignedTo { get; set; }

        [Required]
        [Column("AssignedBy")]
        public int AssignedBy { get; set; }

        [Column("AssignedAt")]
        public DateTime AssignedAt { get; set; }

        [StringLength(200)]
        [Column("Note")]
        public string? Note { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // 導航屬性
        [ForeignKey("TicketId")]
        public virtual SupportTicket Ticket { get; set; } = null!;

        [ForeignKey("AssignedTo")]
        public virtual ManagerData AssignedToManager { get; set; } = null!;

        [ForeignKey("AssignedBy")]
        public virtual ManagerData AssignedByManager { get; set; } = null!;
    }
}