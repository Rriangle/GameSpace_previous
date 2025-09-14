using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 寵物系統控制器
    /// </summary>
    public class PetController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<PetController> _logger;

        public PetController(GameSpaceDbContext context, ILogger<PetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示我的史萊姆頁面
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                // 如果沒有寵物，創建一個新的史萊姆
                pet = await CreateNewPetAsync(userId.Value);
            }

            return View(pet);
        }

        /// <summary>
        /// 餵食寵物
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feed()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return Json(new { success = false, message = "找不到您的寵物" });
            }

            // 增加飢餓值
            pet.Hunger = Math.Min(100, pet.Hunger + 20);
            pet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "餵食成功！", hunger = pet.Hunger });
        }

        /// <summary>
        /// 清潔寵物
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clean()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return Json(new { success = false, message = "找不到您的寵物" });
            }

            // 增加清潔值
            pet.Cleanliness = Math.Min(100, pet.Cleanliness + 25);
            pet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "清潔成功！", cleanliness = pet.Cleanliness });
        }

        /// <summary>
        /// 玩耍（增加心情）
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Play()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return Json(new { success = false, message = "找不到您的寵物" });
            }

            // 增加心情和經驗值
            pet.Mood = Math.Min(100, pet.Mood + 15);
            pet.Experience += 10;
            pet.UpdatedAt = DateTime.UtcNow;

            // 檢查是否升級
            var newLevel = CalculateLevel(pet.Experience);
            if (newLevel > pet.Level)
            {
                pet.Level = newLevel;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"升級了！現在是 {pet.Level} 級！", level = pet.Level, experience = pet.Experience });
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "玩耍成功！", mood = pet.Mood, experience = pet.Experience });
        }

        /// <summary>
        /// 休息（恢復體力）
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rest()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var pet = await _context.Pet
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return Json(new { success = false, message = "找不到您的寵物" });
            }

            // 恢復體力
            pet.Stamina = Math.Min(100, pet.Stamina + 30);
            pet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "休息成功！", stamina = pet.Stamina });
        }

        /// <summary>
        /// 創建新寵物
        /// </summary>
        private async Task<Pet> CreateNewPetAsync(int userId)
        {
            var pet = new Pet
            {
                UserId = userId,
                PetName = "我的史萊姆",
                Level = 1,
                Hunger = 80,
                Mood = 80,
                Stamina = 80,
                Cleanliness = 80,
                Health = 100,
                Experience = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Pet.Add(pet);
            await _context.SaveChangesAsync();

            return pet;
        }

        /// <summary>
        /// 計算等級
        /// </summary>
        private int CalculateLevel(int experience)
        {
            return (int)Math.Floor(Math.Sqrt(experience / 100)) + 1;
        }

        /// <summary>
        /// 獲取當前用戶ID
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}