using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// �Ʀ�]Ū���M�Φs�x�w��{ - Stage 2 �s�פ���
    /// ��{�Ʀ�]�E�X�d���޿�
    /// </summary>
    public class LeaderboardReadOnlyRepository : ILeaderboardReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public LeaderboardReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ���o�Ʀ�]�`���]�]�t�C��B�C�g�B�C��Ʀ�]�^
        /// </summary>
        public async Task<LeaderboardOverviewReadModel> GetLeaderboardOverviewAsync(int? userId = null)
        {
            // �æ�d�ߦU�ɶ��g�����Ʀ�]
            var dailyTask = GetLeaderboardByPeriodAsync("daily", limit: 10);
            var weeklyTask = GetLeaderboardByPeriodAsync("weekly", limit: 10);
            var monthlyTask = GetLeaderboardByPeriodAsync("monthly", limit: 10);

            Task<UserRankingInfo?> userRankingTask = Task.FromResult<UserRankingInfo?>(null);
            if (userId.HasValue)
            {
                userRankingTask = GetUserRankingInfoAsync(userId.Value);
            }

            await Task.WhenAll(dailyTask, weeklyTask, monthlyTask, userRankingTask);

            return new LeaderboardOverviewReadModel
            {
                DailyLeaderboard = await dailyTask,
                WeeklyLeaderboard = await weeklyTask,
                MonthlyLeaderboard = await monthlyTask,
                MyRanking = await userRankingTask,
                LastUpdated = DateTime.UtcNow
            };
        }

        /// <summary>
        /// ���o���w�ɶ��g�����Ʀ�]
        /// �ثe��^������ơA���ݫ��򧹾��{
        /// </summary>
        public async Task<List<LeaderboardEntryReadModel>> GetLeaderboardByPeriodAsync(string period, int? gameId = null, int limit = 50)
        {
            // �ثe��^������ơA���ݫ��򧹾��{
            // �ݭn�ھڹ�ڪ���Ʈw schema �M DbContext �վ�d���޿�
            await Task.Delay(1); // �������B�ާ@

            return new List<LeaderboardEntryReadModel>
            {
                new LeaderboardEntryReadModel
                {
                    Rank = 1,
                    UserId = 1001,
                    UserName = "���y���a1",
                    GameId = gameId ?? 1,
                    GameName = "�d�ҹC��",
                    IndexValue = 9999,
                    Period = period,
                    SnapshotTime = DateTime.UtcNow.AddHours(-1)
                },
                new LeaderboardEntryReadModel
                {
                    Rank = 2,
                    UserId = 1002,
                    UserName = "���y���a2",
                    GameId = gameId ?? 1,
                    GameName = "�d�ҹC��",
                    IndexValue = 8888,
                    Period = period,
                    SnapshotTime = DateTime.UtcNow.AddHours(-1)
                }
            };
        }

        /// <summary>
        /// ���o���w�C�����Ʀ�]
        /// </summary>
        public async Task<GameLeaderboardReadModel?> GetGameLeaderboardAsync(int gameId, string period, int limit = 50)
        {
            var entries = await GetLeaderboardByPeriodAsync(period, gameId, limit);

            return new GameLeaderboardReadModel
            {
                GameId = gameId,
                GameName = $"�C�� {gameId}",
                GameDescription = "�d�ҹC���y�z",
                Entries = entries,
                Period = period,
                TotalParticipants = 100,
                LastUpdated = DateTime.UtcNow
            };
        }

        /// <summary>
        /// ���o�Τ�b�U�ɶ��g�����ƦW��T
        /// </summary>
        public async Task<UserRankingInfo?> GetUserRankingInfoAsync(int userId)
        {
            var dailyRankTask = GetUserGameRankingAsync(userId, 0, "daily");
            var weeklyRankTask = GetUserGameRankingAsync(userId, 0, "weekly");
            var monthlyRankTask = GetUserGameRankingAsync(userId, 0, "monthly");

            await Task.WhenAll(dailyRankTask, weeklyRankTask, monthlyRankTask);

            return new UserRankingInfo
            {
                UserId = userId,
                DailyRank = await dailyRankTask,
                WeeklyRank = await weeklyRankTask,
                MonthlyRank = await monthlyRankTask
            };
        }

        /// <summary>
        /// ���o�Τ�b���w�C���M�ɶ��g�����ƦW
        /// </summary>
        public async Task<RankingDetail?> GetUserGameRankingAsync(int userId, int gameId, string period)
        {
            // �ثe��^�������
            await Task.Delay(1);

            return new RankingDetail
            {
                Rank = 42,
                IndexValue = 1234,
                SnapshotTime = DateTime.UtcNow.AddHours(-2)
            };
        }

        /// <summary>
        /// ���o�Ʀ�]���v�Ͷա]�Τ�b���w�C�����ƦW�ܤơ^
        /// </summary>
        public async Task<List<RankingDetail>> GetUserRankingHistoryAsync(int userId, int gameId, string period, int days = 30)
        {
            // �ثe��^�ŦC��
            await Task.Delay(1);
            return new List<RankingDetail>();
        }
    }
}
