namespace GameSpace.Models
{
    /// <summary>
    /// �Ʀ�]�`���E�XŪ���ҫ� - Stage 2 �s�פ���
    /// ���ѱƦ�]�����һݪ��E�X��T
    /// </summary>
    public class LeaderboardOverviewReadModel
    {
        /// <summary>
        /// �C��Ʀ�]�]�e 10 �W�^
        /// </summary>
        public List<LeaderboardEntryReadModel> DailyLeaderboard { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// �C�g�Ʀ�]�]�e 10 �W�^
        /// </summary>
        public List<LeaderboardEntryReadModel> WeeklyLeaderboard { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// �C��Ʀ�]�]�e 10 �W�^
        /// </summary>
        public List<LeaderboardEntryReadModel> MonthlyLeaderboard { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// �ڪ��ƦW��T�]�p�G�Τᦳ�n�J�^
        /// </summary>
        public UserRankingInfo? MyRanking { get; set; }

        /// <summary>
        /// �̫��s�ɶ�
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// �Ʀ�]����Ū���ҫ�
    /// </summary>
    public class LeaderboardEntryReadModel
    {
        /// <summary>
        /// �ƦW
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// �Τ� ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// �Τ�W��
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// �Τ�ʺ�
        /// </summary>
        public string? UserNickName { get; set; }

        /// <summary>
        /// �C�� ID
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// �C���W��
        /// </summary>
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// ���мƭȡ]���ơB�g��ȵ��^
        /// </summary>
        public decimal IndexValue { get; set; }

        /// <summary>
        /// �ɶ��g��
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// �ַӮɶ�
        /// </summary>
        public DateTime SnapshotTime { get; set; }
    }

    /// <summary>
    /// �Τ�ƦW��T
    /// </summary>
    public class UserRankingInfo
    {
        /// <summary>
        /// �Τ� ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// �C��ƦW
        /// </summary>
        public RankingDetail? DailyRank { get; set; }

        /// <summary>
        /// �C�g�ƦW
        /// </summary>
        public RankingDetail? WeeklyRank { get; set; }

        /// <summary>
        /// �C��ƦW
        /// </summary>
        public RankingDetail? MonthlyRank { get; set; }
    }

    /// <summary>
    /// �ƦW�Ա�
    /// </summary>
    public class RankingDetail
    {
        public int Rank { get; set; }
        public decimal IndexValue { get; set; }
        public DateTime SnapshotTime { get; set; }
    }

    /// <summary>
    /// �C���Ʀ�]�E�XŪ���ҫ�
    /// ���ѯS�w�C�����Ʀ�]��T
    /// </summary>
    public class GameLeaderboardReadModel
    {
        /// <summary>
        /// �C�� ID
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// �C���W��
        /// </summary>
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// �C���y�z
        /// </summary>
        public string? GameDescription { get; set; }

        /// <summary>
        /// �Ʀ�]���ئC��
        /// </summary>
        public List<LeaderboardEntryReadModel> Entries { get; set; } = new List<LeaderboardEntryReadModel>();

        /// <summary>
        /// �ɶ��g��
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// �`�ѻP�H��
        /// </summary>
        public int TotalParticipants { get; set; }

        /// <summary>
        /// �̫��s�ɶ�
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
