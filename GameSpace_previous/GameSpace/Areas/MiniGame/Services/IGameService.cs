using GameSpace.Areas.MiniGame.Models;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 遊戲服務介面 - 處理遊戲結果提交、獎勵計算與記錄
    /// 對應 database.sql MiniGame、User_Wallet、Pet 資料表操作
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// 提交遊戲結果 - 記錄遊戲過程並計算獎勵
        /// </summary>
        /// <param name="gameResult">遊戲結果資料</param>
        /// <param name="userId">使用者編號</param>
        /// <param name="petId">參與遊戲的寵物編號</param>
        /// <returns>遊戲結果處理結果</returns>
        Task<GameSubmissionResultViewModel> SubmitGameResultAsync(GameResultSubmissionModel gameResult, int userId, int petId);

        /// <summary>
        /// 取得遊戲大廳顯示資料
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <returns>遊戲大廳顯示模型</returns>
        Task<GameHallDisplayViewModel> GetGameHallDataAsync(int userId);

        /// <summary>
        /// 取得可用遊戲列表
        /// </summary>
        /// <returns>可用遊戲列表</returns>
        Task<List<AvailableGameViewModel>> GetAvailableGamesAsync();

        /// <summary>
        /// 取得使用者遊戲歷史記錄
        /// </summary>
        /// <param name="userId">使用者編號</param>
        /// <param name="limit">記錄數量限制</param>
        /// <returns>遊戲歷史記錄</returns>
        Task<List<MiniGameViewModel>> GetUserGameHistoryAsync(int userId, int limit = 10);

        /// <summary>
        /// 檢查遊戲是否可玩（寵物狀態檢查）
        /// </summary>
        /// <param name="petId">寵物編號</param>
        /// <param name="gameId">遊戲編號</param>
        /// <returns>遊戲可玩性檢查結果</returns>
        Task<GameEligibilityViewModel> CheckGameEligibilityAsync(int petId, int gameId);
    }

    /// <summary>
    /// 遊戲結果提交模型 - 對應前端提交的遊戲資料
    /// </summary>
    public class GameResultSubmissionModel
    {
        /// <summary>
        /// 遊戲編號
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// 遊戲分數
        /// </summary>
        public int Score { get; set; }

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
        public decimal SpeedMultiplier { get; set; } = 1.0m;

        /// <summary>
        /// 是否完成遊戲
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 是否中止遊戲
        /// </summary>
        public bool IsAborted { get; set; }

        /// <summary>
        /// 遊戲持續時間（秒）
        /// </summary>
        public int DurationSeconds { get; set; }

        /// <summary>
        /// 遊戲開始時間
        /// </summary>
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// 遊戲提交結果視圖模型
    /// </summary>
    public class GameSubmissionResultViewModel
    {
        /// <summary>
        /// 是否提交成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 結果訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 遊戲記錄編號
        /// </summary>
        public int PlayRecordId { get; set; }

        /// <summary>
        /// 獲得積分
        /// </summary>
        public int PointsGained { get; set; }

        /// <summary>
        /// 獲得經驗值
        /// </summary>
        public int ExpGained { get; set; }

        /// <summary>
        /// 獎勵優惠券代碼
        /// </summary>
        public string? BonusCouponCode { get; set; }

        /// <summary>
        /// 寵物屬性變化
        /// </summary>
        public PetAttributeChangesViewModel PetChanges { get; set; } = new PetAttributeChangesViewModel();

        /// <summary>
        /// 是否觸發寵物升級
        /// </summary>
        public bool PetLevelUpTriggered { get; set; }

        /// <summary>
        /// 寵物升級獎勵
        /// </summary>
        public PetLevelUpRewardViewModel? PetLevelUpReward { get; set; }

        /// <summary>
        /// 是否達到新的個人最高分
        /// </summary>
        public bool NewHighScore { get; set; }

        /// <summary>
        /// 遊戲評級（S/A/B/C/D）
        /// </summary>
        public string GameRank { get; set; } = string.Empty;

        /// <summary>
        /// 更新後的使用者總積分
        /// </summary>
        public int TotalUserPoints { get; set; }

        /// <summary>
        /// 遊戲完成時間
        /// </summary>
        public DateTime CompletedAt { get; set; }
    }

    /// <summary>
    /// 遊戲可玩性檢查視圖模型
    /// </summary>
    public class GameEligibilityViewModel
    {
        /// <summary>
        /// 是否可以開始遊戲
        /// </summary>
        public bool CanPlay { get; set; }

        /// <summary>
        /// 不可玩的原因
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// 建議的處理方式
        /// </summary>
        public List<string> Suggestions { get; set; } = new List<string>();

        /// <summary>
        /// 寵物目前狀態評分
        /// </summary>
        public int PetConditionScore { get; set; }

        /// <summary>
        /// 最低要求狀態評分
        /// </summary>
        public int MinRequiredScore { get; set; } = 30;
    }
}