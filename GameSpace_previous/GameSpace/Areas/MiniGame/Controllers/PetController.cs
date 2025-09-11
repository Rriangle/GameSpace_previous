using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Areas.MiniGame.Controllers
{
    /// <summary>
    /// 寵物管理控制器 - 負責處理寵物飼養、屬性調整、互動功能
    /// 對應資料表：Pet
    /// </summary>
    [Area("MiniGame")]
    public class PetController : Controller
    {
        /// <summary>
        /// 寵物主頁面 - 顯示寵物狀態與基本資訊
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "我的寵物";
            return View();
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
        /// 餵食寵物動作 - POST 方法
        /// </summary>
        [HttpPost]
        public IActionResult Feed(int id)
        {
            // 目前回傳成功訊息，實際邏輯待後續階段實作
            TempData["SuccessMessage"] = "餵食成功！寵物飢餓度 +20";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 與寵物玩耍動作 - POST 方法
        /// </summary>
        [HttpPost]
        public IActionResult Play(int id)
        {
            // 目前回傳成功訊息，實際邏輯待後續階段實作
            TempData["SuccessMessage"] = "陪玩成功！寵物心情 +15";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 取得寵物目前狀態 - AJAX 端點
        /// </summary>
        [HttpGet]
        public IActionResult GetPetStatus(int id)
        {
            // 目前回傳假資料，實際邏輯待後續階段實作
            return Json(new { 
                petId = id,
                name = "可愛小龍",
                level = 5,
                experience = 320,
                hunger = 80,
                mood = 75,
                stamina = 60,
                cleanliness = 90,
                health = 95
            });
        }
    }
}