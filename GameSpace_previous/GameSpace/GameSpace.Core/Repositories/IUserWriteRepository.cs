using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// �Τ�����g�J�s�x�w
    /// </summary>
    public interface IUserWriteRepository
    {
        /// <summary>
        /// �B�z�Τ�ñ��
        /// </summary>
        Task<SignInResponse> ProcessSignInAsync(SignInRequest request);

        /// <summary>
        /// ��s�Τ���]
        /// </summary>
        Task<bool> UpdateUserWalletAsync(int userId, int pointsChange, string description);

        /// <summary>
        /// �K�[���]���v�O��
        /// </summary>
        Task<bool> AddWalletHistoryAsync(int userId, int pointsChange, string description, string transactionType);

        /// <summary>
        /// ��s�d���g��ȩM����
        /// </summary>
        Task<bool> UpdatePetExpAsync(int userId, int expGained);

        /// <summary>
        /// �I���u�f��
        /// </summary>
        Task<bool> RedeemCouponAsync(int userId, int couponId);

        /// <summary>
        /// �I��§��
        /// </summary>
        Task<bool> RedeemEVoucherAsync(int userId, int evoucherId);

        /// <summary>
        /// �ˬd�����ʪ��_�O�_�w�ϥ�
        /// </summary>
        Task<bool> IsIdempotencyKeyUsedAsync(string idempotencyKey);
    }
}
