using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class PaymentTransactions
{
    public int TransactionId { get; set; }
    public string TransactionCode { get; set; } = null!;
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string PaymentProvider { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string? PaymentReference { get; set; }
    public string? PaymentGatewayResponse { get; set; }
    public string? PaymentGatewayTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? CancellationReason { get; set; }
    public string? RefundReason { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? RefundReference { get; set; }
    public string? Notes { get; set; }
    public string? Metadata { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
    public string? Settings { get; set; }
}