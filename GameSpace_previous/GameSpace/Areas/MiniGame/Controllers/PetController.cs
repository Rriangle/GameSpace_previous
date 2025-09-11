using Microsoft.AspNetCore.Mvc;
using GameSpace.Areas.MiniGame.Models;
using GameSpace.Areas.MiniGame.Services;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 寵物管理控制器 - 負責處理寵物飼養、屬性調整、互動功能
    /// 對應資料表：Pet
    /// </summary>
    [Area("MiniGame")]
    public class PetController : Controller
    {
        private readonly IPetInteractionService _petService;

        public PetController(IPetInteractionService petService)
        {
            _petService = petService;
        }
        /// <summary>
        /// 寵物主頁面 - 顯示寵物狀態與基本資訊
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "我的寵物";
            
            const int currentUserId = 1; // 實際會從認證系統取得
            const int petId = 1; // 實際會查詢使用者的寵物列表
            
            // 使用寵物服務取得實際狀態資料
            var viewModel = await _petService.GetPetStatusAsync(petId, currentUserId);
            
            return View(viewModel);
        }

        /// <summary>
        /// 寵物詳細資訊頁面 - 顯示完整屬性與成長記錄
        /// </summary>
        public IActionResult Details(int id)
        {
            ViewData["Title"] = "寵物詳細資訊";
            ViewData["PetId"] = id;
            return View();
        }

        /// <summary>
        /// 寵物屬性調整頁面 - 設定外觀、顏色等可調整項目
        /// </summary>
        public IActionResult Customize(int id)
        {
            ViewData["Title"] = "寵物外觀設定";
            ViewData["PetId"] = id;
            return View();
        }

        /// <summary>
        /// 餵食寵物動作 - POST 方法，實作真實餵食邏輯
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Feed(int id)
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            
            var result = await _petService.FeedPetAsync(id, currentUserId);
            
            if (result.Success)
            {
                var message = result.Message;
                if (result.LevelUpTriggered && result.LevelUpReward != null)
                {
                    message += $" 恭喜升級到 Lv.{result.LevelUpReward.NewLevel}！獲得 {result.LevelUpReward.PointsReward} 積分！";
                }
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 與寵物玩耍動作 - POST 方法，實作真實陪玩邏輯
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Play(int id)
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            
            var result = await _petService.PlayWithPetAsync(id, currentUserId);
            
            if (result.Success)
            {
                var message = result.Message;
                if (result.LevelUpTriggered && result.LevelUpReward != null)
                {
                    message += $" {result.LevelUpReward.Message}獲得 {result.LevelUpReward.PointsReward} 積分！";
                }
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 清潔寵物動作 - POST 方法，實作真實清潔邏輯
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Clean(int id)
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            
            var result = await _petService.CleanPetAsync(id, currentUserId);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 取得寵物目前狀態 - AJAX 端點，實作真實狀態查詢
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPetStatus(int id)
        {
            const int currentUserId = 1; // 實際會從認證系統取得
            
            try
            {
                var petStatus = await _petService.GetPetStatusAsync(id, currentUserId);
                
                return Json(new { 
                    success = true,
                    petId = petStatus.Pet.PetID,
                    name = petStatus.Pet.PetName,
                    level = petStatus.Pet.Level,
                    experience = petStatus.Pet.Experience,
                    nextLevelExp = petStatus.NextLevelExpRequired,
                    hunger = petStatus.Pet.Hunger,
                    mood = petStatus.Pet.Mood,
                    stamina = petStatus.Pet.Stamina,
                    cleanliness = petStatus.Pet.Cleanliness,
                    health = petStatus.Pet.Health,
                    overallScore = petStatus.OverallHealthScore,
                    needsCare = petStatus.NeedsCare,
                    suggestedActions = petStatus.SuggestedCareActions
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false,
                    message = "取得寵物狀態失敗" 
                });
            }
        }
    }
}