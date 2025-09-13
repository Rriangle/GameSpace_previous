using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class Support_Ticket_Assignments
{
    public int AssignmentId { get; set; }
    public int TicketId { get; set; }
    public int ManagerId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public DateTime? UnassignedAt { get; set; }
    public string? AssignedBy { get; set; }
    public string? UnassignedBy { get; set; }
    public string? AssignmentReason { get; set; }
    public string? UnassignmentReason { get; set; }
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
    public string? Metadata { get; set; }
    public string? Settings { get; set; }
}