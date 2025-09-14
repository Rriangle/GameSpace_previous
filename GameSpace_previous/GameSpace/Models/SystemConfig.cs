using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 系統配置資料表
    /// </summary>
    public partial class SystemConfig
    {
        public int ConfigId { get; set; }
        public string ConfigKey { get; set; } = null!;
        public string ConfigValue { get; set; } = null!;
        public string? Description { get; set; }
        public string ConfigType { get; set; } = "String"; // String, Int, Bool, Decimal
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}