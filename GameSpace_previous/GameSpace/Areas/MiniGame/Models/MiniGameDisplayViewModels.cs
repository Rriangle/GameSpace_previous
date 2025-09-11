using System.ComponentModel.DataAnnotations;

namespace GameSpace.Areas.MiniGame.Models
{
    /// <summary>
    /// 錢包總覽顯示視圖模型 - 聚合錢包相關資訊用於 UI 顯示
    /// </summary>
    public class WalletOverviewDisplayViewModel
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        
        /// <summary>
        /// 目前積分
        /// </summary>
        public int CurrentPoints { get; set; }
        
        /// <summary>
        /// 可用優惠券數量
        /// </summary>
        public int AvailableCouponsCount { get; set; }
        
        /// <summary>
        /// 已使用優惠券數量
        /// </summary>
        public int UsedCouponsCount { get; set; }
        
        /// <summary>
        /// 可用電子禮券數量
        /// </summary>
        public int AvailableEVouchersCount { get; set; }
        
        /// <summary>
        /// 已使用電子禮券數量
        /// </summary>
        public int UsedEVouchersCount { get; set; }
        
        /// <summary>
        /// 本月積分收入
        /// </summary>
        public int MonthlyPointsEarned { get; set; }
        
        /// <summary>
        /// 本月積分支出
        /// </summary>
        public int MonthlyPointsSpent { get; set; }
        
        /// <summary>
        /// 最近交易記錄（前5筆）
        /// </summary>
        public List<WalletHistoryViewModel> RecentTransactions { get; set; } = new List<WalletHistoryViewModel>();
        
        /// <summary>
        /// 可用優惠券列表（前3張）
        /// </summary>
        public List<CouponViewModel> AvailableCoupons { get; set; } = new List<CouponViewModel>();
        
