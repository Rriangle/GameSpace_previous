using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// ���]Ū���M�Φs�x�w��{ - Stage 2 �s�פ���
    /// ��{���]�E�X�d���޿�
    /// </summary>
    public class WalletReadOnlyRepository : IWalletReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public WalletReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ���o�Τ���]�`���]�E�X�d�ߡG�n�� + �u�f�� + �q�l§��^
        /// �ثe��^������ơA���ݫ��򧹾��{
        /// </summary>
        public async Task<WalletOverviewReadModel?> GetWalletOverviewAsync(int userId)
        {
            // �ثe��^������ơA���ݫ��򧹾��{
            // �ݭn�ھڹ�ڪ���Ʈw schema �վ�d���޿�
            await Task.Delay(1); // �������B�ާ@

            return new WalletOverviewReadModel
            {
                UserId = userId,
                UserName = $"�Τ�{userId}",
                CurrentPoints = 1000,
                AvailableCouponsCount = 3,
                UsedCouponsCount = 2,
                AvailableEVouchersCount = 1,
                UsedEVouchersCount = 1,
                RecentTransactions = new List<WalletHistoryReadModel>(),
                AvailableCoupons = new List<CouponOverviewReadModel>(),
                AvailableEVouchers = new List<EVoucherOverviewReadModel>()
            };
        }

        /// <summary>
        /// ���o�Τ��e�n���l�B
        /// </summary>
        public async Task<int> GetUserPointsAsync(int userId)
        {
            // �ثe��^������ơA���ݫ��򧹾��{
            await Task.Delay(1); // �������B�ާ@
            return 1000; // �����n��
        }

        /// <summary>
        /// ���o�Τ���]���ʾ��v�]�����A���ɶ��˧ǡ^
        /// </summary>
        public async Task<List<WalletHistoryReadModel>> GetWalletHistoryAsync(int userId, int pageIndex = 0, int pageSize = 10)
        {
            // �ثe��^�ŦC��A���ݫ��򧹾��{
            await Task.Delay(1); // �������B�ާ@
            return new List<WalletHistoryReadModel>();
        }

        /// <summary>
        /// ���o�Τ�i���u�f��C��]���ϥΥB���L���^
        /// </summary>
        public async Task<List<CouponOverviewReadModel>> GetAvailableCouponsAsync(int userId)
        {
            // �ثe��^�ŦC��A���ݫ��򧹾��{
            await Task.Delay(1); // �������B�ާ@
            return new List<CouponOverviewReadModel>();
        }

        /// <summary>
        /// ���o�Τ�i�ιq�l§��C��]���ϥΥB���L���^
        /// </summary>
        public async Task<List<EVoucherOverviewReadModel>> GetAvailableEVouchersAsync(int userId)
        {
            // �ثe��^�ŦC��A���ݫ��򧹾��{
            await Task.Delay(1); // �������B�ާ@
            return new List<EVoucherOverviewReadModel>();
        }
    }
}
