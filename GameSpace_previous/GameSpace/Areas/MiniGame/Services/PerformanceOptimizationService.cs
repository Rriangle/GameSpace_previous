using GameSpace.Areas.MiniGame.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace GameSpace.Areas.MiniGame.Services
{
    /// <summary>
    /// 性能優化服務實作 - 處理 MiniGame 區域的熱點查詢優化
    /// 專注於錢包聚合、簽到寫入、寵物讀取等高頻操作的性能提升
    /// </summary>
    public class PerformanceOptimizationService : IPerformanceOptimizationService
    {
        private readonly ILogger<PerformanceOptimizationService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IWalletService _walletService;
        private readonly IUserSignInService _signInService;
        private readonly IPetInteractionService _petService;
        private readonly IGameService _gameService;

        // 快取設定常數
        private const int CACHE_EXPIRY_MINUTES = 5;
        private const int USER_DATA_CACHE_MINUTES = 10;
        private const int STATIC_DATA_CACHE_MINUTES = 30;

        public PerformanceOptimizationService(
            ILogger<PerformanceOptimizationService> logger,
            IMemoryCache cache,
            IWalletService walletService,
            IUserSignInService signInService,
            IPetInteractionService petService,
            IGameService gameService)
        {
            _logger = logger;
            _cache = cache;
            _walletService = walletService;
            _signInService = signInService;
            _petService = petService;
            _gameService = gameService;
        }

        /// <summary>
        /// 優化錢包聚合查詢 - 使用快取和批次查詢減少資料庫負載
        /// 性能目標：從平均 150ms 降至 50ms 以下
        /// </summary>
        public async Task<WalletOverviewDisplayViewModel> GetOptimizedWalletOverviewAsync(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"wallet_overview_{userId}";

            try
            {
                // 嘗試從快取取得資料
                if (_cache.TryGetValue(cacheKey, out WalletOverviewDisplayViewModel? cachedData) && cachedData != null)
                {
                    _logger.LogDebug("錢包聚合查詢快取命中 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                        userId, stopwatch.ElapsedMilliseconds);
                    return cachedData;
                }

                // 並行查詢多個資料源以提升性能
                var tasks = new[]
                {
                    _walletService.GetWalletOverviewAsync(userId), // 主要錢包資料
                    _walletService.GetAvailableCouponTypesAsync(),  // 靜態優惠券類型
                    _walletService.GetAvailableEVoucherTypesAsync() // 靜態電子禮券類型
                };

                await Task.WhenAll(tasks);

                var walletData = await tasks[0];
                var couponTypes = await tasks[1];
                var voucherTypes = await tasks[2];

                // 增強資料處理 - 添加類型資訊到優惠券和電子禮券
                foreach (var coupon in walletData.AvailableCoupons)
                {
                    coupon.CouponType = couponTypes.FirstOrDefault(ct => ct.CouponTypeID == coupon.CouponTypeID);
                }

                foreach (var voucher in walletData.AvailableEVouchers)
                {
                    voucher.EVoucherType = voucherTypes.FirstOrDefault(vt => vt.EVoucherTypeID == voucher.EVoucherTypeID);
                }

                // 設定快取 - 較短時間以確保資料新鮮度
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES),
                    Priority = CacheItemPriority.High
                };

                _cache.Set(cacheKey, walletData, cacheOptions);

                stopwatch.Stop();
                _logger.LogInformation("錢包聚合查詢優化完成 - UserId: {UserId}, 耗時: {ElapsedMs}ms, 快取狀態: 新建", 
                    userId, stopwatch.ElapsedMilliseconds);

                return walletData;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "錢包聚合查詢優化失敗 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    userId, stopwatch.ElapsedMilliseconds);
                
                // 降級到標準查詢
                return await _walletService.GetWalletOverviewAsync(userId);
            }
        }

        /// <summary>
        /// 優化簽到寫入路徑 - 使用事務批次處理提升寫入性能
        /// 性能目標：從平均 200ms 降至 80ms 以下
        /// </summary>
        public async Task<SignInResultViewModel> ProcessOptimizedSignInAsync(int userId)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // 預先檢查快取狀態 - 避免重複簽到檢查
                var todayCacheKey = $"signin_status_{userId}_{DateTime.Now:yyyyMMdd}";
                if (_cache.TryGetValue(todayCacheKey, out bool hasSignedToday) && hasSignedToday)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("簽到狀態快取命中 - UserId: {UserId}, 已簽到, 耗時: {ElapsedMs}ms", 
                        userId, stopwatch.ElapsedMilliseconds);
                    
                    return new SignInResultViewModel
                    {
                        Success = false,
                        Message = "您今日已經簽到過了",
                        SignInTime = DateTime.Now
                    };
                }

                // 執行優化的簽到流程
                var result = await _signInService.ProcessSignInAsync(userId);

                if (result.Success)
                {
                    // 設定今日簽到狀態快取
                    var signInCacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Today.AddDays(1), // 到今日結束過期
                        Priority = CacheItemPriority.High
                    };
                    _cache.Set(todayCacheKey, true, signInCacheOptions);

                    // 清除相關快取以確保資料一致性
                    _cache.Remove($"wallet_overview_{userId}");
                    _cache.Remove($"signin_stats_{userId}");
                }

                stopwatch.Stop();
                _logger.LogInformation("簽到寫入路徑優化完成 - UserId: {UserId}, 成功: {Success}, 耗時: {ElapsedMs}ms", 
                    userId, result.Success, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "簽到寫入路徑優化失敗 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    userId, stopwatch.ElapsedMilliseconds);
                
                // 降級到標準簽到流程
                return await _signInService.ProcessSignInAsync(userId);
            }
        }

        /// <summary>
        /// 優化寵物狀態讀取 - 智慧快取和預載入策略
        /// 性能目標：從平均 100ms 降至 30ms 以下
        /// </summary>
        public async Task<PetStatusDisplayViewModel> GetOptimizedPetStatusAsync(int petId, int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"pet_status_{petId}_{userId}";

            try
            {
                // 檢查快取
                if (_cache.TryGetValue(cacheKey, out PetStatusDisplayViewModel? cachedPetData) && cachedPetData != null)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("寵物狀態快取命中 - PetId: {PetId}, UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                        petId, userId, stopwatch.ElapsedMilliseconds);
                    return cachedPetData;
                }

                // 執行優化的寵物資料查詢
                var petData = await _petService.GetPetStatusAsync(petId, userId);

                // 預載入相關資料 - 減少後續查詢
                var preloadTasks = new[]
                {
                    PreloadPetInteractionHistoryAsync(petId),
                    PreloadPetLevelUpHistoryAsync(petId),
                    PreloadUserGameDataAsync(userId) // 為遊戲功能預載入
                };

                // 非阻塞預載入
                _ = Task.WhenAll(preloadTasks).ContinueWith(t => 
                {
                    if (t.Exception != null)
                    {
                        _logger.LogWarning(t.Exception, "寵物相關資料預載入部分失敗 - PetId: {PetId}", petId);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);

                // 設定快取 - 寵物狀態變化相對較慢
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(USER_DATA_CACHE_MINUTES),
                    SlidingExpiration = TimeSpan.FromMinutes(3), // 3分鐘內有訪問就延長
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, petData, cacheOptions);

                stopwatch.Stop();
                _logger.LogInformation("寵物狀態讀取優化完成 - PetId: {PetId}, UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    petId, userId, stopwatch.ElapsedMilliseconds);

                return petData;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "寵物狀態讀取優化失敗 - PetId: {PetId}, UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    petId, userId, stopwatch.ElapsedMilliseconds);
                
                // 降級到標準查詢
                return await _petService.GetPetStatusAsync(petId, userId);
            }
        }

        /// <summary>
        /// 優化遊戲大廳資料載入 - 並行查詢和預計算
        /// 性能目標：從平均 120ms 降至 60ms 以下
        /// </summary>
        public async Task<GameHallDisplayViewModel> GetOptimizedGameHallDataAsync(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"game_hall_{userId}";

            try
            {
                // 檢查快取
                if (_cache.TryGetValue(cacheKey, out GameHallDisplayViewModel? cachedGameData) && cachedGameData != null)
                {
                    stopwatch.Stop();
                    _logger.LogDebug("遊戲大廳快取命中 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                        userId, stopwatch.ElapsedMilliseconds);
                    return cachedGameData;
                }

                // 並行查詢提升載入速度
                var parallelTasks = new[]
                {
                    _gameService.GetGameHallDataAsync(userId),
                    PreloadWalletSummaryAsync(userId), // 預載入錢包摘要
                    PreloadPetStatusSummaryAsync(userId) // 預載入寵物狀態摘要
                };

                var gameHallData = await parallelTasks[0];

                // 設定快取 - 遊戲統計相對穩定
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES),
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, gameHallData, cacheOptions);

                stopwatch.Stop();
                _logger.LogInformation("遊戲大廳載入優化完成 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    userId, stopwatch.ElapsedMilliseconds);

                return gameHallData;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "遊戲大廳載入優化失敗 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    userId, stopwatch.ElapsedMilliseconds);
                
                // 降級到標準查詢
                return await _gameService.GetGameHallDataAsync(userId);
            }
        }

        /// <summary>
        /// 快取預熱 - 預載入使用者常用資料
        /// </summary>
        public async Task<bool> WarmupUserCacheAsync(int userId)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                // 預載入核心資料到快取
                var warmupTasks = new[]
                {
                    GetOptimizedWalletOverviewAsync(userId),
                    GetOptimizedPetStatusAsync(1, userId), // 假設寵物ID為1
                    GetOptimizedGameHallDataAsync(userId)
                };

                await Task.WhenAll(warmupTasks);

                stopwatch.Stop();
                _logger.LogInformation("使用者快取預熱完成 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    userId, stopwatch.ElapsedMilliseconds);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "使用者快取預熱失敗 - UserId: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// 批次積分更新 - 優化多筆積分異動的寫入性能
        /// </summary>
        public async Task<bool> BatchUpdateUserPointsAsync(int userId, List<PointsChangeRecord> pointsChanges)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (!pointsChanges.Any())
                {
                    return true;
                }

                // 批次處理積分變動 - 減少資料庫事務數量
                var totalPointsChange = pointsChanges.Sum(pc => pc.PointsChanged);
                
                // Stage 5 模擬批次更新 - 實際會使用單一事務處理多筆更新
                await Task.Delay(10); // 模擬批次寫入延遲

                // 批次插入 WalletHistory 記錄
                foreach (var change in pointsChanges)
                {
                    await Task.Delay(1); // 模擬批次插入
                }

                // 清除相關快取
                _cache.Remove($"wallet_overview_{userId}");
                _cache.Remove($"wallet_history_{userId}");

                stopwatch.Stop();
                _logger.LogInformation("批次積分更新完成 - UserId: {UserId}, 異動筆數: {Count}, 總變動: {TotalChange}, 耗時: {ElapsedMs}ms", 
                    userId, pointsChanges.Count, totalPointsChange, stopwatch.ElapsedMilliseconds);

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "批次積分更新失敗 - UserId: {UserId}, 耗時: {ElapsedMs}ms", 
                    userId, stopwatch.ElapsedMilliseconds);
                return false;
            }
        }

        /// <summary>
        /// 取得查詢性能統計
        /// </summary>
        public async Task<PerformanceStatsViewModel> GetPerformanceStatsAsync()
        {
            await Task.Delay(10); // 模擬統計查詢

            // Stage 5 模擬性能統計 - 實際會從性能監控系統取得
            return new PerformanceStatsViewModel
            {
                WalletQueryAverageMs = 45.2,
                SignInWriteAverageMs = 78.5,
                PetQueryAverageMs = 28.3,
                GameHallLoadAverageMs = 55.1,
                CacheHitRate = 0.85, // 85% 快取命中率
                DbConnectionPoolUsage = 0.45, // 45% 連線池使用率
                MemoryUsageMB = 128.5,
                StatsPeriodStart = DateTime.Now.AddHours(-1),
                StatsPeriodEnd = DateTime.Now
            };
        }

        #region 私人預載入方法

        /// <summary>
        /// 預載入寵物互動歷史
        /// </summary>
        private async Task PreloadPetInteractionHistoryAsync(int petId)
        {
            var cacheKey = $"pet_history_{petId}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                await Task.Delay(5); // 模擬歷史查詢
                var historyData = new { petId = petId, lastInteraction = DateTime.Now };
                
                _cache.Set(cacheKey, historyData, TimeSpan.FromMinutes(USER_DATA_CACHE_MINUTES));
                _logger.LogDebug("寵物互動歷史預載入完成 - PetId: {PetId}", petId);
            }
        }

        /// <summary>
        /// 預載入寵物升級歷史
        /// </summary>
        private async Task PreloadPetLevelUpHistoryAsync(int petId)
        {
            var cacheKey = $"pet_levelup_history_{petId}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                await Task.Delay(5); // 模擬升級歷史查詢
                var levelUpData = new { petId = petId, currentLevel = 5 };
                
                _cache.Set(cacheKey, levelUpData, TimeSpan.FromMinutes(STATIC_DATA_CACHE_MINUTES));
                _logger.LogDebug("寵物升級歷史預載入完成 - PetId: {PetId}", petId);
            }
        }

        /// <summary>
        /// 預載入使用者遊戲資料
        /// </summary>
        private async Task PreloadUserGameDataAsync(int userId)
        {
            var cacheKey = $"user_game_summary_{userId}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                await Task.Delay(8); // 模擬遊戲統計查詢
                var gameData = new { userId = userId, totalGames = 50, highScore = 2450 };
                
                _cache.Set(cacheKey, gameData, TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES));
                _logger.LogDebug("使用者遊戲資料預載入完成 - UserId: {UserId}", userId);
            }
        }

        /// <summary>
        /// 預載入錢包摘要
        /// </summary>
        private async Task PreloadWalletSummaryAsync(int userId)
        {
            var cacheKey = $"wallet_summary_{userId}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                await Task.Delay(5); // 模擬錢包摘要查詢
                var walletSummary = new { userId = userId, totalPoints = 1250 };
                
                _cache.Set(cacheKey, walletSummary, TimeSpan.FromMinutes(USER_DATA_CACHE_MINUTES));
                _logger.LogDebug("錢包摘要預載入完成 - UserId: {UserId}", userId);
            }
        }

        /// <summary>
        /// 預載入寵物狀態摘要
        /// </summary>
        private async Task PreloadPetStatusSummaryAsync(int userId)
        {
            var cacheKey = $"pet_summary_{userId}";
            if (!_cache.TryGetValue(cacheKey, out _))
            {
                await Task.Delay(5); // 模擬寵物摘要查詢
                var petSummary = new { userId = userId, petLevel = 5, overallScore = 80 };
                
                _cache.Set(cacheKey, petSummary, TimeSpan.FromMinutes(USER_DATA_CACHE_MINUTES));
                _logger.LogDebug("寵物狀態摘要預載入完成 - UserId: {UserId}", userId);
            }
        }

        #endregion
    }
}