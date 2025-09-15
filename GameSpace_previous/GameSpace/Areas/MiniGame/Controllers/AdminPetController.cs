using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GameSpace.Areas.MiniGame.Controllers
{
    [Area("MiniGame")]
    [Route("MiniGame/[controller]")]
    public class AdminPetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MiniGame/AdminPet
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int? levelFrom = null, int? levelTo = null, int page = 1, int pageSize = 20)
        {
            ViewData["Title"] = "寵物管理";
            ViewData["Description"] = "查看和管理所有會員的寵物狀態";

            var petsQuery = _context.Pet
                .Include(p => p.User)
                .AsNoTracking();

            // 搜尋功能
            if (!string.IsNullOrEmpty(searchTerm))
            {
                petsQuery = petsQuery.Where(p => 
                    p.User.User_name.Contains(searchTerm) ||
                    p.User.User_Account.Contains(searchTerm) ||
                    p.PetName.Contains(searchTerm));
                ViewData["SearchTerm"] = searchTerm;
            }

            // 等級篩選
            if (levelFrom.HasValue)
            {
                petsQuery = petsQuery.Where(p => p.Level >= levelFrom.Value);
                ViewData["LevelFrom"] = levelFrom.Value;
            }

            if (levelTo.HasValue)
            {
                petsQuery = petsQuery.Where(p => p.Level <= levelTo.Value);
                ViewData["LevelTo"] = levelTo.Value;
            }

            // 分頁
            var totalCount = await petsQuery.CountAsync();
            var pets = await petsQuery
                .OrderByDescending(p => p.Level)
                .ThenByDescending(p => p.Experience)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PetReadModel
                {
                    PetID = p.PetID,
                    UserName = p.User.User_name,
                    UserAccount = p.User.User_Account,
                    PetName = p.PetName,
                    Level = p.Level,
                    Experience = p.Experience,
                    LevelUpTime = p.LevelUpTime,
                    Hunger = p.Hunger,
                    Mood = p.Mood,
                    Stamina = p.Stamina,
                    Cleanliness = p.Cleanliness,
                    Health = p.Health,
                    SkinColor = p.SkinColor,
                    BackgroundColor = p.BackgroundColor
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(pets);
        }

        // GET: MiniGame/AdminPet/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "寵物詳細資料";
            ViewData["Description"] = "查看寵物的詳細狀態與歷史記錄";

            var pet = await _context.Pet
                .Include(p => p.User)
                .AsNoTracking()
                .Where(p => p.PetID == id)
                .Select(p => new PetDetailReadModel
                {
                    PetID = p.PetID,
                    UserID = p.UserID,
                    UserName = p.User.User_name,
                    UserAccount = p.User.User_Account,
                    PetName = p.PetName,
                    Level = p.Level,
                    Experience = p.Experience,
                    LevelUpTime = p.LevelUpTime,
                    Hunger = p.Hunger,
                    Mood = p.Mood,
                    Stamina = p.Stamina,
                    Cleanliness = p.Cleanliness,
                    Health = p.Health,
                    SkinColor = p.SkinColor,
                    ColorChangedTime = p.ColorChangedTime,
                    BackgroundColor = p.BackgroundColor,
                    BackgroundColorChangedTime = p.BackgroundColorChangedTime,
                    PointsChanged_color = p.PointsChanged_color,
                    PointsChangedTime_color = p.PointsChangedTime_color,
                    PointsGained_levelUp = p.PointsGained_levelUp,
                    PointsGainedTime_levelUp = p.PointsGainedTime_levelUp
                })
                .FirstOrDefaultAsync();

            if (pet == null)
            {
                return NotFound($"找不到ID {id} 的寵物");
            }

            // 取得寵物相關的小遊戲記錄（最近20筆）
            pet.GameHistory = await _context.MiniGame
                .AsNoTracking()
                .Where(g => g.PetID == id)
                .OrderByDescending(g => g.StartTime)
                .Take(20)
                .Select(g => new MiniGameHistoryReadModel
                {
                    PlayID = g.PlayID,
                    Level = g.Level,
                    Result = g.Result,
                    ExpGained = g.ExpGained,
                    PointsChanged = g.PointsChanged,
                    StartTime = g.StartTime,
                    EndTime = g.EndTime,
                    Aborted = g.Aborted
                })
                .ToListAsync();

            return View(pet);
        }

        // GET: MiniGame/AdminPet/Status
        [HttpGet("Status")]
        public async Task<IActionResult> Status(int page = 1, int pageSize = 20)
        {
            ViewData["Title"] = "寵物狀態調整";
            ViewData["Description"] = "批量調整寵物的各項狀態值";

            // 取得需要關注的寵物（狀態值過低）
            var petsQuery = _context.Pet
                .Include(p => p.User)
                .AsNoTracking()
                .Where(p => p.Health < 20 || p.Hunger > 80 || p.Mood < 20 || p.Stamina < 20 || p.Cleanliness < 20);

            var totalCount = await petsQuery.CountAsync();
            var pets = await petsQuery
                .OrderBy(p => p.Health)
                .ThenByDescending(p => p.Hunger)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PetStatusReadModel
                {
                    PetID = p.PetID,
                    UserName = p.User.User_name,
                    UserAccount = p.User.User_Account,
                    PetName = p.PetName,
                    Level = p.Level,
                    Hunger = p.Hunger,
                    Mood = p.Mood,
                    Stamina = p.Stamina,
                    Cleanliness = p.Cleanliness,
                    Health = p.Health,
                    StatusIssues = GetStatusIssues(p.Health, p.Hunger, p.Mood, p.Stamina, p.Cleanliness)
                })
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["TotalCount"] = totalCount;

            return View(pets);
        }

        // GET: MiniGame/AdminPet/Levels
        [HttpGet("Levels")]
        public async Task<IActionResult> Levels(int page = 1, int pageSize = 20)
        {
            ViewData["Title"] = "寵物等級管理";
            ViewData["Description"] = "查看寵物等級分布與升級統計";

            // 等級分布統計
            var levelDistribution = await _context.Pet
                .AsNoTracking()
                .GroupBy(p => p.Level)
                .Select(g => new LevelDistribution
                {
                    Level = g.Key,
                    Count = g.Count(),
                    AvgExperience = g.Average(p => p.Experience)
                })
                .OrderBy(ld => ld.Level)
                .ToListAsync();

            // 高等級寵物列表
            var highLevelPets = await _context.Pet
                .Include(p => p.User)
                .AsNoTracking()
                .OrderByDescending(p => p.Level)
                .ThenByDescending(p => p.Experience)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PetLevelReadModel
                {
                    PetID = p.PetID,
                    UserName = p.User.User_name,
                    UserAccount = p.User.User_Account,
                    PetName = p.PetName,
                    Level = p.Level,
                    Experience = p.Experience,
                    LevelUpTime = p.LevelUpTime,
                    PointsGained_levelUp = p.PointsGained_levelUp
                })
                .ToListAsync();

            var totalCount = await _context.Pet.CountAsync();

            var viewModel = new PetLevelsViewModel
            {
                LevelDistribution = levelDistribution,
                HighLevelPets = highLevelPets,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // POST: MiniGame/AdminPet/AdjustStatus (Stub for future implementation)
        [HttpPost("AdjustStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustStatus(int petId, string adjustmentType, int adjustmentValue)
        {
            // 這是一個 Stub 實現，實際的狀態調整功能將在後續階段實現
            // 目前只返回成功訊息，不實際修改資料庫
            
            TempData["InfoMessage"] = $"寵物狀態調整功能尚未完全實現。請求的調整：寵物ID {petId}，調整類型 {adjustmentType}，調整值 {adjustmentValue}";
            
            return RedirectToAction(nameof(Status));
        }

        #region Helper Methods

        private static List<string> GetStatusIssues(int health, int hunger, int mood, int stamina, int cleanliness)
        {
            var issues = new List<string>();

            if (health < 20) issues.Add("健康度過低");
            if (hunger > 80) issues.Add("飢餓度過高");
            if (mood < 20) issues.Add("心情不佳");
            if (stamina < 20) issues.Add("體力不足");
            if (cleanliness < 20) issues.Add("清潔度不足");

            return issues;
        }

        #endregion
    }

    // Read Models
    public class PetReadModel
    {
        public int PetID { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string PetName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public DateTime LevelUpTime { get; set; }
        public int Hunger { get; set; }
        public int Mood { get; set; }
        public int Stamina { get; set; }
        public int Cleanliness { get; set; }
        public int Health { get; set; }
        public string SkinColor { get; set; }
        public string BackgroundColor { get; set; }
    }

    public class PetDetailReadModel : PetReadModel
    {
        public int UserID { get; set; }
        public DateTime ColorChangedTime { get; set; }
        public DateTime BackgroundColorChangedTime { get; set; }
        public int PointsChanged_color { get; set; }
        public DateTime PointsChangedTime_color { get; set; }
        public int PointsGained_levelUp { get; set; }
        public DateTime PointsGainedTime_levelUp { get; set; }
        public List<MiniGameHistoryReadModel> GameHistory { get; set; } = new List<MiniGameHistoryReadModel>();
    }

    public class PetStatusReadModel
    {
        public int PetID { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string PetName { get; set; }
        public int Level { get; set; }
        public int Hunger { get; set; }
        public int Mood { get; set; }
        public int Stamina { get; set; }
        public int Cleanliness { get; set; }
        public int Health { get; set; }
        public List<string> StatusIssues { get; set; } = new List<string>();
    }

    public class PetLevelReadModel
    {
        public int PetID { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string PetName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public DateTime LevelUpTime { get; set; }
        public int PointsGained_levelUp { get; set; }
    }

    public class MiniGameHistoryReadModel
    {
        public int PlayID { get; set; }
        public int Level { get; set; }
        public string Result { get; set; }
        public int ExpGained { get; set; }
        public int PointsChanged { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Aborted { get; set; }
    }

    public class LevelDistribution
    {
        public int Level { get; set; }
        public int Count { get; set; }
        public double AvgExperience { get; set; }
    }

    public class PetLevelsViewModel
    {
        public List<LevelDistribution> LevelDistribution { get; set; } = new List<LevelDistribution>();
        public List<PetLevelReadModel> HighLevelPets { get; set; } = new List<PetLevelReadModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}