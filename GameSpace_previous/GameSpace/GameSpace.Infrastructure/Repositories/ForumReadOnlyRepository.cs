using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Infrastructure.Repositories
{
    /// <summary>
    /// �׾�Ū���M�Φs�x�w��{ - Stage 2 �s�פ���
    /// ��{�׾»E�X�d���޿�
    /// </summary>
    public class ForumReadOnlyRepository : IForumReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public ForumReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ���o�׾¦C��]�]�t�򥻲έp��T�^
        /// </summary>
        public async Task<List<ForumListReadModel>> GetForumListAsync()
        {
            // �ثe��^�ŦC��A���ݫ����{
            // �o�̻ݭn��{��ڪ��׾¬d���޿�
            return await Task.FromResult(new List<ForumListReadModel>());
        }

        /// <summary>
        /// ���o���w�׾¸Ա��]�]�t�D�D�C��^
        /// </summary>
        public async Task<ForumDetailReadModel?> GetForumDetailAsync(int forumId, int pageIndex = 0, int pageSize = 20)
        {
            // �ثe��^ null�A���ݫ����{
            // �o�̻ݭn��{��ڪ��׾¸Ա��d���޿�
            return await Task.FromResult<ForumDetailReadModel?>(null);
        }

        /// <summary>
        /// ���o���w�D�D�Ա��]�]�t�^�ЦC��^
        /// </summary>
        public async Task<ThreadDetailReadModel?> GetThreadDetailAsync(long threadId, int pageIndex = 0, int pageSize = 20)
        {
            // �ثe��^ null�A���ݫ����{
            // �o�̻ݭn��{��ڪ��D�D�Ա��d���޿�
            return await Task.FromResult<ThreadDetailReadModel?>(null);
        }

        /// <summary>
        /// ���o�Τ᪺�D�D�C��
        /// </summary>
        public async Task<List<ThreadSummaryReadModel>> GetUserThreadsAsync(int userId, int pageIndex = 0, int pageSize = 20)
        {
            // �ثe��^�ŦC��A���ݫ����{
            // �o�̻ݭn��{��ڪ��Τ�D�D�d���޿�
            return await Task.FromResult(new List<ThreadSummaryReadModel>());
        }

        /// <summary>
        /// �j�M�D�D�]�����D�^
        /// </summary>
        public async Task<List<ThreadSummaryReadModel>> SearchThreadsAsync(string keyword, int? forumId = null, int pageIndex = 0, int pageSize = 20)
        {
            // �ثe��^�ŦC��A���ݫ����{
            // �o�̻ݭn��{��ڪ��D�D�j�M�޿�
            return await Task.FromResult(new List<ThreadSummaryReadModel>());
        }
    }
}
