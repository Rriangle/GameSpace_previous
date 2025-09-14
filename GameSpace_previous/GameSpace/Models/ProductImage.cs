using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class ProductImage
{
    public int ImageId { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? ImageType { get; set; }
    public string? ImageFormat { get; set; }
    public int? ImageSize { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsPrimary { get; set; }
    public bool? IsActive { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Metadata { get; set; }
    public string? Settings { get; set; }
}