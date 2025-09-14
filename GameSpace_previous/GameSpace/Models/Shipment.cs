using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class Shipment
{
    public int ShipmentId { get; set; }
    public int OrderId { get; set; }
    public string TrackingNumber { get; set; } = null!;
    public string? Carrier { get; set; }
    public string? ServiceType { get; set; }
    public string? Status { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ExpectedDeliveryAt { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? Notes { get; set; }
    public decimal? ShippingCost { get; set; }
    public string? CurrencyCode { get; set; }
    public string? PackageType { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? InsuranceValue { get; set; }
    public bool? RequiresSignature { get; set; }
    public bool? IsFragile { get; set; }
    public bool? IsHazardous { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? DeliveryInstructions { get; set; }
    public string? DeliveryTimePreference { get; set; }
    public string? DeliveryMethod { get; set; }
    public string? DeliveryCompany { get; set; }
    public string? DeliveryStatus { get; set; }
    public string? DeliveredTo { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? SignatureNotes { get; set; }
    public string? DeliveryProof { get; set; }
    public string? DeliveryPhoto { get; set; }
    public string? DeliveryVideo { get; set; }
    public string? DeliveryAudio { get; set; }
    public string? DeliveryLocation { get; set; }
    public decimal? DeliveryLatitude { get; set; }
    public decimal? DeliveryLongitude { get; set; }
    public string? DeliveryAccuracy { get; set; }
    public string? DeliveryAltitude { get; set; }
    public string? DeliverySpeed { get; set; }
    public string? DeliveryDirection { get; set; }
    public string? DeliveryDistance { get; set; }
    public string? DeliveryDuration { get; set; }
    public string? DeliveryRoute { get; set; }
    public string? DeliveryStops { get; set; }
    public string? DeliveryDelays { get; set; }
    public string? DeliveryIssues { get; set; }
    public string? DeliveryResolutions { get; set; }
    public string? DeliveryFeedback { get; set; }
    public string? DeliveryRating { get; set; }
    public string? DeliveryComments { get; set; }
    public string? DeliveryTags { get; set; }
    public string? DeliveryCategory { get; set; }
    public string? DeliverySubCategory { get; set; }
    public string? DeliveryMetadata { get; set; }
    public string? DeliverySettings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
}