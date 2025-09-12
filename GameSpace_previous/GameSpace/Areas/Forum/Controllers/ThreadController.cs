using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Forum;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.Forum.Controllers
{
    [Area("Forum")]
    public class ThreadController : Controller
    {
        private readonly IForumService _forumService;
        private readonly ILogger<ThreadController> _logger;

        public ThreadController(IForumService forumService, ILogger<ThreadController> logger)
        {
            _forumService = forumService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _forumService.GetThreadAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index", "Forum");
            }

            var posts = await _forumService.GetThreadPostsAsync(id, 1, 50);
            var viewModel = new ThreadDetailsViewModel
            {
                Thread = result.Thread!,
                Posts = posts
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create(int forumId)
        {
            var viewModel = new CreateThreadViewModel
            {
                ForumId = forumId
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateThreadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _forumService.CreateThreadAsync(model.ForumId, userId, model.Title, model.Content);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Details", new { id = result.Thread!.ThreadId });
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Reply(int threadId, int? parentPostId = null)
        {
            var viewModel = new CreatePostViewModel
            {
                ThreadId = threadId,
                ParentPostId = parentPostId
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _forumService.CreatePostAsync(model.ThreadId, userId, model.Content, model.ParentPostId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Details", new { id = model.ThreadId });
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction(int postId, string reactionType)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _forumService.AddReactionAsync(postId, userId, reactionType);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveReaction(int postId, string reactionType)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _forumService.RemoveReactionAsync(postId, userId, reactionType);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetReactions(int postId)
        {
            var reactions = await _forumService.GetPostReactionsAsync(postId);
            return Json(new { reactions = reactions });
        }
    }

    public class ThreadDetailsViewModel
    {
        public Thread Thread { get; set; } = null!;
        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public class CreateThreadViewModel
    {
        public int ForumId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class CreatePostViewModel
    {
        public int ThreadId { get; set; }
        public int? ParentPostId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}