using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// 論壇讀取專用存儲庫介面 - Stage 2 廣度切片
    /// 提供論壇列表和詳情聚合查詢功能
    /// </summary>
    public interface IForumReadOnlyRepository
    {
        /// <summary>
        /// 取得論壇列表（包含基本統計資訊）
        /// </summary>
        /// <returns>論壇列表聚合模型</returns>
        Task<List<ForumListReadModel>> GetForumListAsync();

        /// <summary>
        /// 取得指定論壇詳情（包含主題列表）
        /// </summary>
        /// <param name="forumId">論壇 ID</param>
        /// <param name="pageIndex">頁數索引（從 0 開始）</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>論壇詳情聚合模型</returns>
        Task<ForumDetailReadModel?> GetForumDetailAsync(int forumId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// 取得指定主題詳情（包含回覆列表）
        /// </summary>
        /// <param name="threadId">主題 ID</param>
        /// <param name="pageIndex">頁數索引（從 0 開始）</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>主題詳情聚合模型</returns>
        Task<ThreadDetailReadModel?> GetThreadDetailAsync(long threadId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// 取得用戶的主題列表
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <param name="pageIndex">頁數索引</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>主題摘要列表</returns>
        Task<List<ThreadSummaryReadModel>> GetUserThreadsAsync(int userId, int pageIndex = 0, int pageSize = 20);

        /// <summary>
        /// 搜尋主題（按標題）
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="forumId">論壇 ID（可選）</param>
        /// <param name="pageIndex">頁數索引</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns>主題摘要列表</returns>
        Task<List<ThreadSummaryReadModel>> SearchThreadsAsync(string keyword, int? forumId = null, int pageIndex = 0, int pageSize = 20);
    }
}
