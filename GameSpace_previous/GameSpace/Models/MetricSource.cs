using System;
using System.Collections.Generic;

namespace GameSpace.Models
{
    /// <summary>
    /// 指標來源資料表
    /// </summary>
    public partial class MetricSource
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; } = null!;
        public string SourceType { get; set; } = null!; // API, Manual, Internal
        public string? ApiEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UpdateFrequency { get; set; } // 更新頻率（分鐘）
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();
    }
}