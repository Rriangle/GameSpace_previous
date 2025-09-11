using System.ComponentModel.DataAnnotations;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶讀取模型
    /// </summary>
    public class UserReadModel
    {
        public int User_ID { get; set; }
        public string User_name { get; set; } = string.Empty;
        public string User_Account { get; set; } = string.Empty;
        public bool User_EmailConfirmed { get; set; }
        public bool User_PhoneNumberConfirmed { get; set; }
        public bool User_TwoFactorEnabled { get; set; }
        public int User_AccessFailedCount { get; set; }
        public bool User_LockoutEnabled { get; set; }
        public DateTime? User_LockoutEnd { get; set; }
    }

    /// <summary>
    /// 用戶介紹讀取模型
    /// </summary>
    public class UserIntroduceReadModel
    {
        public int User_ID { get; set; }
        public string User_NickName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime Create_Account { get; set; }
        public byte[]? User_Picture { get; set; }
        public string? User_Introduce { get; set; }
    }

    /// <summary>
    /// 用戶權限讀取模型
    /// </summary>
    public class UserRightsReadModel
    {
        public int User_Id { get; set; }
        public bool User_Status { get; set; }
        public bool ShoppingPermission { get; set; }
        public bool MessagePermission { get; set; }
        public bool SalesAuthority { get; set; }
    }

    /// <summary>
    /// 寵物讀取模型
    /// </summary>
    public class PetReadModel
    {
        public int PetID { get; set; }
        public int UserID { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string PetType { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int Happiness { get; set; }
        public int Hunger { get; set; }
        public int Energy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastFed { get; set; }
        public DateTime? LastPlayed { get; set; }
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// 積分讀取模型
    /// </summary>
    public class PointsReadModel
    {
        public int PointsID { get; set; }
        public int UserID { get; set; }
        public int CurrentPoints { get; set; }
        public int TotalEarned { get; set; }
        public int TotalSpent { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// 優惠券讀取模型
    /// </summary>
    public class CouponReadModel
    {
        public int CouponID { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public int CouponTypeID { get; set; }
        public int UserID { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AcquiredTime { get; set; }
        public DateTime? UsedTime { get; set; }
        public int? UsedInOrderID { get; set; }
    }

    /// <summary>
    /// 電子禮券讀取模型
    /// </summary>
    public class EVoucherReadModel
    {
        public int EVoucherID { get; set; }
        public string EVoucherCode { get; set; } = string.Empty;
        public int EVoucherTypeID { get; set; }
        public int UserID { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AcquiredTime { get; set; }
        public DateTime? UsedTime { get; set; }
    }

    /// <summary>
    /// 錢包歷史讀取模型
    /// </summary>
    public class WalletHistoryReadModel
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public int PointsChanged { get; set; }
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public DateTime ChangeTime { get; set; }
    }
}
