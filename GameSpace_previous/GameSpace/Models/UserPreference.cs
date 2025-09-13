using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 用戶偏好設定資料表
    /// </summary>
    public partial class UserPreference
    {
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        public string PreferenceKey { get; set; } = null!;
        public string PreferenceValue { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Users User { get; set; } = null!;
    }
}