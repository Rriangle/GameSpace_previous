using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class StockMovements
{
    public int MovementId { get; set; }
    public int ProductId { get; set; }
    public string MovementType { get; set; } = null!;
    public int Quantity { get; set; }
    public int? PreviousStock { get; set; }
    public int? NewStock { get; set; }
    public string? Reason { get; set; }
    public string? Reference { get; set; }
    public int? OrderId { get; set; }
    public int? UserId { get; set; }
    public int? ManagerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
    public string? Metadata { get; set; }
    public string? Settings { get; set; }
}