using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Controllers
{
    [ApiController]
    [Route("api/pet")]
    public class PetRuntimeController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult Me()
        {
            // 前台 index.txt 為示範頁，這裡提供最小可用假資料以對應 UI
            var demo = new
            {
                name = "史萊姆",
                level = 1,
                xp = 0,
                xpMax = 50,
                hunger = 80,
                mood = 80,
                energy = 80,
                clean = 80,
                cleanliness = 80,
                health = 100
            };
            return Ok(demo);
        }

        public class CareRequest { public string? action { get; set; } }

        [HttpPost("care")]
        public IActionResult Care([FromBody] CareRequest req)
        {
            var message = string.IsNullOrWhiteSpace(req?.action)
                ? "已執行：Unknown"
                : $"已執行：{req!.action}";
            var resp = new
            {
                message,
                points = 0,
                pet = new { name = "史萊姆", level = 1, xp = 0, xpMax = 50, hunger = 80, mood = 80, energy = 80, clean = 80, cleanliness = 80, health = 100 }
            };
            return Ok(resp);
        }

        [HttpPost("adventure")]
        public IActionResult Adventure()
        {
            var resp = new
            {
                win = false,
                monster = "slime",
                rewardPoints = 0,
                xp = 0,
                pet = new { name = "史萊姆", level = 1, xp = 0, xpMax = 50, hunger = 80, mood = 80, energy = 80, clean = 80, cleanliness = 80, health = 100 }
            };
            return Ok(resp);
        }
    }
}

