using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Web.Controllers
{
    /// <summary>
    /// 論壇 MVC 控制器
    /// </summary>
    public class ForumController : Controller
    {
        private readonly ICommunityReadOnlyRepository _communityRepository;

        public ForumController(ICommunityReadOnlyRepository communityRepository)
        {
            _communityRepository = communityRepository;
        }

        /// <summary>
        /// 論壇首頁
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var forums = await _communityRepository.GetForumsAsync();
                return View(forums);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入論壇列表失敗: {ex.Message}";
                return View(new List<ForumReadModel>());
            }
        }

        /// <summary>
        /// 論壇詳情頁
        /// </summary>
        public async Task<IActionResult> Forum(int id, int page = 1)
        {
            try
            {
                var forum = await _communityRepository.GetForumByIdAsync(id);
                if (forum == null)
                {
                    return NotFound();
                }

                var threads = await _communityRepository.GetThreadsByForumAsync(id, page, 20);
                var threadCount = await _communityRepository.GetThreadCountAsync(id);

                ViewBag.Forum = forum;
                ViewBag.Threads = threads;
                ViewBag.ThreadCount = threadCount;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)threadCount / 20);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入論壇失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 主題詳情頁
        /// </summary>
        public async Task<IActionResult> Thread(long id, int page = 1)
        {
            try
            {
                var thread = await _communityRepository.GetThreadByIdAsync(id);
                if (thread == null)
                {
                    return NotFound();
                }

                var posts = await _communityRepository.GetThreadPostsAsync(id, page, 20);
                var postCount = await _communityRepository.GetPostCountAsync(id);

                ViewBag.Thread = thread;
                ViewBag.Posts = posts;
                ViewBag.PostCount = postCount;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)postCount / 20);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入主題失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 文章列表頁
        /// </summary>
        public async Task<IActionResult> Posts(string? type = null, int? gameId = null, int page = 1)
        {
            try
            {
                var posts = await _communityRepository.GetPostsAsync(type, gameId, page, 20);
                
                ViewBag.Posts = posts;
                ViewBag.Type = type;
                ViewBag.GameId = gameId;
                ViewBag.CurrentPage = page;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入文章列表失敗: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// 文章詳情頁
        /// </summary>
        public async Task<IActionResult> Post(int id)
        {
            try
            {
                var post = await _communityRepository.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                ViewBag.Post = post;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"載入文章失敗: {ex.Message}";
                return View();
            }
        }
    }
}
