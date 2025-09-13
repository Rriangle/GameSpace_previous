using System;
using System.Collections.Generic;

namespace GameSpace.Models;

public partial class Users
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Timezone { get; set; }
    public string? Language { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsEmailVerified { get; set; }
    public bool? IsPhoneVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int? LoginCount { get; set; }
    public int? FailedLoginCount { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool? TwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }
    public string? RecoveryCodes { get; set; }
    public string? Preferences { get; set; }
    public string? Settings { get; set; }
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