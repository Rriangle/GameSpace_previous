using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// �Ʀ�]Ū���M�Φs�x�w���� - Stage 2 �s�פ���
    /// ���ѱƦ�]�E�X�d�ߥ\��
    /// </summary>
    public interface ILeaderboardReadOnlyRepository
    {
        /// <summary>
        /// ���o�Ʀ�]�`���]�]�t�C��B�C�g�B�C��Ʀ�]�^
        /// </summary>
        /// <param name="userId">�Τ� ID�]�i��A�Ω�d�߭ӤH�ƦW�^</param>
        /// <returns>�Ʀ�]�`���E�X�ҫ�</returns>
        Task<LeaderboardOverviewReadModel> GetLeaderboardOverviewAsync(int? userId = null);

        /// <summary>
        /// ���o���w�ɶ��g�����Ʀ�]
        /// </summary>
        /// <param name="period">�ɶ��g���]daily, weekly, monthly�^</param>
        /// <param name="gameId">�C�� ID�]�i��^</param>
        /// <param name="limit">���o�ƶq����]�w�] 50�^</param>
        /// <returns>�Ʀ�]���ئC��</returns>
        Task<List<LeaderboardEntryReadModel>> GetLeaderboardByPeriodAsync(string period, int? gameId = null, int limit = 50);

        /// <summary>
        /// ���o���w�C�����Ʀ�]
        /// </summary>
        /// <param name="gameId">�C�� ID</param>
        /// <param name="period">�ɶ��g���]daily, weekly, monthly�^</param>
        /// <param name="limit">���o�ƶq����]�w�] 50�^</param>
        /// <returns>�C���Ʀ�]�E�X�ҫ�</returns>
        Task<GameLeaderboardReadModel?> GetGameLeaderboardAsync(int gameId, string period, int limit = 50);

        /// <summary>
        /// ���o�Τ�b�U�ɶ��g�����ƦW��T
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <returns>�Τ�ƦW��T</returns>
        Task<UserRankingInfo?> GetUserRankingInfoAsync(int userId);

        /// <summary>
        /// ���o�Τ�b���w�C���M�ɶ��g�����ƦW
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="gameId">�C�� ID</param>
        /// <param name="period">�ɶ��g��</param>
        /// <returns>�ƦW�Ա�</returns>
        Task<RankingDetail?> GetUserGameRankingAsync(int userId, int gameId, string period);

        /// <summary>
        /// ���o�Ʀ�]���v�Ͷա]�Τ�b���w�C�����ƦW�ܤơ^
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="gameId">�C�� ID</param>
        /// <param name="period">�ɶ��g��</param>
        /// <param name="days">�d�ߤѼơ]�w�] 30 �ѡ^</param>
        /// <returns>�ƦW���v�C��</returns>
        Task<List<RankingDetail>> GetUserRankingHistoryAsync(int userId, int gameId, string period, int days = 30);
    }
}
