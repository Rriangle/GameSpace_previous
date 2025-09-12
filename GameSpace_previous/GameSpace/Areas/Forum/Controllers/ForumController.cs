using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Forum;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.Forum.Controllers
{
    [Area("Forum")]
    public class ForumController : Controller
    {
        private readonly IForumService _forumService;
        private readonly ILogger<ForumController> _logger;

        public ForumController(IForumService forumService, ILogger<ForumController> logger)
        {
            _forumService = forumService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var forums = await _forumService.GetAllForumsAsync();
            return View(forums);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _forumService.GetForumAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            var threads = await _forumService.GetForumThreadsAsync(id, 1, 20);
            var viewModel = new ForumDetailsViewModel
            {
                Forum = result.Forum!,
                Threads = threads
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateForumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _forumService.CreateForumAsync(model.Name, model.Description, model.GameId);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _forumService.GetForumAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            var viewModel = new EditForumViewModel
            {
                ForumId = result.Forum!.ForumId,
                Name = result.Forum.Name,
                Description = result.Forum.Description,
                GameId = result.Forum.GameId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditForumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _forumService.UpdateForumAsync(model.ForumId, model.Name, model.Description);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _forumService.DeleteForumAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search(string keyword)
        {
            ViewBag.Keyword = keyword;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchPosts(string keyword, int page = 1)
        {
            var result = await _forumService.SearchPostsAsync(keyword, page, 20);
            return Json(new { success = result.Success, posts = result.Posts, totalCount = result.TotalCount });
        }

        [HttpPost]
        public async Task<IActionResult> SearchThreads(string keyword, int page = 1)
        {
            var result = await _forumService.SearchThreadsAsync(keyword, page, 20);
            return Json(new { success = result.Success, threads = result.Threads, totalCount = result.TotalCount });
        }
    }

    public class ForumDetailsViewModel
    {
        public Forum Forum { get; set; } = null!;
        public List<Thread> Threads { get; set; } = new List<Thread>();
    }

    public class CreateForumViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? GameId { get; set; }
    }

    public class EditForumViewModel
    {
        public int ForumId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? GameId { get; set; }
    }
}