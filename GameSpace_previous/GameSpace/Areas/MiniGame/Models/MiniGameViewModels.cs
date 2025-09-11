using System.ComponentModel.DataAnnotations;

namespace GameSpace.Areas.MiniGame.Models
{
    /// <summary>
    /// 使用者錢包視圖模型 - 對應 database.sql User_Wallet 資料表
    /// </summary>
    public class UserWalletViewModel
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int User_Id { get; set; }
        
        /// <summary>
        /// 使用者積分
        /// </summary>
        public int User_Point { get; set; }
    }

    /// <summary>
    /// 使用者簽到統計視圖模型 - 對應 database.sql UserSignInStats 資料表
    /// </summary>
    public class UserSignInStatsViewModel
    {
        /// <summary>
        /// 紀錄編號
        /// </summary>
        public int LogID { get; set; }
        
        /// <summary>
        /// 簽到時間
        /// </summary>
        public DateTime SignTime { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 獲得積分
        /// </summary>
        public int PointsGained { get; set; }
        
        /// <summary>
        /// 積分獲得時間
        /// </summary>
        public DateTime PointsGainedTime { get; set; }
        
        /// <summary>
        /// 獲得經驗值
        /// </summary>
        public int ExpGained { get; set; }
        
        /// <summary>
        /// 經驗值獲得時間
        /// </summary>
        public DateTime ExpGainedTime { get; set; }
        
        /// <summary>
        /// 獲得優惠券代碼
        /// </summary>
        public string CouponGained { get; set; } = string.Empty;
        
        /// <summary>
        /// 優惠券獲得時間
        /// </summary>
        public DateTime CouponGainedTime { get; set; }
    }

    /// <summary>
    /// 寵物視圖模型 - 對應 database.sql Pet 資料表
    /// </summary>
    public class PetViewModel
    {
        /// <summary>
        /// 寵物編號
        /// </summary>
        public int PetID { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 寵物名稱
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PetName { get; set; } = string.Empty;
        
        /// <summary>
        /// 等級
        /// </summary>
        public int Level { get; set; }
        
        /// <summary>
        /// 升級時間
        /// </summary>
        public DateTime LevelUpTime { get; set; }
        
        /// <summary>
        /// 經驗值
        /// </summary>
        public int Experience { get; set; }
        
        /// <summary>
        /// 飢餓度 (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Hunger { get; set; }
        
        /// <summary>
        /// 心情值 (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Mood { get; set; }
        
        /// <summary>
        /// 體力值 (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Stamina { get; set; }
        
        /// <summary>
        /// 清潔度 (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Cleanliness { get; set; }
        
        /// <summary>
        /// 健康值 (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Health { get; set; }
        
        /// <summary>
        /// 皮膚顏色
        /// </summary>
        [StringLength(10)]
        public string SkinColor { get; set; } = string.Empty;
        
        /// <summary>
        /// 皮膚顏色變更時間
        /// </summary>
        public DateTime SkinColorChangedTime { get; set; }
        
        /// <summary>
        /// 背景顏色
        /// </summary>
        [StringLength(50)]
        public string BackgroundColor { get; set; } = string.Empty;
        
        /// <summary>
        /// 背景顏色變更時間
        /// </summary>
        public DateTime BackgroundColorChangedTime { get; set; }
        
        /// <summary>
        /// 皮膚顏色變更消耗積分
        /// </summary>
        public int PointsChanged_SkinColor { get; set; }
        
        /// <summary>
        /// 背景顏色變更消耗積分
        /// </summary>
        public int PointsChanged_BackgroundColor { get; set; }
        
        /// <summary>
        /// 升級獲得積分
        /// </summary>
        public int PointsGained_LevelUp { get; set; }
        
        /// <summary>
        /// 升級積分獲得時間
        /// </summary>
        public DateTime PointsGainedTime_LevelUp { get; set; }
    }

    /// <summary>
    /// 小遊戲視圖模型 - 對應 database.sql MiniGame 資料表
    /// </summary>
    public class MiniGameViewModel
    {
        /// <summary>
        /// 遊玩紀錄編號
        /// </summary>
        public int PlayID { get; set; }
        
        /// <summary>
        /// 使用者編號
        /// </summary>
        public int UserID { get; set; }
        
        /// <summary>
        /// 寵物編號
        /// </summary>
        public int PetID { get; set; }
        
        /// <summary>
        /// 遊戲等級
        /// </summary>
        public int Level { get; set; }
        
        /// <summary>
        /// 怪物數量
        /// </summary>
        public int MonsterCount { get; set; }
        
        /// <summary>
        /// 速度倍數
        /// </summary>
        public decimal SpeedMultiplier { get; set; }
        
        /// <summary>
        /// 遊戲結果
        /// </summary>
        [StringLength(10)]
        public string Result { get; set; } = string.Empty;
        
        /// <summary>
        /// 獲得經驗值
        /// </summary>
        public int ExpGained { get; set; }
        
        /// <summary>
        /// 經驗值獲得時間
        /// </summary>
        public DateTime ExpGainedTime { get; set; }
        
        /// <summary>
        /// 獲得積分
        /// </summary>
        public int PointsGained { get; set; }
        
        /// <summary>
        /// 積分獲得時間
        /// </summary>
        public DateTime PointsGainedTime { get; set; }
        
        /// <summary>
        /// 獲得優惠券代碼
        /// </summary>
        [StringLength(50)]
        public string CouponGained { get; set; } = string.Empty;
        
        /// <summary>
        /// 優惠券獲得時間
        /// </summary>
        public DateTime CouponGainedTime { get; set; }
        
        /// <summary>
        /// 寵物飢餓度變化
        /// </summary>
        public int HungerDelta { get; set; }
        
        /// <summary>
        /// 寵物心情變化
        /// </summary>
        public int MoodDelta { get; set; }
        
        /// <summary>
        /// 寵物體力變化
        /// </summary>
        public int StaminaDelta { get; set; }
        
        /// <summary>
        /// 寵物清潔度變化
        /// </summary>
        public int CleanlinessDelta { get; set; }
        
        /// <summary>
        /// 遊戲開始時間
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// 遊戲結束時間
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// 是否中止遊戲
        /// </summary>
        public bool Aborted { get; set; }
    }
}