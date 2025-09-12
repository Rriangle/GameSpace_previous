using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.MiniGame;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.MiniGame.Controllers
{
    [Area("MiniGame")]
    public class GameController : Controller
    {
        private readonly IMiniGameService _miniGameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IMiniGameService miniGameService, ILogger<GameController> logger)
        {
            _miniGameService = miniGameService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartGame(int petId, int level)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _miniGameService.StartGameAsync(userId, petId, level);
                if (result.Success)
                {
                    return Json(new { success = true, message = result.Message, gameId = result.GameRecord?.PlayId });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start game for user {UserId}", userId);
                return Json(new { success = false, message = "Failed to start game. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndGame(int playId, string result, int monsterCount, decimal speedMultiplier, int hungerDelta, int moodDelta, int staminaDelta, int cleanlinessDelta)
        {
            try
            {
                var gameResult = await _miniGameService.EndGameAsync(playId, result, monsterCount, speedMultiplier, hungerDelta, moodDelta, staminaDelta, cleanlinessDelta);
                if (gameResult.Success)
                {
                    return Json(new { success = true, message = gameResult.Message, gameRecord = gameResult.GameRecord });
                }
                else
                {
                    return Json(new { success = false, message = gameResult.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to end game {PlayId}", playId);
                return Json(new { success = false, message = "Failed to complete game. Please try again later." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> History(int page = 1)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var gameHistory = await _miniGameService.GetUserGameHistoryAsync(userId, page, 20);
            return View(gameHistory);
        }

        [HttpGet]
        public async Task<IActionResult> DailyCount()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var dailyCount = await _miniGameService.GetUserDailyPlayCountAsync(userId);
            return Json(new { dailyCount = dailyCount, maxDaily = 3 });
        }
    }
}