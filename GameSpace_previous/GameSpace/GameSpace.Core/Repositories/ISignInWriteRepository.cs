using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// ñ��g�J�M�Φs�x�w���� - Stage 3 �g�J�ާ@
    /// ����ñ��������g�J�ާ@�A�]�t����B�z�M������
    /// </summary>
    public interface ISignInWriteRepository
    {
        /// <summary>
        /// ����Τ�ñ��ާ@�]�]�t����B�z�M�������ˬd�^
        /// </summary>
        /// <param name="request">ñ��ШD</param>
        /// <returns>ñ���T��</returns>
        Task<SignInResponse> ProcessSignInAsync(SignInRequest request);

        /// <summary>
        /// �ˬd�����ʱK�_�O�_�w�s�b
        /// </summary>
        /// <param name="idempotencyKey">�����ʱK�_</param>
        /// <returns>�p�G�s�b�h��^���e���T���A�_�h��^ null</returns>
        Task<SignInResponse?> CheckIdempotencyAsync(string idempotencyKey);

        /// <summary>
        /// �O�s�����ʰO��
        /// </summary>
        /// <param name="record">�����ʰO��</param>
        Task SaveIdempotencyRecordAsync(IdempotencyRecord record);

        /// <summary>
        /// ��s�Τ���]�n���]�]�t���]���v�O���^
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="pointsToAdd">�n�W�[���n��</param>
        /// <param name="description">�y�z</param>
        /// <param name="itemCode">���إN�X</param>
        /// <returns>��s�᪺�`�n��</returns>
        Task<int> UpdateUserPointsAsync(int userId, int pointsToAdd, string description, string? itemCode = null);

        /// <summary>
        /// ��s�d���g���
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="expToAdd">�n�W�[���g���</param>
        /// <returns>�O�_�ɯ�</returns>
        Task<bool> UpdatePetExpAsync(int userId, int expToAdd);

        /// <summary>
        /// �ͦ��H���u�f��]�p�G�ŦX����^
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="consecutiveDays">�s��ñ��Ѽ�</param>
        /// <returns>�u�f��N�X�A�p�G�S���h��^ null</returns>
        Task<string?> GenerateRandomCouponAsync(int userId, int consecutiveDays);

        /// <summary>
        /// ����γЫإΤ�ñ��έp
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <returns>ñ��έp</returns>
        Task<SignInStats> GetOrCreateSignInStatsAsync(int userId);

        /// <summary>
        /// ��s�Τ�ñ��έp
        /// </summary>
        /// <param name="stats">ñ��έp</param>
        Task UpdateSignInStatsAsync(SignInStats stats);
    }
}
