using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Models;
using GameSpace.Core.Repositories;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 社群相關API端點
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityReadOnlyRepository _communityRepository;
        private readonly ILogger<CommunityController> _logger;

        public CommunityController(ICommunityReadOnlyRepository communityRepository, ILogger<CommunityController> logger)
        {
            _communityRepository = communityRepository;
            _logger = logger;
        }

        /// <summary>
        /// 獲取文章列表
        /// </summary>
        [HttpGet("posts")]
        public async Task<ActionResult<List<PostReadModel>>> GetPosts(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20, 
            [FromQuery] string? category = null)
        {
            try
            {
                var posts = await _communityRepository.GetPostsByTypeAsync(category ?? "all", page, pageSize);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取文章列表時發生錯誤");
                return StatusCode(500, "內部伺服器錯誤");
            }
        }

        /// <summary>
        /// 獲取論壇列表
        /// </summary>
        [HttpGet("forums")]
        public async Task<ActionResult<List<ForumReadModel>>> GetForums()
        {
            try
            {
                var forums = await _communityRepository.GetAllForumsAsync();
                return Ok(forums);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取論壇列表時發生錯誤");
                return StatusCode(500, "內部伺服器錯誤");
            }
        }

        /// <summary>
        /// 獲取討論串列表
        /// </summary>
        [HttpGet("threads")]
        public async Task<ActionResult<List<ThreadReadModel>>> GetThreads(
            [FromQuery] int forumId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var threads = await _communityRepository.GetThreadsByForumIdAsync(forumId, page, pageSize);
                return Ok(threads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取討論串列表時發生錯誤");
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
    }
}
