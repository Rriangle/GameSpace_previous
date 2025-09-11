namespace GameSpace.Models
{
    /// <summary>
    /// ñ��^���ҫ� - �w�X�i�� Stage 3 �ݨD
    /// </summary>
    public class SignInResponse
    {
        /// <summary>
        /// �O�_���\
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// �T��
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ��o���I��
        /// </summary>
        public int PointsEarned { get; set; }

        /// <summary>
        /// ��o���g���
        /// </summary>
        public int ExpEarned { get; set; }

        /// <summary>
        /// �s��ñ��Ѽ�
        /// </summary>
        public int ConsecutiveDays { get; set; }

        /// <summary>
        /// �O�_��o�B�~���y
        /// </summary>
        public bool HasBonusReward { get; set; }

        /// <summary>
        /// ���y�y�z
        /// </summary>
        public string BonusDescription { get; set; } = string.Empty;

        // ===== Stage 3 �X�i��� =====

        /// <summary>
        /// ñ��ɶ��]Stage 3 �s�W�^
        /// </summary>
        public DateTime SignInTime { get; set; }

        /// <summary>
        /// ��o���n���]Stage 3 �s�W�A�P PointsEarned �P�q���i�g�J�^
        /// </summary>
        public int PointsGained { get; set; }

        /// <summary>
        /// ��o���g��ȡ]Stage 3 �s�W�A�P ExpEarned �P�q���i�g�J�^
        /// </summary>
        public int ExpGained { get; set; }

        /// <summary>
        /// ��o���u�f��N�X�]�p�G���^�]Stage 3 �s�W�^
        /// </summary>
        public string? CouponGained { get; set; }

        /// <summary>
        /// ñ��᪺�`�n���]Stage 3 �s�W�^
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// �����ʱK�_�]Stage 3 �s�W�^
        /// </summary>
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}
