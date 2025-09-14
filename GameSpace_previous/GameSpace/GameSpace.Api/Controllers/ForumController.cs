using GameSpace.Core.Models;
using GameSpace.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 論壇 API 控制器 - Stage 2 廣度切片
    /// 提供論壇聚合查詢端點
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {
        private readonly IForumReadOnlyRepository _forumRepository;
        private readonly ILogger<ForumController> _logger;

        public ForumController(
            IForumReadOnlyRepository forumRepository,
            ILogger<ForumController> logger)
        {
            _forumRepository = forumRepository;
            _logger = logger;
        }

        /// <summary>
        /// 取得論壇列表
        /// </summary>
        /// <returns>論壇列表聚合資訊</returns>
        [HttpGet("list")]
        public async Task<ActionResult<List<ForumListReadModel>>> GetForumList()
        {
            try
            {
                _logger.LogInformation("正在查詢論壇列表");

                var forums = await _forumRepository.GetForumListAsync();
                
                _logger.LogInformation("成功取得論壇列表 Count: {Count}", forums.Count);

                return Ok(forums);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢論壇列表時發生錯誤");
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得論壇詳情
        /// </summary>
        /// <param name="forumId">論壇 ID</param>
        /// <param name="pageIndex">頁數索引（預設 0）</param>
        /// <param name="pageSize">每頁筆數（預設 20）</param>
        /// <returns>論壇詳情聚合資訊</returns>
        [HttpGet("detail/{forumId:int}")]
        public async Task<ActionResult<ForumDetailReadModel>> GetForumDetail(
            int forumId, 
            [FromQuery] int pageIndex = 0, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("正在查詢論壇詳情 ForumId: {ForumId}, Page: {PageIndex}, Size: {PageSize}", 
                    forumId, pageIndex, pageSize);

                // 驗證分頁參數
                if (pageIndex < 0) pageIndex = 0;
                if (pageSize <= 0 || pageSize > 100) pageSize = 20;

                var forumDetail = await _forumRepository.GetForumDetailAsync(forumId, pageIndex, pageSize);
                
                if (forumDetail == null)
                {
                    _logger.LogWarning("找不到論壇資料 ForumId: {ForumId}", forumId);
                    return NotFound(new { Message = "找不到指定的論壇" });
                }

                _logger.LogInformation("成功取得論壇詳情 ForumId: {ForumId}, ThreadCount: {ThreadCount}", 
                    forumId, forumDetail.Threads.Count);

                return Ok(forumDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢論壇詳情時發生錯誤 ForumId: {ForumId}", forumId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 取得主題詳情
        /// </summary>
        /// <param name="threadId">主題 ID</param>
        /// <param name="pageIndex">頁數索引（預設 0）</param>
        /// <param name="pageSize">每頁筆數（預設 20）</param>
        /// <returns>主題詳情聚合資訊</returns>
        [HttpGet("thread/{threadId:long}")]
        public async Task<ActionResult<ThreadDetailReadModel>> GetThreadDetail(
            long threadId, 
            [FromQuery] int pageIndex = 0, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("正在查詢主題詳情 ThreadId: {ThreadId}, Page: {PageIndex}, Size: {PageSize}", 
                    threadId, pageIndex, pageSize);

                // 驗證分頁參數
                if (pageIndex < 0) pageIndex = 0;
                if (pageSize <= 0 || pageSize > 100) pageSize = 20;

                var threadDetail = await _forumRepository.GetThreadDetailAsync(threadId, pageIndex, pageSize);
                
                if (threadDetail == null)
                {
                    _logger.LogWarning("找不到主題資料 ThreadId: {ThreadId}", threadId);
                    return NotFound(new { Message = "找不到指定的主題" });
                }

                _logger.LogInformation("成功取得主題詳情 ThreadId: {ThreadId}, PostCount: {PostCount}", 
                    threadId, threadDetail.Posts.Count);

                return Ok(threadDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢主題詳情時發生錯誤 ThreadId: {ThreadId}", threadId);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }

        /// <summary>
        /// 搜尋主題
        /// </summary>
        /// <param name="keyword">搜尋關鍵字</param>
        /// <param name="forumId">論壇 ID（可選）</param>
        /// <param name="pageIndex">頁數索引（預設 0）</param>
        /// <param name="pageSize">每頁筆數（預設 20）</param>
        /// <returns>主題摘要列表</returns>
        [HttpGet("search")]
        public async Task<ActionResult<List<ThreadSummaryReadModel>>> SearchThreads(
            [FromQuery] string keyword,
            [FromQuery] int? forumId = null,
            [FromQuery] int pageIndex = 0, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { Message = "搜尋關鍵字不能為空" });
                }

                _logger.LogInformation("正在搜尋主題 Keyword: {Keyword}, ForumId: {ForumId}, Page: {PageIndex}, Size: {PageSize}", 
                    keyword, forumId, pageIndex, pageSize);

                // 驗證分頁參數
                if (pageIndex < 0) pageIndex = 0;
                if (pageSize <= 0 || pageSize > 100) pageSize = 20;

                var threads = await _forumRepository.SearchThreadsAsync(keyword, forumId, pageIndex, pageSize);
                
                _logger.LogInformation("成功搜尋主題 Keyword: {Keyword}, Count: {Count}", keyword, threads.Count);

                return Ok(threads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜尋主題時發生錯誤 Keyword: {Keyword}", keyword);
                return StatusCode(500, new { Message = "伺服器內部錯誤" });
            }
        }
    }
}