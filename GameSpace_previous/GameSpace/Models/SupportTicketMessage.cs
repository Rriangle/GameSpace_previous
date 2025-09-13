using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 客服工單訊息模型
    /// </summary>
    [Table("Support_Ticket_Messages")]
    public class SupportTicketMessage
    {
        [Key]
        [Column("MessageID")]
        public int MessageId { get; set; }

        [Required]
        [Column("TicketID")]
        public int TicketId { get; set; }

        [Column("SenderUser")]
        public int? SenderUser { get; set; }

        [Column("SenderManager")]
        public int? SenderManager { get; set; }

        [Required]
        [Column("Message", TypeName = "text")]
        public string Message { get; set; } = string.Empty;

        [Column("IsRead")]
        public bool IsRead { get; set; }

        [Column("ReadAt")]
        public DateTime? ReadAt { get; set; }

        [Column("SentAt")]
        public DateTime SentAt { get; set; }

        // 導航屬性
        [ForeignKey("TicketId")]
        public virtual SupportTicket Ticket { get; set; } = null!;

        [ForeignKey("SenderUser")]
        public virtual Users? SenderUserNavigation { get; set; }

        [ForeignKey("SenderManager")]
        public virtual ManagerData? SenderManagerNavigation { get; set; }
    }
}