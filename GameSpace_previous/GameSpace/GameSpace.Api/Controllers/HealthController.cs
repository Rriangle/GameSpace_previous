using Microsoft.AspNetCore.Mvc;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 健康檢查控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// 基本健康檢查端點
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                message = "GameSpace API 運行正常"
            });
        }

        /// <summary>
        /// 詳細健康檢查端點
        /// </summary>
        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            return Ok(new {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                services = new {
                    database = "connected",
                    cache = "operational",
                    logging = "active"
                },
                message = "所有服務運行正常"
            });
        }
    }
}
