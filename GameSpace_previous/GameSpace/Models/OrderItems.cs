using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class OrderItems
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductType { get; set; }
    public string? ProductSku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? ShippingCost { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
    public string? Specifications { get; set; }
    public string? Variants { get; set; }
    public string? Customizations { get; set; }
    public string? GiftMessage { get; set; }
    public bool? IsGift { get; set; }
    public string? GiftWrapType { get; set; }
    public decimal? GiftWrapCost { get; set; }
    public string? WarrantyInfo { get; set; }
    public string? ReturnPolicy { get; set; }
    public bool? IsReturnable { get; set; }
    public DateTime? ReturnDeadline { get; set; }
    public string? SupplierInfo { get; set; }
    public string? InventoryLocation { get; set; }
    public string? TrackingInfo { get; set; }
    public string? DeliveryInstructions { get; set; }
    public string? SpecialRequirements { get; set; }
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? Material { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Barcode { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? Description { get; set; }
    public string? Features { get; set; }
    public string? Requirements { get; set; }
    public decimal? Rating { get; set; }
    public int? RatingCount { get; set; }
    public int? ViewCount { get; set; }
    public int? PurchaseCount { get; set; }
    public string? Reviews { get; set; }
    public string? Qa { get; set; }
    public string? Faq { get; set; }
    public string? Manual { get; set; }
    public string? SupportInfo { get; set; }
    public string? RelatedProducts { get; set; }
    public string? CrossSells { get; set; }
    public string? UpSells { get; set; }
    public string? Bundles { get; set; }
    public string? Promotions { get; set; }
    public string? Coupons { get; set; }
    public string? LoyaltyPoints { get; set; }
    public string? Rewards { get; set; }
    public string? Settings { get; set; }
    public string? Metadata { get; set; }
}