using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class Support_Tickets
{
    public int TicketId { get; set; }
    public string TicketNumber { get; set; } = null!;
    public int UserId { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? SubCategory { get; set; }
    public int? AssignedManagerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public string? CloseReason { get; set; }
    public string? Resolution { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public string? Metadata { get; set; }
    public string? Settings { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}