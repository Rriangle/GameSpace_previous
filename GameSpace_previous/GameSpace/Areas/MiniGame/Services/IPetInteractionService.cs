using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 寵物互動服務介面 - 處理寵物餵食、陪玩、清潔等互動邏輯
    /// 對應 database.sql Pet 資料表操作
    /// </summary>
    public interface IPetInteractionService
    {
        /// <summary>
        /// 餵食寵物 - 增加飢餓度並消耗使用者積分
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>餵食結果</returns>
        Task<PetInteractionResultViewModel> FeedPetAsync(int petId, int userId);

        /// <summary>
        /// 與寵物玩耍 - 增加心情值並消耗使用者積分
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>陪玩結果</returns>
        Task<PetInteractionResultViewModel> PlayWithPetAsync(int petId, int userId);

        /// <summary>
        /// 清潔寵物 - 增加清潔度並消耗使用者積分
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>清潔結果</returns>
        Task<PetInteractionResultViewModel> CleanPetAsync(int petId, int userId);

        /// <summary>
        /// 更新寵物外觀 - 變更皮膚或背景顏色
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <param name="skinColor">新皮膚顏色</param>
        /// <param name="backgroundColor">新背景顏色</param>
        /// <returns>外觀更新結果</returns>
        Task<PetInteractionResultViewModel> UpdatePetAppearanceAsync(int petId, int userId, string? skinColor = null, string? backgroundColor = null);

        /// <summary>
        /// 取得寵物完整狀態資料
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>寵物狀態顯示模型</returns>
        Task<PetStatusDisplayViewModel> GetPetStatusAsync(int petId, int userId);

        /// <summary>
        /// 檢查寵物是否屬於指定使用者
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="userId">使用者編號</param>
        /// <returns>是否為該使用者的寵物</returns>
        Task<bool> ValidatePetOwnershipAsync(int petId, int userId);
    }

    /// <summary>
    /// 寵物互動結果視圖模型 - 包含互動後的狀態變化資訊
    /// </summary>
    public class PetInteractionResultViewModel
    {
        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 結果訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 互動類型（餵食、陪玩、清潔、外觀變更）
        /// </summary>
        public string InteractionType { get; set; } = string.Empty;

        /// <summary>
        /// 消耗的積分
        /// </summary>
        public int PointsCost { get; set; }

        /// <summary>
        /// 寵物屬性變化資訊
        /// </summary>
        public PetAttributeChangesViewModel AttributeChanges { get; set; } = new PetAttributeChangesViewModel();

        /// <summary>
        /// 是否觸發升級
        /// </summary>
        public bool LevelUpTriggered { get; set; }

        /// <summary>
        /// 升級獎勵資訊
        /// </summary>
        public PetLevelUpRewardViewModel? LevelUpReward { get; set; }

        /// <summary>
        /// 互動後寵物總體狀態評分
        /// </summary>
        public int NewOverallScore { get; set; }

        /// <summary>
        /// 使用者剩餘積分
        /// </summary>
        public int RemainingUserPoints { get; set; }
    }

    /// <summary>
    /// 寵物屬性變化視圖模型
    /// </summary>
    public class PetAttributeChangesViewModel
    {
        /// <summary>
        /// 飢餓度變化
        /// </summary>
        public int HungerDelta { get; set; }

        /// <summary>
        /// 心情值變化
        /// </summary>
        public int MoodDelta { get; set; }

        /// <summary>
        /// 體力值變化
        /// </summary>
        public int StaminaDelta { get; set; }

        /// <summary>
        /// 清潔度變化
        /// </summary>
        public int CleanlinessDelta { get; set; }

        /// <summary>
        /// 健康值變化
        /// </summary>
        public int HealthDelta { get; set; }

        /// <summary>
        /// 經驗值變化
        /// </summary>
        public int ExperienceDelta { get; set; }
    }

    /// <summary>
    /// 寵物升級獎勵視圖模型
    /// </summary>
    public class PetLevelUpRewardViewModel
    {
        /// <summary>
        /// 新等級
        /// </summary>
        public int NewLevel { get; set; }

        /// <summary>
        /// 獲得積分獎勵
        /// </summary>
        public int PointsReward { get; set; }

        /// <summary>
        /// 升級時間
        /// </summary>
        public DateTime LevelUpTime { get; set; }

        /// <summary>
        /// 升級訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}