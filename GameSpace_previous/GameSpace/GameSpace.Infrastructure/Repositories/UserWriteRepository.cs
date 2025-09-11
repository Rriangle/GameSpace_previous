using Microsoft.EntityFrameworkCore;
using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using System.Transactions;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// �Τ�����g�J�s�x�w��{
    /// </summary>
    public class UserWriteRepository : IUserWriteRepository
    {
        private readonly GameSpaceDbContext _context;

        public UserWriteRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// �B�z�Τ�ñ��
        /// </summary>
        public async Task<SignInResponse> ProcessSignInAsync(SignInRequest request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // �ˬd�����ʪ��_
                if (await IsIdempotencyKeyUsedAsync(request.IdempotencyKey))
                {
                    return new SignInResponse
                    {
                        Success = false,
                        Message = "ñ��ШD�w�B�z�L�A�Фŭ��ƴ���"
                    };
                }

                // ����Τ��T
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return new SignInResponse
                    {
                        Success = false,
                        Message = "�Τᤣ�s�b"
                    };
                }

                // ����γЫؿ��]
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

                // ����γЫ�ñ��έp
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

                // �p����y
                var pointsEarned = 100; // ��¦�I��
                var expEarned = 50; // ��¦�g���
                var consecutiveDays = 1;
                var hasBonusReward = false;
                var bonusDescription = "";

                // �ˬd�s��ñ��
                if (signInStats.SignInDate.Date == DateTime.Today)
                {
                    return new SignInResponse
                    {
                        Success = false,
                        Message = "����wñ��A�Щ��ѦA��"
                    };
                }
                else if (signInStats.SignInDate.Date == DateTime.Today.AddDays(-1))
                {
                    consecutiveDays = signInStats.ConsecutiveDays + 1;
                }

                // �s��ñ����y
                if (consecutiveDays >= 7)
                {
                    pointsEarned += 200;
                    expEarned += 100;
                    hasBonusReward = true;
                    bonusDescription = "�s��ñ��7�Ѽ��y�I";
                }
                else if (consecutiveDays >= 3)
                {
                    pointsEarned += 50;
                    expEarned += 25;
                    hasBonusReward = true;
                    bonusDescription = "�s��ñ��3�Ѽ��y�I";
                }

                // ��s���]
                wallet.Points += pointsEarned;
                wallet.UpdatedAt = DateTime.Now;

                // ��sñ��έp
                signInStats.ConsecutiveDays = consecutiveDays;
                signInStats.SignInDate = DateTime.Now;
                signInStats.PointsEarned = pointsEarned;
                signInStats.UpdatedAt = DateTime.Now;
                signInStats.LastUpdated = DateTime.Now;

                // �K�[���]���v�O��
                await AddWalletHistoryAsync(request.UserId, pointsEarned, 
                    $"ñ����y (�s��{consecutiveDays}��)", "signin");

                // ��s�d���g���
                await UpdatePetExpAsync(request.UserId, expEarned);

                // �O�s�ܧ�
                await _context.SaveChangesAsync();
                scope.Complete();

                return new SignInResponse
                {
                    Success = true,
                    Message = "簽到成功",
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
                    Message = $"ñ��B�z����: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// ��s�Τ���]
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
        /// �K�[���]���v�O��
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
        /// ��s�d���g��ȩM����
        /// </summary>
        public async Task<bool> UpdatePetExpAsync(int userId, int expGained)
        {
            try
            {
                var pet = await _context.Pets
                    .FirstOrDefaultAsync(p => p.UserID == userId);
                
                if (pet == null)
                {
                    // �Ыطs�d��
                    pet = new Pet
                    {
                        UserID = userId,
                        PetName = "�p�٦�",
                        PetType = "default",
                        Level = 1,
                        Experience = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Pets.Add(pet);
                }

                pet.Experience += expGained;

                // �ˬd�ɯ�
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
        /// �I���u�f��
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
        /// �I��§��
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
        /// �ˬd�����ʪ��_�O�_�w�ϥ�
        /// </summary>
        public async Task<bool> IsIdempotencyKeyUsedAsync(string idempotencyKey)
        {
            // �o�̥i�H�ϥ� Redis �θ�Ʈw�Ӧs�x�����ʪ��_
            // ²�ƹ�{�G�ˬd���]���v�O�����O�_���ۦP���y�z
            var exists = await _context.WalletHistory
                .AnyAsync(h => h.Description.Contains(idempotencyKey));
            
            return exists;
        }
    }
}
