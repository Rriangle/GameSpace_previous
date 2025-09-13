using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class UserTokens
{
    public int TokenId { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public string? Purpose { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? UsedIp { get; set; }
    public string? UsedUserAgent { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public string? Notes { get; set; }
    public string? Metadata { get; set; }
    public string? Settings { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
}