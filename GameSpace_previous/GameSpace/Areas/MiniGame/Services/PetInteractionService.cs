using GameSpace.Areas.MiniGame.Models;
using Microsoft.Extensions.Logging;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 寵物互動服務實作 - 處理寵物餵食、陪玩、清潔等互動邏輯
    /// 對應 database.sql Pet 與 User_Wallet 資料表操作
    /// </summary>
    public class PetInteractionService : IPetInteractionService
    {
        private readonly ILogger<PetInteractionService> _logger;
        
        // 互動消耗積分設定
        private const int FEED_COST = 5;
        private const int PLAY_COST = 3;
        private const int CLEAN_COST = 8;
        private const int SKIN_COLOR_COST = 50;
        private const int BACKGROUND_COLOR_COST = 30;

        public PetInteractionService(ILogger<PetInteractionService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 餵食寵物 - 增加飢餓度並消耗使用者積分
        /// 對應 Pet 資料表的 Hunger 欄位更新
        /// </summary>
        public async Task<PetInteractionResultViewModel> FeedPetAsync(int petId, int userId)
        {
            try
            {
                // 驗證寵物擁有權
                if (!await ValidatePetOwnershipAsync(petId, userId))
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = "無法操作此寵物",
                        InteractionType = "餵食"
                    };
                }

                // 檢查使用者積分是否足夠
                var userPoints = await GetUserPointsAsync(userId);
                if (userPoints < FEED_COST)
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = $"積分不足，需要 {FEED_COST} 積分",
                        InteractionType = "餵食",
                        PointsCost = FEED_COST
                    };
                }

                // 取得目前寵物狀態
                var currentPet = await GetPetDataAsync(petId);
                
                // 計算屬性變化 - 餵食主要影響飢餓度，輕微影響健康和心情
                var hungerIncrease = Math.Min(25, 100 - currentPet.Hunger); // 最多加到100
                var healthIncrease = Math.Min(5, 100 - currentPet.Health);
                var moodIncrease = Math.Min(3, 100 - currentPet.Mood);
                var expGain = 2; // 餵食獲得少量經驗

                var attributeChanges = new PetAttributeChangesViewModel
                {
                    HungerDelta = hungerIncrease,
                    HealthDelta = healthIncrease,
                    MoodDelta = moodIncrease,
                    ExperienceDelta = expGain
                };

                // 檢查是否升級
                var newExperience = currentPet.Experience + expGain;
                var expRequiredForNextLevel = GetExpRequiredForLevel(currentPet.Level + 1);
                var levelUpReward = (PetLevelUpRewardViewModel?)null;

                if (newExperience >= expRequiredForNextLevel)
                {
                    var newLevel = currentPet.Level + 1;
                    var pointsReward = newLevel * 10; // 升級獎勵積分
                    
                    levelUpReward = new PetLevelUpRewardViewModel
                    {
                        NewLevel = newLevel,
                        PointsReward = pointsReward,
                        LevelUpTime = DateTime.Now,
                        Message = $"恭喜！{currentPet.PetName} 升級到 Lv.{newLevel}！"
                    };
                }

                // 模擬資料庫更新操作
                await SimulatePetUpdateAsync(petId, attributeChanges, levelUpReward);
                await SimulateUserPointsDeductAsync(userId, FEED_COST);

                _logger.LogInformation("寵物餵食成功 - PetId: {PetId}, UserId: {UserId}, HungerGain: {Hunger}", 
                    petId, userId, hungerIncrease);

                return new PetInteractionResultViewModel
                {
                    Success = true,
                    Message = $"餵食成功！{currentPet.PetName} 的飢餓度增加了 {hungerIncrease}",
                    InteractionType = "餵食",
                    PointsCost = FEED_COST,
                    AttributeChanges = attributeChanges,
                    LevelUpTriggered = levelUpReward != null,
                    LevelUpReward = levelUpReward,
                    NewOverallScore = CalculateOverallScore(currentPet, attributeChanges),
                    RemainingUserPoints = userPoints - FEED_COST + (levelUpReward?.PointsReward ?? 0)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "餵食寵物失敗 - PetId: {PetId}, UserId: {UserId}", petId, userId);
                
                return new PetInteractionResultViewModel
                {
                    Success = false,
                    Message = "餵食失敗，請稍後再試",
                    InteractionType = "餵食"
                };
            }
        }

        /// <summary>
        /// 與寵物玩耍 - 增加心情值並輕微影響其他屬性
        /// </summary>
        public async Task<PetInteractionResultViewModel> PlayWithPetAsync(int petId, int userId)
        {
            try
            {
                if (!await ValidatePetOwnershipAsync(petId, userId))
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = "無法操作此寵物",
                        InteractionType = "陪玩"
                    };
                }

                var userPoints = await GetUserPointsAsync(userId);
                if (userPoints < PLAY_COST)
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = $"積分不足，需要 {PLAY_COST} 積分",
                        InteractionType = "陪玩",
                        PointsCost = PLAY_COST
                    };
                }

                var currentPet = await GetPetDataAsync(petId);
                
                // 計算屬性變化 - 陪玩主要影響心情，消耗體力，增加經驗
                var moodIncrease = Math.Min(20, 100 - currentPet.Mood);
                var staminaDecrease = Math.Min(10, currentPet.Stamina);
                var expGain = 5; // 陪玩獲得較多經驗

                var attributeChanges = new PetAttributeChangesViewModel
                {
                    MoodDelta = moodIncrease,
                    StaminaDelta = -staminaDecrease,
                    ExperienceDelta = expGain
                };

                // 檢查升級邏輯（同餵食）
                var newExperience = currentPet.Experience + expGain;
                var expRequiredForNextLevel = GetExpRequiredForLevel(currentPet.Level + 1);
                var levelUpReward = (PetLevelUpRewardViewModel?)null;

                if (newExperience >= expRequiredForNextLevel)
                {
                    var newLevel = currentPet.Level + 1;
                    var pointsReward = newLevel * 10;
                    
                    levelUpReward = new PetLevelUpRewardViewModel
                    {
                        NewLevel = newLevel,
                        PointsReward = pointsReward,
                        LevelUpTime = DateTime.Now,
                        Message = $"太棒了！{currentPet.PetName} 在遊戲中升級到 Lv.{newLevel}！"
                    };
                }

                await SimulatePetUpdateAsync(petId, attributeChanges, levelUpReward);
                await SimulateUserPointsDeductAsync(userId, PLAY_COST);

                _logger.LogInformation("寵物陪玩成功 - PetId: {PetId}, UserId: {UserId}, MoodGain: {Mood}", 
                    petId, userId, moodIncrease);

                return new PetInteractionResultViewModel
                {
                    Success = true,
                    Message = $"陪玩成功！{currentPet.PetName} 的心情增加了 {moodIncrease}",
                    InteractionType = "陪玩",
                    PointsCost = PLAY_COST,
                    AttributeChanges = attributeChanges,
                    LevelUpTriggered = levelUpReward != null,
                    LevelUpReward = levelUpReward,
                    NewOverallScore = CalculateOverallScore(currentPet, attributeChanges),
                    RemainingUserPoints = userPoints - PLAY_COST + (levelUpReward?.PointsReward ?? 0)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寵物陪玩失敗 - PetId: {PetId}, UserId: {UserId}", petId, userId);
                
                return new PetInteractionResultViewModel
                {
                    Success = false,
                    Message = "陪玩失敗，請稍後再試",
                    InteractionType = "陪玩"
                };
            }
        }

        /// <summary>
        /// 清潔寵物 - 增加清潔度和健康值
        /// </summary>
        public async Task<PetInteractionResultViewModel> CleanPetAsync(int petId, int userId)
        {
            try
            {
                if (!await ValidatePetOwnershipAsync(petId, userId))
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = "無法操作此寵物",
                        InteractionType = "清潔"
                    };
                }

                var userPoints = await GetUserPointsAsync(userId);
                if (userPoints < CLEAN_COST)
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = $"積分不足，需要 {CLEAN_COST} 積分",
                        InteractionType = "清潔",
                        PointsCost = CLEAN_COST
                    };
                }

                var currentPet = await GetPetDataAsync(petId);
                
                // 計算屬性變化 - 清潔主要影響清潔度和健康
                var cleanlinessIncrease = Math.Min(30, 100 - currentPet.Cleanliness);
                var healthIncrease = Math.Min(10, 100 - currentPet.Health);
                var moodIncrease = Math.Min(5, 100 - currentPet.Mood); // 清潔後心情稍微提升

                var attributeChanges = new PetAttributeChangesViewModel
                {
                    CleanlinessDelta = cleanlinessIncrease,
                    HealthDelta = healthIncrease,
                    MoodDelta = moodIncrease
                };

                await SimulatePetUpdateAsync(petId, attributeChanges, null);
                await SimulateUserPointsDeductAsync(userId, CLEAN_COST);

                _logger.LogInformation("寵物清潔成功 - PetId: {PetId}, UserId: {UserId}, CleanlinessGain: {Cleanliness}", 
                    petId, userId, cleanlinessIncrease);

                return new PetInteractionResultViewModel
                {
                    Success = true,
                    Message = $"清潔成功！{currentPet.PetName} 變得乾乾淨淨，清潔度增加了 {cleanlinessIncrease}",
                    InteractionType = "清潔",
                    PointsCost = CLEAN_COST,
                    AttributeChanges = attributeChanges,
                    NewOverallScore = CalculateOverallScore(currentPet, attributeChanges),
                    RemainingUserPoints = userPoints - CLEAN_COST
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寵物清潔失敗 - PetId: {PetId}, UserId: {UserId}", petId, userId);
                
                return new PetInteractionResultViewModel
                {
                    Success = false,
                    Message = "清潔失敗，請稍後再試",
                    InteractionType = "清潔"
                };
            }
        }

        /// <summary>
        /// 更新寵物外觀 - 對應 Pet 資料表的 SkinColor 和 BackgroundColor 欄位
        /// </summary>
        public async Task<PetInteractionResultViewModel> UpdatePetAppearanceAsync(int petId, int userId, string? skinColor = null, string? backgroundColor = null)
        {
            try
            {
                if (!await ValidatePetOwnershipAsync(petId, userId))
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = "無法操作此寵物",
                        InteractionType = "外觀變更"
                    };
                }

                var totalCost = 0;
                if (!string.IsNullOrEmpty(skinColor)) totalCost += SKIN_COLOR_COST;
                if (!string.IsNullOrEmpty(backgroundColor)) totalCost += BACKGROUND_COLOR_COST;

                if (totalCost == 0)
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = "請選擇要變更的外觀項目",
                        InteractionType = "外觀變更"
                    };
                }

                var userPoints = await GetUserPointsAsync(userId);
                if (userPoints < totalCost)
                {
                    return new PetInteractionResultViewModel
                    {
                        Success = false,
                        Message = $"積分不足，需要 {totalCost} 積分",
                        InteractionType = "外觀變更",
                        PointsCost = totalCost
                    };
                }

                var currentPet = await GetPetDataAsync(petId);
                
                // 模擬外觀更新 - 實際會更新 Pet 資料表的顏色欄位與時間戳
                await SimulatePetAppearanceUpdateAsync(petId, skinColor, backgroundColor);
                await SimulateUserPointsDeductAsync(userId, totalCost);

                var changeDescription = new List<string>();
                if (!string.IsNullOrEmpty(skinColor)) changeDescription.Add($"皮膚顏色變更為 {skinColor}");
                if (!string.IsNullOrEmpty(backgroundColor)) changeDescription.Add($"背景顏色變更為 {backgroundColor}");

                _logger.LogInformation("寵物外觀變更成功 - PetId: {PetId}, UserId: {UserId}, Changes: {Changes}", 
                    petId, userId, string.Join(", ", changeDescription));

                return new PetInteractionResultViewModel
                {
                    Success = true,
                    Message = $"外觀變更成功！{string.Join("，", changeDescription)}",
                    InteractionType = "外觀變更",
                    PointsCost = totalCost,
                    RemainingUserPoints = userPoints - totalCost
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寵物外觀變更失敗 - PetId: {PetId}, UserId: {UserId}", petId, userId);
                
                return new PetInteractionResultViewModel
                {
                    Success = false,
                    Message = "外觀變更失敗，請稍後再試",
                    InteractionType = "外觀變更"
                };
            }
        }

        /// <summary>
        /// 取得寵物完整狀態資料
        /// </summary>
        public async Task<PetStatusDisplayViewModel> GetPetStatusAsync(int petId, int userId)
        {
            var petData = await GetPetDataAsync(petId);
            
            return new PetStatusDisplayViewModel
            {
                Pet = petData,
                NextLevelExpRequired = GetExpRequiredForLevel(petData.Level + 1),
                ExpProgressPercentage = (double)petData.Experience / GetExpRequiredForLevel(petData.Level + 1) * 100,
                HungerStatus = GetStatusDescription(petData.Hunger),
                MoodStatus = GetStatusDescription(petData.Mood),
                OverallHealthScore = CalculateOverallScore(petData, new PetAttributeChangesViewModel()),
                NeedsCare = petData.Hunger < 30 || petData.Mood < 30 || petData.Stamina < 30 || petData.Cleanliness < 30,
                SuggestedCareActions = GenerateCareActions(petData),
                RecentActivities = await GenerateRecentActivitiesAsync(petId),
                LevelUpHistory = await GenerateLevelUpHistoryAsync(petId)
            };
        }

        /// <summary>
        /// 檢查寵物是否屬於指定使用者
        /// </summary>
        public async Task<bool> ValidatePetOwnershipAsync(int petId, int userId)
        {
            await Task.Delay(5); // 模擬資料庫查詢
            // Stage 4 模擬 - 實際會查詢 Pet.UserID = @userId AND Pet.PetID = @petId
            return true; // 暫時回傳有效
        }

        #region 私人輔助方法

        /// <summary>
        /// 模擬取得寵物資料 - 對應 Pet 資料表查詢
        /// </summary>
        private async Task<PetViewModel> GetPetDataAsync(int petId)
        {
            await Task.Delay(5);
            
            // Stage 4 模擬資料 - 實際會從 Pet 資料表查詢
            return new PetViewModel
            {
                PetID = petId,
                UserID = 1,
                PetName = "小火龍",
                Level = 5,
                Experience = 320,
                Hunger = 80,
                Mood = 75,
                Stamina = 60,
                Cleanliness = 90,
                Health = 95,
                SkinColor = "#FF6B35",
                BackgroundColor = "#FFE5B4"
            };
        }

        /// <summary>
        /// 取得使用者積分
        /// </summary>
        private async Task<int> GetUserPointsAsync(int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢 User_Wallet.User_Point
            return 1250;
        }

        /// <summary>
        /// 計算升級所需經驗值
        /// </summary>
        private int GetExpRequiredForLevel(int level)
        {
            return level * 100; // 每級需要 level * 100 經驗值
        }

        /// <summary>
        /// 取得屬性狀態描述
        /// </summary>
        private string GetStatusDescription(int value)
        {
            return value switch
            {
                >= 80 => "優秀",
                >= 60 => "良好", 
                >= 40 => "普通",
                >= 20 => "需要照顧",
                _ => "急需照顧"
            };
        }

        /// <summary>
        /// 計算寵物整體評分
        /// </summary>
        private int CalculateOverallScore(PetViewModel pet, PetAttributeChangesViewModel changes)
        {
            var newHunger = Math.Max(0, Math.Min(100, pet.Hunger + changes.HungerDelta));
            var newMood = Math.Max(0, Math.Min(100, pet.Mood + changes.MoodDelta));
            var newStamina = Math.Max(0, Math.Min(100, pet.Stamina + changes.StaminaDelta));
            var newCleanliness = Math.Max(0, Math.Min(100, pet.Cleanliness + changes.CleanlinessDelta));
            var newHealth = Math.Max(0, Math.Min(100, pet.Health + changes.HealthDelta));

            return (int)((newHunger + newMood + newStamina + newCleanliness + newHealth) / 5.0);
        }

        /// <summary>
        /// 產生照顧建議
        /// </summary>
        private List<string> GenerateCareActions(PetViewModel pet)
        {
            var actions = new List<string>();
            
            if (pet.Hunger < 40) actions.Add("建議餵食提升飢餓度");
            if (pet.Mood < 40) actions.Add("建議陪玩提升心情");
            if (pet.Stamina < 30) actions.Add("讓寵物休息恢復體力");
            if (pet.Cleanliness < 40) actions.Add("建議清潔提升清潔度");
            if (pet.Health < 50) actions.Add("注意寵物健康狀況");

            return actions;
        }

        /// <summary>
        /// 模擬寵物資料表更新
        /// </summary>
        private async Task SimulatePetUpdateAsync(int petId, PetAttributeChangesViewModel changes, PetLevelUpRewardViewModel? levelUpReward)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會更新 Pet 資料表的各屬性欄位
            _logger.LogInformation("模擬寵物屬性更新 - PetId: {PetId}, Changes: {Changes}", petId, changes);
        }

        /// <summary>
        /// 模擬使用者積分扣除
        /// </summary>
        private async Task SimulateUserPointsDeductAsync(int userId, int pointsToDeduct)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會更新 User_Wallet.User_Point
            _logger.LogInformation("模擬使用者積分扣除 - UserId: {UserId}, PointsDeducted: {Points}", userId, pointsToDeduct);
        }

        /// <summary>
        /// 模擬寵物外觀更新
        /// </summary>
        private async Task SimulatePetAppearanceUpdateAsync(int petId, string? skinColor, string? backgroundColor)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會更新 Pet 資料表的 SkinColor, BackgroundColor 和對應的時間戳欄位
            _logger.LogInformation("模擬寵物外觀更新 - PetId: {PetId}, SkinColor: {SkinColor}, BgColor: {BgColor}", petId, skinColor, backgroundColor);
        }

        /// <summary>
        /// 產生最近活動記錄
        /// </summary>
        private async Task<List<PetActivityLogViewModel>> GenerateRecentActivitiesAsync(int petId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢寵物互動歷史
            return new List<PetActivityLogViewModel>
            {
                new PetActivityLogViewModel { ActivityType = "餵食", Description = "餵食了美味的寵物糧食", ActivityTime = DateTime.Now.AddHours(-2), IconClass = "fas fa-utensils text-warning" },
                new PetActivityLogViewModel { ActivityType = "陪玩", Description = "完成了一場愉快的遊戲", ActivityTime = DateTime.Now.AddHours(-5), IconClass = "fas fa-gamepad text-info" }
            };
        }

        /// <summary>
        /// 產生升級歷史記錄
        /// </summary>
        private async Task<List<PetLevelUpHistoryViewModel>> GenerateLevelUpHistoryAsync(int petId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢升級歷史記錄
            return new List<PetLevelUpHistoryViewModel>
            {
                new PetLevelUpHistoryViewModel { Level = 5, LevelUpTime = DateTime.Now.AddDays(-1), PointsReward = 50 },
                new PetLevelUpHistoryViewModel { Level = 4, LevelUpTime = DateTime.Now.AddDays(-8), PointsReward = 40 }
            };
        }

        #endregion
    }
}