using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// �׾�Ū���M�Φs�x�w���� - Stage 2 �s�פ���
    /// ���ѽ׾¦C��M�Ա��E�X�d�ߥ\��
    /// </summary>
    public interface IForumReadOnlyRepository
    {
        /// <summary>
        /// ���o�׾¦C��]�]�t�򥻲έp��T�^
        /// </summary>
        /// <returns>�׾¦C��E�X�ҫ�</returns>
        Task<List<ForumListReadModel>> GetForumListAsync();

        /// <summary>
        /// ���o���w�׾¸Ա��]�]�t�D�D�C��^
        /// </summary>
        /// <param name="forumId">�׾� ID</param>
        /// <param name="pageIndex">���Ư��ޡ]�q 0 �}�l�^</param>
        /// <param name="pageSize">�C������</param>
        /// <returns>�׾¸Ա��E�X�ҫ�</returns>
        Task<ForumDetailReadModel?> GetForumDetailAsync(int forumId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// ���o���w�D�D�Ա��]�]�t�^�ЦC��^
        /// </summary>
        /// <param name="threadId">�D�D ID</param>
        /// <param name="pageIndex">���Ư��ޡ]�q 0 �}�l�^</param>
        /// <param name="pageSize">�C������</param>
        /// <returns>�D�D�Ա��E�X�ҫ�</returns>
        Task<ThreadDetailReadModel?> GetThreadDetailAsync(long threadId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// ���o�Τ᪺�D�D�C��
        /// </summary>
        /// <param name="userId">�Τ� ID</param>
        /// <param name="pageIndex">���Ư���</param>
        /// <param name="pageSize">�C������</param>
        /// <returns>�D�D�K�n�C��</returns>
        Task<List<ThreadSummaryReadModel>> GetUserThreadsAsync(int userId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// �j�M�D�D�]�����D�^
        /// </summary>
        /// <param name="keyword">�j�M����r</param>
        /// <param name="forumId">�׾� ID�]�i��^</param>
        /// <param name="pageIndex">���Ư���</param>
        /// <param name="pageSize">�C������</param>
        /// <returns>�D�D�K�n�C��</returns>
        Task<List<ThreadSummaryReadModel>> SearchThreadsAsync(string keyword, int? forumId = null, int pageIndex = 0, int pageSize = 20);
    }
}
