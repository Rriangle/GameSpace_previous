using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// ���]Ū���M�Φs�x�w���� - Stage 2 �s�פ���
    /// ���ѿ��]�`���E�X�d�ߥ\��
    /// </summary>
    public interface IWalletReadOnlyRepository
    {
        /// <summary>
        /// ���o�Τ���]�`���]�]�t�n���B�u�f��B�q�l§��E�X��T�^
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <returns>���]�`��Ū���ҫ�</returns>
        Task<WalletOverviewReadModel?> GetWalletOverviewAsync(int userId);

        /// <summary>
        /// ���o�Τ��e�n���l�B
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <returns>�n���l�B</returns>
        Task<int> GetUserPointsAsync(int userId);

        /// <summary>
        /// ���o�Τ���]���ʾ��v�]�����^
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="pageIndex">���Ư��ޡ]�q 0 �}�l�^</param>
        /// <param name="pageSize">�C������</param>
        /// <returns>���]���ʾ��v�C��</returns>
        Task<List<WalletHistoryReadModel>> GetWalletHistoryAsync(int userId, int pageIndex = 0, int pageSize = 10);

        /// <summary>
        /// ���o�Τ�i���u�f��C��
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <returns>�i���u�f��C��</returns>
        Task<List<CouponOverviewReadModel>> GetAvailableCouponsAsync(int userId);

        /// <summary>
        /// ���o�Τ�i�ιq�l§��C��
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <returns>�i�ιq�l§��C��</returns>
        Task<List<EVoucherOverviewReadModel>> GetAvailableEVouchersAsync(int userId);
    }
}
