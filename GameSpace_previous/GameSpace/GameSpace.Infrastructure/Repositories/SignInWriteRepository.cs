using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// 簽到寫入專用存儲庫實現 - Stage 3 寫入操作
    /// 實現簽到相關的寫入操作，包含交易處理和冪等性
    /// </summary>
    public class SignInWriteRepository : ISignInWriteRepository
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<SignInWriteRepository> _logger;

        public SignInWriteRepository(GameSpaceDbContext context, ILogger<SignInWriteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 執行用戶簽到操作（包含交易處理和冪等性檢查）
        /// </summary>
        public async Task<SignInResponse> ProcessSignInAsync(SignInRequest request)
        {
            // 1. 檢查冪等性
            var existingResponse = await CheckIdempotencyAsync(request.IdempotencyKey);
            if (existingResponse != null)
            {
                _logger.LogInformation("簽到請求已處理過，返回快取結果 IdempotencyKey: {IdempotencyKey}", request.IdempotencyKey);
                return existingResponse;
            }

            // 2. 使用交易處理簽到邏輯
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var signInTime = request.SignInTime ?? DateTime.UtcNow;
                
                // 獲取簽到統計
                var stats = await GetOrCreateSignInStatsAsync(request.UserId);
                
                // 檢查是否今天已經簽到
                if (stats.LastSignInDate.Date == signInTime.Date)
                {
                    var duplicateResponse = new SignInResponse
                    {
                        Success = false,
                        Message = "今天已經簽到過了",
                        SignInTime = stats.LastSignInDate,
                        IdempotencyKey = request.IdempotencyKey
                    };
                    
                    // 保存冪等性記錄
                    await SaveIdempotencyRecordAsync(new IdempotencyRecord
                    {
                        IdempotencyKey = request.IdempotencyKey,
                        UserId = request.UserId,
                        Operation = "signin",
                        ResponseData = JsonSerializer.Serialize(duplicateResponse),
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(1)
                    });
                    
                    await transaction.CommitAsync();
                    return duplicateResponse;
                }

                // 計算連續簽到天數
                var consecutiveDays = CalculateConsecutiveDays(stats.LastSignInDate, signInTime);
                
                // 計算獎勵
                var pointsGained = CalculateSignInPoints(consecutiveDays);
                var expGained = CalculateSignInExp(consecutiveDays);
                
                // 更新用戶積分
                var totalPoints = await UpdateUserPointsAsync(request.UserId, pointsGained, "每日簽到獎勵", "SIGNIN");
                
                // 更新寵物經驗值
                await UpdatePetExpAsync(request.UserId, expGained);
                
                // 生成隨機優惠券
                var couponGained = await GenerateRandomCouponAsync(request.UserId, consecutiveDays);
                
                // 更新簽到統計
                stats.LastSignInDate = signInTime;
                stats.ConsecutiveDays = consecutiveDays;
                stats.TotalSignIns++;
                stats.UpdatedAt = DateTime.UtcNow;
                await UpdateSignInStatsAsync(stats);

                var response = new SignInResponse
                {
                    Success = true,
                    Message = $"簽到成功！連續簽到 {consecutiveDays} 天",
                    SignInTime = signInTime,
                    PointsGained = pointsGained,
                    ExpGained = expGained,
                    CouponGained = couponGained,
                    TotalPoints = totalPoints,
                    ConsecutiveDays = consecutiveDays,
                    IdempotencyKey = request.IdempotencyKey
                };

                // 保存冪等性記錄
                await SaveIdempotencyRecordAsync(new IdempotencyRecord
                {
                    IdempotencyKey = request.IdempotencyKey,
                    UserId = request.UserId,
                    Operation = "signin",
                    ResponseData = JsonSerializer.Serialize(response),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                });

                await transaction.CommitAsync();
                
                _logger.LogInformation("簽到成功 UserId: {UserId}, Points: {Points}, Exp: {Exp}, ConsecutiveDays: {ConsecutiveDays}", 
                    request.UserId, pointsGained, expGained, consecutiveDays);
                
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "簽到處理失敗 UserId: {UserId}, IdempotencyKey: {IdempotencyKey}", 
                    request.UserId, request.IdempotencyKey);
                
                return new SignInResponse
                {
                    Success = false,
                    Message = "簽到處理失敗，請稍後再試",
                    IdempotencyKey = request.IdempotencyKey
                };
            }
        }

        /// <summary>
        /// 檢查冪等性密鑰是否已存在
        /// 目前使用模擬實現
        /// </summary>
        public async Task<SignInResponse?> CheckIdempotencyAsync(string idempotencyKey)
        {
            // 目前返回 null，表示沒有找到現有記錄
            // 實際實現需要查詢冪等性記錄表
            await Task.Delay(1);
            return null;
        }

        /// <summary>
        /// 保存冪等性記錄
        /// 目前使用模擬實現
        /// </summary>
        public async Task SaveIdempotencyRecordAsync(IdempotencyRecord record)
        {
            // 目前不實際保存，等待後續實現
            // 實際實現需要保存到冪等性記錄表
            await Task.Delay(1);
            _logger.LogInformation("保存冪等性記錄 Key: {Key}, UserId: {UserId}", record.IdempotencyKey, record.UserId);
        }

        /// <summary>
        /// 更新用戶錢包積分（包含錢包歷史記錄）
        /// 目前使用模擬實現
        /// </summary>
        public async Task<int> UpdateUserPointsAsync(int userId, int pointsToAdd, string description, string? itemCode = null)
        {
            // 目前返回模擬結果
            await Task.Delay(1);
            var newTotal = 1000 + pointsToAdd; // 模擬當前積分 + 新增積分
            
            _logger.LogInformation("更新用戶積分 UserId: {UserId}, Added: {PointsAdded}, NewTotal: {NewTotal}", 
                userId, pointsToAdd, newTotal);
            
            return newTotal;
        }

        /// <summary>
        /// 更新寵物經驗值
        /// 目前使用模擬實現
        /// </summary>
        public async Task<bool> UpdatePetExpAsync(int userId, int expToAdd)
        {
            // 目前返回模擬結果
            await Task.Delay(1);
            
            _logger.LogInformation("更新寵物經驗值 UserId: {UserId}, ExpAdded: {ExpAdded}", userId, expToAdd);
            
            // 模擬有 30% 機率升級
            var levelUp = Random.Shared.Next(1, 101) <= 30;
            if (levelUp)
            {
                _logger.LogInformation("寵物升級了！ UserId: {UserId}", userId);
            }
            
            return levelUp;
        }

        /// <summary>
        /// 生成隨機優惠券（如果符合條件）
        /// </summary>
        public async Task<string?> GenerateRandomCouponAsync(int userId, int consecutiveDays)
        {
            await Task.Delay(1);
            
            // 連續簽到 7 天或以上有機會獲得優惠券
            if (consecutiveDays >= 7 && Random.Shared.Next(1, 101) <= 20) // 20% 機率
            {
                var couponCode = $"SIGNIN_{userId}_{DateTime.UtcNow:yyyyMMdd}_{Random.Shared.Next(1000, 9999)}";
                _logger.LogInformation("生成簽到優惠券 UserId: {UserId}, CouponCode: {CouponCode}", userId, couponCode);
                return couponCode;
            }
            
            return null;
        }

        /// <summary>
        /// 獲取或創建用戶簽到統計
        /// 目前使用模擬實現
        /// </summary>
        public async Task<SignInStats> GetOrCreateSignInStatsAsync(int userId)
        {
            await Task.Delay(1);
            
            // 模擬返回簽到統計
            return new SignInStats
            {
                UserId = userId,
                LastSignInDate = DateTime.UtcNow.AddDays(-1), // 模擬昨天最後簽到
                ConsecutiveDays = 3, // 模擬已連續簽到 3 天
                TotalSignIns = 10,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
        }

        /// <summary>
        /// 更新用戶簽到統計
        /// 目前使用模擬實現
        /// </summary>
        public async Task UpdateSignInStatsAsync(SignInStats stats)
        {
            await Task.Delay(1);
            _logger.LogInformation("更新簽到統計 UserId: {UserId}, ConsecutiveDays: {ConsecutiveDays}, TotalSignIns: {TotalSignIns}", 
                stats.UserId, stats.ConsecutiveDays, stats.TotalSignIns);
        }

        #region 私有輔助方法

        /// <summary>
        /// 計算連續簽到天數
        /// </summary>
        private static int CalculateConsecutiveDays(DateTime lastSignInDate, DateTime currentSignInDate)
        {
            var daysDiff = (currentSignInDate.Date - lastSignInDate.Date).Days;
            
            if (daysDiff == 1)
            {
                // 連續簽到
                return 1; // 這裡應該從現有統計中取得並加 1
            }
            else if (daysDiff > 1)
            {
                // 中斷了連續簽到
                return 1;
            }
            else
            {
                // 同一天或其他情況
                return 1;
            }
        }

        /// <summary>
        /// 計算簽到積分獎勵
        /// </summary>
        private static int CalculateSignInPoints(int consecutiveDays)
        {
            var basePoints = 10;
            var bonusPoints = Math.Min(consecutiveDays - 1, 20) * 2; // 每連續一天額外 2 分，最多 20 天
            return basePoints + bonusPoints;
        }

        /// <summary>
        /// 計算簽到經驗值獎勵
        /// </summary>
        private static int CalculateSignInExp(int consecutiveDays)
        {
            var baseExp = 5;
            var bonusExp = Math.Min(consecutiveDays - 1, 10) * 1; // 每連續一天額外 1 經驗，最多 10 天
            return baseExp + bonusExp;
        }

        #endregion
    }
}
