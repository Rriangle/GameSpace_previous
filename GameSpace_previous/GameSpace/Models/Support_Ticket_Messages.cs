using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class Support_Ticket_Messages
{
    public int MessageId { get; set; }
    public int TicketId { get; set; }
    public int? SenderUserId { get; set; }
    public int? SenderManagerId { get; set; }
    public string MessageText { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public int? ReadByUserId { get; set; }
    public int? ReadByManagerId { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public int? DeletedByManagerId { get; set; }
    public string? MessageType { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
    public int? ReplyToMessageId { get; set; }
    public string? Status { get; set; }
    public string? Metadata { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Priority { get; set; }
    public bool? IsInternal { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }
    public int? PinOrder { get; set; }
    public string? PinnedBy { get; set; }
    public string? PinReason { get; set; }
    public bool? IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivedBy { get; set; }
    public string? ArchiveReason { get; set; }
    public string? Settings { get; set; }
}