using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class UserSignInStats
{
    public int StatId { get; set; }
    public int UserId { get; set; }
    public DateTime SignInDate { get; set; }
    public string? SignInIp { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceType { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Timezone { get; set; }
    public string? Language { get; set; }
    public string? Referrer { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public string? UtmTerm { get; set; }
    public string? UtmContent { get; set; }
    public string? SessionId { get; set; }
    public DateTime? SessionStart { get; set; }
    public DateTime? SessionEnd { get; set; }
    public int? SessionDuration { get; set; }
    public int? PageViews { get; set; }
    public int? Actions { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }
}