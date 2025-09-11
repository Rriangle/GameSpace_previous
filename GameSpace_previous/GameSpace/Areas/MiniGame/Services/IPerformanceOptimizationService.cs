using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 性能優化服務介面 - 處理 MiniGame 區域的熱點查詢優化
    /// 專注於錢包聚合、簽到寫入、寵物讀取等高頻操作的性能提升
    /// </summary>
    public interface IPerformanceOptimizationService
    {
        /// <summary>
        /// 優化錢包聚合查詢 - 批次查詢減少資料庫往返次數
        /// 熱點：User_Wallet + Coupon + EVoucher + WalletHistory 聚合查詢
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>優化後的錢包總覽資料</returns>
        Task<WalletOverviewDisplayViewModel> GetOptimizedWalletOverviewAsync(int userId);

        /// <summary>
        /// 優化簽到寫入路徑 - 批次事務處理
        /// 熱點：UserSignInStats INSERT + User_Wallet UPDATE + WalletHistory INSERT
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>優化後的簽到結果</returns>
        Task<SignInResultViewModel> ProcessOptimizedSignInAsync(int userId);

        /// <summary>
        /// 優化寵物狀態讀取 - 單一查詢獲取完整寵物資料
        /// 熱點：Pet 資料表的頻繁查詢優化
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>優化後的寵物狀態資料</returns>
        Task<PetStatusDisplayViewModel> GetOptimizedPetStatusAsync(int petId, int userId);

        /// <summary>
        /// 優化遊戲大廳資料載入 - 並行查詢提升載入速度
        /// 熱點：MiniGame + User_Wallet + Pet 多表並行查詢
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>優化後的遊戲大廳資料</returns>
        Task<GameHallDisplayViewModel> GetOptimizedGameHallDataAsync(int userId);

        /// <summary>
        /// 快取預熱 - 預載入常用資料到記憶體快取
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>預熱操作結果</returns>
        Task<bool> WarmupUserCacheAsync(int userId);

        /// <summary>
        /// 批次積分更新 - 優化多筆積分異動的寫入性能
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <param name="pointsChanges">積分變動記錄列表</param>
        /// <returns>批次更新結果</returns>
        Task<bool> BatchUpdateUserPointsAsync(int userId, List<PointsChangeRecord> pointsChanges);

        /// <summary>
        /// 取得查詢性能統計 - 監控各項操作的執行時間
        /// </summary>
        /// <returns>性能統計資料</returns>
        Task<PerformanceStatsViewModel> GetPerformanceStatsAsync();
    }

    /// <summary>
    /// 積分變動記錄 - 批次更新用
    /// </summary>
    public class PointsChangeRecord
    {
        /// <summary>
        /// 變動類型
        /// </summary>
        public string ChangeType { get; set; } = string.Empty;

        /// <summary>
        /// 積分變動數量
        /// </summary>
        public int PointsChanged { get; set; }

        /// <summary>
        /// 項目代碼
        /// </summary>
        public string? ItemCode { get; set; }

        /// <summary>
        /// 變動描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 變動時間
        /// </summary>
        public DateTime ChangeTime { get; set; }
    }

    /// <summary>
    /// 性能統計視圖模型
    /// </summary>
    public class PerformanceStatsViewModel
    {
        /// <summary>
        /// 錢包查詢平均耗時（毫秒）
        /// </summary>
        public double WalletQueryAverageMs { get; set; }

        /// <summary>
        /// 簽到寫入平均耗時（毫秒）
        /// </summary>
        public double SignInWriteAverageMs { get; set; }

        /// <summary>
        /// 寵物查詢平均耗時（毫秒）
        /// </summary>
        public double PetQueryAverageMs { get; set; }

        /// <summary>
        /// 遊戲大廳載入平均耗時（毫秒）
        /// </summary>
        public double GameHallLoadAverageMs { get; set; }

        /// <summary>
        /// 快取命中率
        /// </summary>
        public double CacheHitRate { get; set; }

        /// <summary>
        /// 資料庫連線池使用率
        /// </summary>
        public double DbConnectionPoolUsage { get; set; }

        /// <summary>
        /// 記憶體使用量（MB）
        /// </summary>
        public double MemoryUsageMB { get; set; }

        /// <summary>
        /// 統計時間範圍
        /// </summary>
        public DateTime StatsPeriodStart { get; set; }

        /// <summary>
        /// 統計結束時間
        /// </summary>
        public DateTime StatsPeriodEnd { get; set; }
    }
}