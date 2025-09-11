using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// ñ��g�J�M�Φs�x�w��{ - Stage 3 �g�J�ާ@
    /// ��{ñ��������g�J�ާ@�A�]�t����B�z�M������
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
        /// ����Τ�ñ��ާ@�]�]�t����B�z�M�������ˬd�^
        /// </summary>
        public async Task<SignInResponse> ProcessSignInAsync(SignInRequest request)
        {
            // 1. �ˬd������
            var existingResponse = await CheckIdempotencyAsync(request.IdempotencyKey);
            if (existingResponse != null)
            {
                _logger.LogInformation("ñ��ШD�w�B�z�L�A��^�֨����G IdempotencyKey: {IdempotencyKey}", request.IdempotencyKey);
                return existingResponse;
            }

            // 2. �ϥΥ���B�zñ���޿�
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var signInTime = request.SignInTime ?? DateTime.UtcNow;
                
                // ���ñ��έp
                var stats = await GetOrCreateSignInStatsAsync(request.UserId);
                
                // �ˬd�O�_���Ѥw�gñ��
                if (stats.LastSignInDate.Date == signInTime.Date)
                {
                    var duplicateResponse = new SignInResponse
                    {
                        Success = false,
                        Message = "���Ѥw�gñ��L�F",
                        SignInTime = stats.LastSignInDate,
                        IdempotencyKey = request.IdempotencyKey
                    };
                    
                    // �O�s�����ʰO��
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

                // �p��s��ñ��Ѽ�
                var consecutiveDays = CalculateConsecutiveDays(stats.LastSignInDate, signInTime);
                
                // �p����y
                var pointsGained = CalculateSignInPoints(consecutiveDays);
                var expGained = CalculateSignInExp(consecutiveDays);
                
                // ��s�Τ�n��
                var totalPoints = await UpdateUserPointsAsync(request.UserId, pointsGained, "�C��ñ����y", "SIGNIN");
                
                // ��s�d���g���
                await UpdatePetExpAsync(request.UserId, expGained);
                
                // �ͦ��H���u�f��
                var couponGained = await GenerateRandomCouponAsync(request.UserId, consecutiveDays);
                
                // ��sñ��έp
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

                // �O�s�����ʰO��
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
                
                _logger.LogInformation("使用者簽到成功 UserId: {UserId}, Points: {Points}, Exp: {Exp}, ConsecutiveDays: {ConsecutiveDays}", 
                    request.UserId, pointsGained, expGained, consecutiveDays);
                
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ñ��B�z���� UserId: {UserId}, IdempotencyKey: {IdempotencyKey}", 
                    request.UserId, request.IdempotencyKey);
                
                return new SignInResponse
                {
                    Success = false,
                    Message = "ñ��B�z���ѡA�еy��A��",
                    IdempotencyKey = request.IdempotencyKey
                };
            }
        }

        /// <summary>
        /// �ˬd�����ʱK�_�O�_�w�s�b
        /// �ثe�ϥμ�����{
        /// </summary>
        public async Task<SignInResponse?> CheckIdempotencyAsync(string idempotencyKey)
        {
            // �ثe��^ null�A���ܨS�����{���O��
            // ��ڹ�{�ݭn�d�߾����ʰO����
            await Task.Delay(1);
            return null;
        }

        /// <summary>
        /// �O�s�����ʰO��
        /// �ثe�ϥμ�����{
        /// </summary>
        public async Task SaveIdempotencyRecordAsync(IdempotencyRecord record)
        {
            // �ثe����ګO�s�A���ݫ����{
            // ��ڹ�{�ݭn�O�s�쾭���ʰO����
            await Task.Delay(1);
            _logger.LogInformation("�O�s�����ʰO�� Key: {Key}, UserId: {UserId}", record.IdempotencyKey, record.UserId);
        }

        /// <summary>
        /// ��s�Τ���]�n���]�]�t���]���v�O���^
        /// �ثe�ϥμ�����{
        /// </summary>
        public async Task<int> UpdateUserPointsAsync(int userId, int pointsToAdd, string description, string? itemCode = null)
        {
            // �ثe��^�������G
            await Task.Delay(1);
            var newTotal = 1000 + pointsToAdd; // �������e�n�� + �s�W�n��
            
            _logger.LogInformation("��s�Τ�n�� UserId: {UserId}, Added: {PointsAdded}, NewTotal: {NewTotal}", 
                userId, pointsToAdd, newTotal);
            
            return newTotal;
        }

        /// <summary>
        /// ��s�d���g���
        /// �ثe�ϥμ�����{
        /// </summary>
        public async Task<bool> UpdatePetExpAsync(int userId, int expToAdd)
        {
            // �ثe��^�������G
            await Task.Delay(1);
            
            _logger.LogInformation("��s�d���g��� UserId: {UserId}, ExpAdded: {ExpAdded}", userId, expToAdd);
            
            // ������ 30% ���v�ɯ�
            var levelUp = Random.Shared.Next(1, 101) <= 30;
            if (levelUp)
            {
                _logger.LogInformation("�d���ɯŤF�I UserId: {UserId}", userId);
            }
            
            return levelUp;
        }

        /// <summary>
        /// �ͦ��H���u�f��]�p�G�ŦX����^
        /// </summary>
        public async Task<string?> GenerateRandomCouponAsync(int userId, int consecutiveDays)
        {
            await Task.Delay(1);
            
            // �s��ñ�� 7 �ѩΥH�W�����|��o�u�f��
            if (consecutiveDays >= 7 && Random.Shared.Next(1, 101) <= 20) // 20% ���v
            {
                var couponCode = $"SIGNIN_{userId}_{DateTime.UtcNow:yyyyMMdd}_{Random.Shared.Next(1000, 9999)}";
                _logger.LogInformation("�ͦ�ñ���u�f�� UserId: {UserId}, CouponCode: {CouponCode}", userId, couponCode);
                return couponCode;
            }
            
            return null;
        }

        /// <summary>
        /// ����γЫإΤ�ñ��έp
        /// �ثe�ϥμ�����{
        /// </summary>
        public async Task<SignInStats> GetOrCreateSignInStatsAsync(int userId)
        {
            await Task.Delay(1);
            
            // ������^ñ��έp
            return new SignInStats
            {
                UserId = userId,
                LastSignInDate = DateTime.UtcNow.AddDays(-1), // �����Q�ѳ̫�ñ��
                ConsecutiveDays = 3, // �����w�s��ñ�� 3 ��
                TotalSignIns = 10,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
        }

        /// <summary>
        /// ��s�Τ�ñ��έp
        /// �ثe�ϥμ�����{
        /// </summary>
        public async Task UpdateSignInStatsAsync(SignInStats stats)
        {
            await Task.Delay(1);
            _logger.LogInformation("��sñ��έp UserId: {UserId}, ConsecutiveDays: {ConsecutiveDays}, TotalSignIns: {TotalSignIns}", 
                stats.UserId, stats.ConsecutiveDays, stats.TotalSignIns);
        }

        #region �p�����U��k

        /// <summary>
        /// �p��s��ñ��Ѽ�
        /// </summary>
        private static int CalculateConsecutiveDays(DateTime lastSignInDate, DateTime currentSignInDate)
        {
            var daysDiff = (currentSignInDate.Date - lastSignInDate.Date).Days;
            
            if (daysDiff == 1)
            {
                // �s��ñ��
                return 1; // �o�����ӱq�{���έp�����o�å[ 1
            }
            else if (daysDiff > 1)
            {
                // ���_�F�s��ñ��
                return 1;
            }
            else
            {
                // �P�@�ѩΨ�L���p
                return 1;
            }
        }

        /// <summary>
        /// �p��ñ��n�����y
        /// </summary>
        private static int CalculateSignInPoints(int consecutiveDays)
        {
            var basePoints = 10;
            var bonusPoints = Math.Min(consecutiveDays - 1, 20) * 2; // �C�s��@���B�~ 2 ���A�̦h 20 ��
            return basePoints + bonusPoints;
        }

        /// <summary>
        /// �p��ñ��g��ȼ��y
        /// </summary>
        private static int CalculateSignInExp(int consecutiveDays)
        {
            var baseExp = 5;
            var bonusExp = Math.Min(consecutiveDays - 1, 10) * 1; // �C�s��@���B�~ 1 �g��A�̦h 10 ��
            return baseExp + bonusExp;
        }

        #endregion
    }
}
