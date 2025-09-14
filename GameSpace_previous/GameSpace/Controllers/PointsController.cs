using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Controllers
{
    [ApiController]
    [Route("api/points")]
    public class PointsController : ControllerBase
    {
        [HttpGet("balance")]
        public IActionResult Balance()
        {
            return Ok(new { balance = 0 });
        }
    }
}

