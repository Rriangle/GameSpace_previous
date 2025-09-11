using GameSpace.Areas.MiniGame.Models;
using Microsoft.Extensions.Logging;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 遊戲服務實作 - 處理遊戲結果提交、獎勵計算與記錄
    /// 對應 database.sql MiniGame、User_Wallet、Pet 資料表操作
    /// </summary>
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> _logger;

        public GameService(ILogger<GameService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 提交遊戲結果 - 記錄到 MiniGame 資料表並計算獎勵
        /// </summary>
        public async Task<GameSubmissionResultViewModel> SubmitGameResultAsync(GameResultSubmissionModel gameResult, int userId, int petId)
        {
            try
            {
                // 檢查遊戲可玩性
                var eligibility = await CheckGameEligibilityAsync(petId, gameResult.GameId);
                if (!eligibility.CanPlay)
                {
                    return new GameSubmissionResultViewModel
                    {
                        Success = false,
                        Message = eligibility.Reason ?? "無法進行遊戲",
                        CompletedAt = DateTime.Now
                    };
                }

                var completedAt = DateTime.Now;
                var endTime = gameResult.StartTime.AddSeconds(gameResult.DurationSeconds);

                // 計算基礎獎勵 - 基於分數、等級、完成狀態
                var basePointsReward = CalculateBasePointsReward(gameResult);
                var baseExpReward = CalculateBaseExpReward(gameResult);
                
                // 等級獎勵加成
                var levelMultiplier = 1.0 + (gameResult.Level - 1) * 0.1; // 每級增加10%獎勵
                var finalPointsGained = (int)(basePointsReward * levelMultiplier);
                var finalExpGained = (int)(baseExpReward * levelMultiplier);

                // 檢查是否達到新高分
                var currentHighScore = await GetUserHighScoreAsync(userId, gameResult.GameId);
                var newHighScore = gameResult.Score > currentHighScore;
                if (newHighScore)
                {
                    finalPointsGained += 50; // 新高分獎勵
                }

                // 計算遊戲評級
                var gameRank = CalculateGameRank(gameResult.Score, gameResult.Level);
                
                // 特殊獎勵優惠券 - 基於評級與機率
                string? bonusCouponCode = null;
                if (gameRank == "S" && Random.Shared.NextDouble() < 0.3) // S級30%機率
                {
                    bonusCouponCode = $"GAME_S_{DateTime.Now:yyyyMMdd}_{Random.Shared.Next(1000, 9999)}";
                }
                else if (gameRank == "A" && Random.Shared.NextDouble() < 0.15) // A級15%機率
                {
                    bonusCouponCode = $"GAME_A_{DateTime.Now:yyyyMMdd}_{Random.Shared.Next(1000, 9999)}";
                }

                // 計算寵物屬性變化 - 遊戲會影響寵物狀態
                var petChanges = CalculatePetAttributeChanges(gameResult);

                // 檢查寵物是否因經驗增加而升級
                var petLevelUpReward = await CheckPetLevelUpAsync(petId, petChanges.ExperienceDelta);

                // 模擬資料庫寫入操作 - Stage 4 階段
                var playRecordId = await SimulateMiniGameInsertAsync(gameResult, userId, petId, finalPointsGained, finalExpGained, bonusCouponCode ?? string.Empty, petChanges, endTime);
                await SimulateUserWalletUpdateAsync(userId, finalPointsGained);
                await SimulatePetUpdateAfterGameAsync(petId, petChanges, petLevelUpReward);

                // 如果有優惠券獎勵，記錄到相關資料表
                if (!string.IsNullOrEmpty(bonusCouponCode))
                {
                    await SimulateCouponCreateAsync(bonusCouponCode, userId, completedAt);
                }

                _logger.LogInformation("遊戲結果提交成功 - UserId: {UserId}, GameId: {GameId}, Score: {Score}, Points: {Points}, Rank: {Rank}", 
                    userId, gameResult.GameId, gameResult.Score, finalPointsGained, gameRank);

                var totalPoints = await GetUserTotalPointsAsync(userId) + finalPointsGained + (petLevelUpReward?.PointsReward ?? 0);

                return new GameSubmissionResultViewModel
                {
                    Success = true,
                    Message = $"遊戲完成！評級 {gameRank}，獲得 {finalPointsGained} 積分",
                    PlayRecordId = playRecordId,
                    PointsGained = finalPointsGained,
                    ExpGained = finalExpGained,
                    BonusCouponCode = bonusCouponCode,
                    PetChanges = petChanges,
                    PetLevelUpTriggered = petLevelUpReward != null,
                    PetLevelUpReward = petLevelUpReward,
                    NewHighScore = newHighScore,
                    GameRank = gameRank,
                    TotalUserPoints = totalPoints,
                    CompletedAt = completedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "遊戲結果提交失敗 - UserId: {UserId}, GameId: {GameId}", userId, gameResult.GameId);
                
                return new GameSubmissionResultViewModel
                {
                    Success = false,
                    Message = "遊戲結果提交失敗，請稍後再試",
                    CompletedAt = DateTime.Now
                };
            }
        }

        /// <summary>
        /// 取得遊戲大廳顯示資料
        /// </summary>
        public async Task<GameHallDisplayViewModel> GetGameHallDataAsync(int userId)
        {
            var availableGames = await GetAvailableGamesAsync();
            var recentHistory = await GetUserGameHistoryAsync(userId, 5);
            var weeklyStats = await GetWeeklyStatsAsync(userId);
            var todayStats = await GetTodayStatsAsync(userId);

            return new GameHallDisplayViewModel
            {
                UserId = userId,
                TodayGameCount = todayStats.GameCount,
                TodayPointsEarned = todayStats.PointsEarned,
                HighestScore = await GetUserHighScoreAsync(userId),
                CurrentGameLevel = await GetUserGameLevelAsync(userId),
                AvailableGames = availableGames,
                RecentGameHistory = recentHistory,
                WeeklyStats = weeklyStats
            };
        }

        /// <summary>
        /// 取得可用遊戲列表
        /// </summary>
        public async Task<List<AvailableGameViewModel>> GetAvailableGamesAsync()
        {
            await Task.Delay(5); // 模擬查詢延遲
            
            // Stage 4 模擬 - 實際會從遊戲設定資料表查詢
            return new List<AvailableGameViewModel>
            {
                new AvailableGameViewModel { GameId = 1, GameName = "打怪獸", Description = "經典射擊遊戲，考驗反應速度與準確度", PointsReward = 50, DifficultyLevel = 3, IconClass = "fas fa-crosshairs text-danger", IsEnabled = true },
                new AvailableGameViewModel { GameId = 2, GameName = "寵物冒險", Description = "策略冒險遊戲，與寵物一起探索未知世界", PointsReward = 75, DifficultyLevel = 4, IconClass = "fas fa-map text-success", IsEnabled = true },
                new AvailableGameViewModel { GameId = 3, GameName = "記憶挑戰", Description = "記憶力訓練遊戲，提升專注力與記憶能力", PointsReward = 30, DifficultyLevel = 2, IconClass = "fas fa-brain text-warning", IsEnabled = true }
            };
        }

        /// <summary>
        /// 取得使用者遊戲歷史記錄
        /// </summary>
        public async Task<List<MiniGameViewModel>> GetUserGameHistoryAsync(int userId, int limit = 10)
        {
            await Task.Delay(5); // 模擬查詢延遲
            
            // Stage 4 模擬 - 實際會從 MiniGame 資料表查詢 WHERE UserID = @userId
            return new List<MiniGameViewModel>
            {
                new MiniGameViewModel { PlayID = 1, UserID = userId, Level = 5, Result = "勝利", ExpGained = 25, PointsGained = 50, StartTime = DateTime.Now.AddHours(-3), EndTime = DateTime.Now.AddHours(-3).AddMinutes(3).AddSeconds(45), MonsterCount = 8, SpeedMultiplier = 1.2m },
                new MiniGameViewModel { PlayID = 2, UserID = userId, Level = 3, Result = "勝利", ExpGained = 15, PointsGained = 30, StartTime = DateTime.Now.AddHours(-6), EndTime = DateTime.Now.AddHours(-6).AddMinutes(2).AddSeconds(30), MonsterCount = 5, SpeedMultiplier = 1.0m },
                new MiniGameViewModel { PlayID = 3, UserID = userId, Level = 8, Result = "勝利", ExpGained = 40, PointsGained = 75, StartTime = DateTime.Now.AddDays(-1), EndTime = DateTime.Now.AddDays(-1).AddMinutes(8).AddSeconds(20), MonsterCount = 12, SpeedMultiplier = 1.5m }
            };
        }

        /// <summary>
        /// 檢查遊戲可玩性 - 基於寵物狀態
        /// </summary>
        public async Task<GameEligibilityViewModel> CheckGameEligibilityAsync(int petId, int gameId)
        {
            await Task.Delay(5);
            
            // 模擬寵物狀態檢查 - 實際會查詢 Pet 資料表
            var petConditionScore = 75; // 模擬目前寵物狀態評分
            var minRequiredScore = 30; // 最低要求
            
            var canPlay = petConditionScore >= minRequiredScore;
            var suggestions = new List<string>();
            
            if (!canPlay)
            {
                suggestions.Add("請先照顧寵物，提升整體狀態");
                if (petConditionScore < 20) suggestions.Add("寵物狀態過低，建議餵食和陪玩");
                if (petConditionScore < 40) suggestions.Add("建議讓寵物休息一下");
            }

            return new GameEligibilityViewModel
            {
                CanPlay = canPlay,
                Reason = canPlay ? null : "寵物狀態不佳，無法進行遊戲",
                Suggestions = suggestions,
                PetConditionScore = petConditionScore,
                MinRequiredScore = minRequiredScore
            };
        }

        #region 私人輔助方法

        /// <summary>
        /// 計算基礎積分獎勵
        /// </summary>
        private int CalculateBasePointsReward(GameResultSubmissionModel gameResult)
        {
            var baseReward = gameResult.GameId switch
            {
                1 => 50,  // 打怪獸
                2 => 75,  // 寵物冒險
                3 => 30,  // 記憶挑戰
                _ => 25
            };

            // 基於分數調整獎勵
            var scoreMultiplier = Math.Min(gameResult.Score / 1000.0, 2.0); // 最多2倍獎勵
            
            // 完成獎勵 vs 中止懲罰
            var completionMultiplier = gameResult.IsCompleted ? 1.0 : 0.5;
            
            return (int)(baseReward * scoreMultiplier * completionMultiplier);
        }

        /// <summary>
        /// 計算基礎經驗值獎勵
        /// </summary>
        private int CalculateBaseExpReward(GameResultSubmissionModel gameResult)
        {
            var baseExp = gameResult.GameId switch
            {
                1 => 15,  // 打怪獸
                2 => 25,  // 寵物冒險 - 經驗較多
                3 => 10,  // 記憶挑戰
                _ => 8
            };

            return gameResult.IsCompleted ? baseExp : baseExp / 2;
        }

        /// <summary>
        /// 計算遊戲評級
        /// </summary>
        private string CalculateGameRank(int score, int level)
        {
            var threshold = level * 200; // 基礎門檻隨等級調整

            return score switch
            {
                var s when s >= threshold * 2.5 => "S",
                var s when s >= threshold * 2.0 => "A",
                var s when s >= threshold * 1.5 => "B", 
                var s when s >= threshold * 1.0 => "C",
                _ => "D"
            };
        }

        /// <summary>
        /// 計算寵物屬性變化 - 遊戲對寵物的影響
        /// </summary>
        private PetAttributeChangesViewModel CalculatePetAttributeChanges(GameResultSubmissionModel gameResult)
        {
            // 不同遊戲對寵物屬性有不同影響
            return gameResult.GameId switch
            {
                1 => new PetAttributeChangesViewModel // 打怪獸 - 消耗體力但提升經驗
                {
                    StaminaDelta = -10,
                    MoodDelta = gameResult.IsCompleted ? 5 : -5,
                    ExperienceDelta = gameResult.IsCompleted ? 8 : 3,
                    HungerDelta = -5 // 遊戲會消耗飢餓度
                },
                2 => new PetAttributeChangesViewModel // 寵物冒險 - 全面影響但較溫和
                {
                    StaminaDelta = -15,
                    MoodDelta = gameResult.IsCompleted ? 10 : -3,
                    ExperienceDelta = gameResult.IsCompleted ? 15 : 8,
                    HungerDelta = -8,
                    CleanlinessDelta = -5 // 冒險會弄髒
                },
                3 => new PetAttributeChangesViewModel // 記憶挑戰 - 輕微影響
                {
                    StaminaDelta = -5,
                    MoodDelta = gameResult.IsCompleted ? 8 : 0,
                    ExperienceDelta = gameResult.IsCompleted ? 5 : 2,
                    HungerDelta = -2
                },
                _ => new PetAttributeChangesViewModel()
            };
        }

        /// <summary>
        /// 檢查寵物是否升級
        /// </summary>
        private async Task<PetLevelUpRewardViewModel?> CheckPetLevelUpAsync(int petId, int expGain)
        {
            await Task.Delay(5);
            
            // 模擬寵物資料查詢 - 實際會從 Pet 資料表查詢
            var currentLevel = 5; // 模擬目前等級
            var currentExp = 320; // 模擬目前經驗值
            var newExp = currentExp + expGain;
            var expRequiredForNextLevel = (currentLevel + 1) * 100;

            if (newExp >= expRequiredForNextLevel)
            {
                var newLevel = currentLevel + 1;
                var pointsReward = newLevel * 15; // 寵物升級獎勵積分

                return new PetLevelUpRewardViewModel
                {
                    NewLevel = newLevel,
                    PointsReward = pointsReward,
                    LevelUpTime = DateTime.Now,
                    Message = $"寵物在遊戲中升級到 Lv.{newLevel}！"
                };
            }

            return null;
        }

        /// <summary>
        /// 取得使用者歷史最高分
        /// </summary>
        private async Task<int> GetUserHighScoreAsync(int userId, int? gameId = null)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢 MiniGame 資料表的最高分數
            return 2000; // 模擬歷史最高分
        }

        /// <summary>
        /// 取得使用者遊戲等級
        /// </summary>
        private async Task<int> GetUserGameLevelAsync(int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會基於總經驗值或遊戲次數計算
            return 12; // 模擬目前等級
        }

        /// <summary>
        /// 取得今日遊戲統計
        /// </summary>
        private async Task<(int GameCount, int PointsEarned)> GetTodayStatsAsync(int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢今日的 MiniGame 記錄
            return (3, 150);
        }

        /// <summary>
        /// 取得本週遊戲統計
        /// </summary>
        private async Task<WeeklyGameStatsViewModel> GetWeeklyStatsAsync(int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢本週的 MiniGame 記錄聚合
            return new WeeklyGameStatsViewModel
            {
                WeeklyGameCount = 15,
                WeeklyPointsEarned = 720,
                WeeklyHighScore = 2450,
                AveragePlayTimeMinutes = 4
            };
        }

        /// <summary>
        /// 取得使用者總積分
        /// </summary>
        private async Task<int> GetUserTotalPointsAsync(int userId)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會查詢 User_Wallet.User_Point
            return 1250;
        }

        /// <summary>
        /// 模擬 MiniGame 資料表插入
        /// </summary>
        private async Task<int> SimulateMiniGameInsertAsync(GameResultSubmissionModel gameResult, int userId, int petId, int pointsGained, int expGained, string couponGained, PetAttributeChangesViewModel petChanges, DateTime endTime)
        {
            await Task.Delay(10);
            
            var playId = Random.Shared.Next(10000, 99999); // 模擬生成的 PlayID
            
            // Stage 4 模擬 - 實際會執行以下 SQL:
            // INSERT INTO MiniGame (PlayID, UserID, PetID, Level, MonsterCount, SpeedMultiplier, Result, 
            //                      ExpGained, ExpGainedTime, PointsGained, PointsGainedTime, CouponGained, CouponGainedTime,
            //                      HungerDelta, MoodDelta, StaminaDelta, CleanlinessDelta, StartTime, EndTime, Aborted)
            // VALUES (@playId, @userId, @petId, @level, @monsterCount, @speedMultiplier, @result,
            //         @expGained, @endTime, @pointsGained, @endTime, @couponGained, @endTime,
            //         @hungerDelta, @moodDelta, @staminaDelta, @cleanlinessDelta, @startTime, @endTime, @aborted)

            _logger.LogInformation("模擬遊戲記錄插入 - PlayID: {PlayId}, UserId: {UserId}, Points: {Points}", 
                playId, userId, pointsGained);

            return playId;
        }

        /// <summary>
        /// 模擬 User_Wallet 積分更新
        /// </summary>
        private async Task SimulateUserWalletUpdateAsync(int userId, int pointsToAdd)
        {
            await Task.Delay(5);
            // Stage 4 模擬 - 實際會執行: UPDATE User_Wallet SET User_Point = User_Point + @pointsToAdd WHERE User_Id = @userId
            _logger.LogInformation("模擬使用者積分增加 - UserId: {UserId}, PointsAdded: {Points}", userId, pointsToAdd);
        }

        /// <summary>
        /// 模擬 Pet 屬性更新
        /// </summary>
        private async Task SimulatePetUpdateAfterGameAsync(int petId, PetAttributeChangesViewModel changes, PetLevelUpRewardViewModel? levelUpReward)
        {
            await Task.Delay(5);
            
            // Stage 4 模擬 - 實際會執行 Pet 資料表的屬性更新
            // UPDATE Pet SET Hunger = Hunger + @hungerDelta, Mood = Mood + @moodDelta, 
            //               Stamina = Stamina + @staminaDelta, Cleanliness = Cleanliness + @cleanlinessDelta,
            //               Experience = Experience + @expDelta [, Level = @newLevel, LevelUpTime = @levelUpTime]
            // WHERE PetID = @petId
            
            _logger.LogInformation("模擬寵物屬性更新 - PetId: {PetId}, Changes: {@Changes}, LevelUp: {LevelUp}", 
                petId, changes, levelUpReward != null);
        }

        /// <summary>
        /// 模擬優惠券建立
        /// </summary>
        private async Task SimulateCouponCreateAsync(string couponCode, int userId, DateTime acquiredTime)
        {
            await Task.Delay(5);
            
            // Stage 4 模擬 - 實際會插入 Coupon 資料表
            // INSERT INTO Coupon (CouponID, CouponCode, CouponTypeID, UserID, IsUsed, AcquiredTime)
            // VALUES (NEXT_ID, @couponCode, 1, @userId, 0, @acquiredTime)
            
            _logger.LogInformation("模擬遊戲獎勵優惠券建立 - Code: {Code}, UserId: {UserId}", couponCode, userId);
        }

        #endregion
    }
}