        /// <summary>
        /// 可用電子禮券列表（前3張）
        /// </summary>
        public List<EVoucherViewModel> AvailableEVouchers { get; set; } = new List<EVoucherViewModel>();
    }

    /// <summary>
    /// 簽到統計顯示視圖模型 - 聚合簽到相關資訊用於 UI 顯示
    /// </summary>
    public class SignInStatsDisplayViewModel
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 今日是否已簽到
        /// </summary>
        public bool HasSignedToday { get; set; }
        
        /// <summary>
        /// 連續簽到天數
        /// </summary>
        public int ConsecutiveDays { get; set; }
        
        /// <summary>
        /// 本月簽到天數
        /// </summary>
        public int MonthlySignInDays { get; set; }
        
        /// <summary>
        /// 總簽到天數
        /// </summary>
        public int TotalSignInDays { get; set; }
        
        /// <summary>
        /// 今日簽到獎勵積分
        /// </summary>
        public int TodayPointsReward { get; set; }
        
        /// <summary>
        /// 今日簽到獎勵經驗值
        /// </summary>
        public int TodayExpReward { get; set; }
        
        /// <summary>
        /// 本月獲得總積分
        /// </summary>
        public int MonthlyPointsEarned { get; set; }
        
        /// <summary>
        /// 本月獲得總經驗值
        /// </summary>
        public int MonthlyExpEarned { get; set; }
        
        /// <summary>
        /// 最近簽到記錄（前10筆）
        /// </summary>
        public List<UserSignInStatsViewModel> RecentSignInStats { get; set; } = new List<UserSignInStatsViewModel>();
        
        /// <summary>
        /// 本月簽到日曆資料（每日簽到狀態）
        /// </summary>
        public Dictionary<int, bool> MonthlyCalendar { get; set; } = new Dictionary<int, bool>();
    }

    /// <summary>
    /// 寵物狀態顯示視圖模型 - 聚合寵物相關資訊用於 UI 顯示
    /// </summary>
    public class PetStatusDisplayViewModel
    {
        /// <summary>
        /// 寵物基本資訊
        /// </summary>
        public PetViewModel Pet { get; set; } = new PetViewModel();
        
        /// <summary>
        /// 下一等級所需經驗值
        /// </summary>
        public int NextLevelExpRequired { get; set; }
        
        /// <summary>
        /// 目前等級經驗值進度百分比
        /// </summary>
        public double ExpProgressPercentage { get; set; }
        
        /// <summary>
        /// 飢餓度狀態描述
        /// </summary>
        public string HungerStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// 心情狀態描述
        /// </summary>
        public string MoodStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// 整體健康狀態評分 (0-100)
        /// </summary>
        public int OverallHealthScore { get; set; }
        
        /// <summary>
        /// 是否需要照顧（任一屬性低於30）
        /// </summary>
        public bool NeedsCare { get; set; }
        
        /// <summary>
        /// 建議的照顧行動
        /// </summary>
        public List<string> SuggestedCareActions { get; set; } = new List<string>();
        
        /// <summary>
        /// 最近活動記錄（餵食、陪玩等）
        /// </summary>
        public List<PetActivityLogViewModel> RecentActivities { get; set; } = new List<PetActivityLogViewModel>();
        
        /// <summary>
        /// 升級歷史記錄
        /// </summary>
        public List<PetLevelUpHistoryViewModel> LevelUpHistory { get; set; } = new List<PetLevelUpHistoryViewModel>();
    }

    /// <summary>
    /// 遊戲大廳顯示視圖模型 - 聚合遊戲相關資訊用於 UI 顯示
    /// </summary>
    public class GameHallDisplayViewModel
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 今日遊戲次數
        /// </summary>
        public int TodayGameCount { get; set; }
        
        /// <summary>
        /// 今日獲得積分
        /// </summary>
        public int TodayPointsEarned { get; set; }
        
        /// <summary>
        /// 歷史最高分數
        /// </summary>
        public int HighestScore { get; set; }
        
        /// <summary>
        /// 目前遊戲等級
        /// </summary>
        public int CurrentGameLevel { get; set; }
        
        /// <summary>
        /// 可用遊戲列表
        /// </summary>
        public List<AvailableGameViewModel> AvailableGames { get; set; } = new List<AvailableGameViewModel>();
        
        /// <summary>
        /// 最近遊戲記錄（前5筆）
        /// </summary>
        public List<MiniGameViewModel> RecentGameHistory { get; set; } = new List<MiniGameViewModel>();
        
        /// <summary>
        /// 本週遊戲統計
        /// </summary>
        public WeeklyGameStatsViewModel WeeklyStats { get; set; } = new WeeklyGameStatsViewModel();
    }

    /// <summary>
    /// 可用遊戲視圖模型 - 遊戲基本資訊
    /// </summary>
    public class AvailableGameViewModel
    {
        /// <summary>
        /// 遊戲編號
        /// </summary>
        public int GameId { get; set; }
        
        /// <summary>
        /// 遊戲名稱
        /// </summary>
        public string GameName { get; set; } = string.Empty;
        
        /// <summary>
        /// 遊戲描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 積分獎勵
        /// </summary>
        public int PointsReward { get; set; }
        
        /// <summary>
        /// 難度等級 (1-5)
        /// </summary>
        public int DifficultyLevel { get; set; }
        
        /// <summary>
        /// 遊戲圖示 CSS 類別
        /// </summary>
        public string IconClass { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// 本週遊戲統計視圖模型
    /// </summary>
    public class WeeklyGameStatsViewModel
    {
        /// <summary>
        /// 本週遊戲次數
        /// </summary>
        public int WeeklyGameCount { get; set; }
        
        /// <summary>
        /// 本週獲得積分
        /// </summary>
        public int WeeklyPointsEarned { get; set; }
        
        /// <summary>
        /// 本週最高分
        /// </summary>
        public int WeeklyHighScore { get; set; }
        
        /// <summary>
        /// 平均每日遊戲時間（分鐘）
        /// </summary>
        public int AveragePlayTimeMinutes { get; set; }
    }

    /// <summary>
    /// 寵物活動記錄視圖模型（用於顯示最近活動）
    /// </summary>
    public class PetActivityLogViewModel
    {
        /// <summary>
        /// 活動類型（餵食、陪玩、清潔等）
        /// </summary>
        public string ActivityType { get; set; } = string.Empty;
        
        /// <summary>
        /// 活動描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 活動時間
        /// </summary>
        public DateTime ActivityTime { get; set; }
        
        /// <summary>
        /// 活動圖示 CSS 類別
        /// </summary>
        public string IconClass { get; set; } = string.Empty;
    }

    /// <summary>
    /// 寵物升級歷史視圖模型
    /// </summary>
    public class PetLevelUpHistoryViewModel
    {
        /// <summary>
        /// 升級到的等級
        /// </summary>
        public int Level { get; set; }
        
        /// <summary>
        /// 升級時間
        /// </summary>
        public DateTime LevelUpTime { get; set; }
        
        /// <summary>
        /// 獲得的獎勵積分
        /// </summary>
        public int PointsReward { get; set; }
    }
}