using Microsoft.AspNetCore.Mvc;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    public class PetController : Controller
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<PetController> _logger;

        public PetController(GameSpacedatabaseContext context, ILogger<PetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Pet
        public async Task<IActionResult> Index()
        {
            var pets = await _context.Pets
                .Include(p => p.User)
                .ToListAsync();
            return View(pets);
        }

        // GET: Pet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PetId == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: Pet/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users, "UserId", "UserName");
            return View();
        }

        // POST: Pet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetId,UserId,PetName,Level,LevelUpTime,Experience,Hunger,Mood,Stamina,Cleanliness,Health,SkinColor,SkinColorChangedTime,BackgroundColor,BackgroundColorChangedTime,PointsChangedSkinColor,PointsChangedBackgroundColor,PointsGainedLevelUp,PointsGainedTimeLevelUp")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users, "UserId", "UserName", pet.UserId);
            return View(pet);
        }

        // GET: Pet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users, "UserId", "UserName", pet.UserId);
            return View(pet);
        }

        // POST: Pet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PetId,UserId,PetName,Level,LevelUpTime,Experience,Hunger,Mood,Stamina,Cleanliness,Health,SkinColor,SkinColorChangedTime,BackgroundColor,BackgroundColorChangedTime,PointsChangedSkinColor,PointsChangedBackgroundColor,PointsGainedLevelUp,PointsGainedTimeLevelUp")] Pet pet)
        {
            if (id != pet.PetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.PetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Users, "UserId", "UserName", pet.UserId);
            return View(pet);
        }

        // GET: Pet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PetId == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // POST: Pet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Pet interaction methods
        [HttpPost]
        public async Task<IActionResult> Feed(int petId)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return NotFound();
            }

            // Increase hunger and mood
            pet.Hunger = Math.Min(100, pet.Hunger + 20);
            pet.Mood = Math.Min(100, pet.Mood + 10);
            
            _context.Update(pet);
            await _context.SaveChangesAsync();

            return Json(new { success = true, hunger = pet.Hunger, mood = pet.Mood });
        }

        [HttpPost]
        public async Task<IActionResult> Play(int petId)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return NotFound();
            }

            // Increase mood and experience, decrease hunger and stamina
            pet.Mood = Math.Min(100, pet.Mood + 15);
            pet.Experience += 10;
            pet.Hunger = Math.Max(0, pet.Hunger - 10);
            pet.Stamina = Math.Max(0, pet.Stamina - 15);

            // Check for level up
            if (pet.Experience >= pet.Level * 100)
            {
                pet.Level++;
                pet.Experience = 0;
                pet.LevelUpTime = DateTime.UtcNow;
                pet.PointsGainedLevelUp += 50;
                pet.PointsGainedTimeLevelUp = DateTime.UtcNow;
            }

            _context.Update(pet);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                mood = pet.Mood, 
                experience = pet.Experience, 
                level = pet.Level,
                leveledUp = pet.Experience == 0 && pet.Level > 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> Clean(int petId)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return NotFound();
            }

            // Increase cleanliness and mood
            pet.Cleanliness = Math.Min(100, pet.Cleanliness + 25);
            pet.Mood = Math.Min(100, pet.Mood + 5);

            _context.Update(pet);
            await _context.SaveChangesAsync();

            return Json(new { success = true, cleanliness = pet.Cleanliness, mood = pet.Mood });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSkinColor(int petId, string color)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return NotFound();
            }

            // Change skin color (costs points)
            if (pet.PointsChangedSkinColor >= 100)
            {
                pet.SkinColor = color;
                pet.SkinColorChangedTime = DateTime.UtcNow;
                pet.PointsChangedSkinColor -= 100;

                _context.Update(pet);
                await _context.SaveChangesAsync();

                return Json(new { success = true, skinColor = pet.SkinColor, points = pet.PointsChangedSkinColor });
            }

            return Json(new { success = false, message = "Not enough points to change skin color" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeBackgroundColor(int petId, string color)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return NotFound();
            }

            // Change background color (costs points)
            if (pet.PointsChangedBackgroundColor >= 150)
            {
                pet.BackgroundColor = color;
                pet.BackgroundColorChangedTime = DateTime.UtcNow;
                pet.PointsChangedBackgroundColor -= 150;

                _context.Update(pet);
                await _context.SaveChangesAsync();

                return Json(new { success = true, backgroundColor = pet.BackgroundColor, points = pet.PointsChangedBackgroundColor });
            }

            return Json(new { success = false, message = "Not enough points to change background color" });
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetId == id);
        }
    }
}