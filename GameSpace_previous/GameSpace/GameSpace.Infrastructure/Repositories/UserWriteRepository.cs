using Microsoft.EntityFrameworkCore;
using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using System.Transactions;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// 用戶相關寫入存儲庫實現
    /// </summary>
    public class UserWriteRepository : IUserWriteRepository
    {
        private readonly GameSpaceDbContext _context;

        public UserWriteRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 處理用戶簽到
        /// </summary>
        public async Task<SignInResponse> ProcessSignInAsync(SignInRequest request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // 檢查冪等性金鑰
                if (await IsIdempotencyKeyUsedAsync(request.IdempotencyKey))
                {
                    return new SignInResponse
                    {
                        Success = false,
                        Message = "簽到請求已處理過，請勿重複提交"
                    };
                }

                // 獲取用戶資訊
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return new SignInResponse
                    {
                        Success = false,
                        Message = "用戶不存在"
                    };
                }

                // 獲取或創建錢包
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(w => w.UserID == request.UserId);
                
                if (wallet == null)
                {
                    wallet = new UserWallet
                    {
                        UserID = request.UserId,
                        Points = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.UserWallets.Add(wallet);
                }

                // 獲取或創建簽到統計
                var signInStats = await _context.UserSignInStats
                    .FirstOrDefaultAsync(s => s.UserID == request.UserId);
                
                if (signInStats == null)
                {
                    signInStats = new UserSignInStats
                    {
                        UserID = request.UserId,
                        ConsecutiveDays = 0,
                        SignInDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Status = "active"
                    };
                    _context.UserSignInStats.Add(signInStats);
                }

                // 計算獎勵
                var pointsEarned = 100; // 基礎點數
                var expEarned = 50; // 基礎經驗值
                var consecutiveDays = 1;
                var hasBonusReward = false;
                var bonusDescription = "";

                // 檢查連續簽到
                if (signInStats.SignInDate.Date == DateTime.Today)
                {
                    return new SignInResponse
                    {
                        Success = false,
                        Message = "今日已簽到，請明天再來"
                    };
                }
                else if (signInStats.SignInDate.Date == DateTime.Today.AddDays(-1))
                {
                    consecutiveDays = signInStats.ConsecutiveDays + 1;
                }

                // 連續簽到獎勵
                if (consecutiveDays >= 7)
                {
                    pointsEarned += 200;
                    expEarned += 100;
                    hasBonusReward = true;
                    bonusDescription = "連續簽到7天獎勵！";
                }
                else if (consecutiveDays >= 3)
                {
                    pointsEarned += 50;
                    expEarned += 25;
                    hasBonusReward = true;
                    bonusDescription = "連續簽到3天獎勵！";
                }

                // 更新錢包
                wallet.Points += pointsEarned;
                wallet.UpdatedAt = DateTime.Now;

                // 更新簽到統計
                signInStats.ConsecutiveDays = consecutiveDays;
                signInStats.SignInDate = DateTime.Now;
                signInStats.PointsEarned = pointsEarned;
                signInStats.UpdatedAt = DateTime.Now;
                signInStats.LastUpdated = DateTime.Now;

                // 添加錢包歷史記錄
                await AddWalletHistoryAsync(request.UserId, pointsEarned, 
                    $"簽到獎勵 (連續{consecutiveDays}天)", "signin");

                // 更新寵物經驗值
                await UpdatePetExpAsync(request.UserId, expEarned);

                // 保存變更
                await _context.SaveChangesAsync();
                scope.Complete();

                return new SignInResponse
                {
                    Success = true,
                    Message = "簽到成功！",
                    PointsEarned = pointsEarned,
                    ExpEarned = expEarned,
                    ConsecutiveDays = consecutiveDays,
                    HasBonusReward = hasBonusReward,
                    BonusDescription = bonusDescription
                };
            }
            catch (Exception ex)
            {
                return new SignInResponse
                {
                    Success = false,
                    Message = $"簽到處理失敗: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新用戶錢包
        /// </summary>
        public async Task<bool> UpdateUserWalletAsync(int userId, int pointsChange, string description)
        {
            try
            {
                var wallet = await _context.UserWallets
                    .FirstOrDefaultAsync(w => w.UserID == userId);
                
                if (wallet == null)
                {
                    wallet = new UserWallet
                    {
                        UserID = userId,
                        Points = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.UserWallets.Add(wallet);
                }

                wallet.Points += pointsChange;
                wallet.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 添加錢包歷史記錄
        /// </summary>
        public async Task<bool> AddWalletHistoryAsync(int userId, int pointsChange, string description, string transactionType)
        {
            try
            {
                var history = new WalletHistory
                {
                    UserID = userId,
                    PointsChanged = pointsChange,
                    Description = description,
                    ChangeType = transactionType,
                    ChangeTime = DateTime.Now
                };

                _context.WalletHistory.Add(history);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新寵物經驗值和等級
        /// </summary>
        public async Task<bool> UpdatePetExpAsync(int userId, int expGained)
        {
            try
            {
                var pet = await _context.Pets
                    .FirstOrDefaultAsync(p => p.UserID == userId);
                
                if (pet == null)
                {
                    // 創建新寵物
                    pet = new Pet
                    {
                        UserID = userId,
                        PetName = "小夥伴",
                        PetType = "default",
                        Level = 1,
                        Experience = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Pets.Add(pet);
                }

                pet.Experience += expGained;

                // 檢查升級
                var requiredExp = pet.Level * 100;
                while (pet.Experience >= requiredExp)
                {
                    pet.Experience -= requiredExp;
                    pet.Level++;
                    requiredExp = pet.Level * 100;
                }

                pet.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 兌換優惠券
        /// </summary>
        public async Task<bool> RedeemCouponAsync(int userId, int couponId)
        {
            try
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.CouponID == couponId);
                
                if (coupon == null || !coupon.IsActive)
                {
                    return false;
                }

                coupon.IsActive = false;
                coupon.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 兌換禮券
        /// </summary>
        public async Task<bool> RedeemEVoucherAsync(int userId, int evoucherId)
        {
            try
            {
                var evoucher = await _context.EVouchers
                    .FirstOrDefaultAsync(e => e.VoucherID == evoucherId);
                
                if (evoucher == null || !evoucher.IsActive)
                {
                    return false;
                }

                evoucher.IsActive = false;
                evoucher.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查冪等性金鑰是否已使用
        /// </summary>
        public async Task<bool> IsIdempotencyKeyUsedAsync(string idempotencyKey)
        {
            // 這裡可以使用 Redis 或資料庫來存儲冪等性金鑰
            // 簡化實現：檢查錢包歷史記錄中是否有相同的描述
            var exists = await _context.WalletHistory
                .AnyAsync(h => h.Description.Contains(idempotencyKey));
            
            return exists;
        }
    }
}
