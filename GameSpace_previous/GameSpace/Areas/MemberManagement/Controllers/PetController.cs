using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Pet;
using GameSpace.Models;
using System.Threading.Tasks;
using System;

namespace GameSpace.Areas.MemberManagement.Controllers
{
    [Area("MemberManagement")]
    public class PetController : Controller
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetController> _logger;

        public PetController(IPetService petService, ILogger<PetController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var pet = await _petService.GetUserPetAsync(userId);
            return View(pet);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string petName)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _petService.CreatePetAsync(userId, petName);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pet creation failed for user {UserId}", userId);
                TempData["ErrorMessage"] = "Failed to create pet. Please try again later.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CareAction(string actionType)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _petService.PerformPetCareActionAsync(userId, actionType);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pet care action failed for user {UserId}", userId);
                TempData["ErrorMessage"] = "Failed to care for pet. Please try again later.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSkinColor(string newColorHex)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _petService.ChangePetSkinColorAsync(userId, newColorHex);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pet skin color change failed for user {UserId}", userId);
                TempData["ErrorMessage"] = "Failed to change pet skin color. Please try again later.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeBackgroundColor(string newBackgroundName)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            try
            {
                var result = await _petService.ChangePetBackgroundColorAsync(userId, newBackgroundName);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pet background color change failed for user {UserId}", userId);
                TempData["ErrorMessage"] = "Failed to change pet background. Please try again later.";
            }

            return RedirectToAction("Index");
        }
    }
}