using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class OtherProductDetail
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductType { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? CurrencyCode { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UnpublishedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? Specifications { get; set; }
    public string? Features { get; set; }
    public string? Requirements { get; set; }
    public decimal? Rating { get; set; }
    public int? RatingCount { get; set; }
    public int? ViewCount { get; set; }
    public int? PurchaseCount { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public string? Material { get; set; }
    public string? Color { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Barcode { get; set; }
    public string? Sku { get; set; }
    public int? Stock { get; set; }
    public int? MinStock { get; set; }
    public string? Notes { get; set; }
}