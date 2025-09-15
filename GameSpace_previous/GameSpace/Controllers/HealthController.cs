using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 系統健康檢查控制器
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly GameSpaceDatabaseContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(GameSpaceDatabaseContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 資料庫連線健康檢查
        /// </summary>
        /// <returns>資料庫連線狀態</returns>
        [HttpGet("db")]
        public async Task<IActionResult> DatabaseHealth()
        {
            try
            {
                // 嘗試執行簡單的資料庫查詢來檢查連線
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    _logger.LogWarning("資料庫連線檢查失敗 - 無法連線到資料庫");
                    return StatusCode(503, new { 
                        status = "unhealthy", 
                        message = "無法連線到資料庫",
                        timestamp = DateTime.UtcNow
                    });
                }

                // 執行簡單的查詢測試
                var userCount = await _context.Users.CountAsync();
                
                _logger.LogInformation("資料庫健康檢查通過 - 使用者總數: {UserCount}", userCount);
                
                return Ok(new { 
                    status = "ok", 
                    message = "資料庫連線正常",
                    userCount = userCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "資料庫健康檢查發生錯誤");
                
                return StatusCode(503, new { 
                    status = "error", 
                    message = "資料庫健康檢查發生錯誤",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// 一般健康檢查
        /// </summary>
        /// <returns>應用程式健康狀態</returns>
        [HttpGet]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "ok", 
                message = "應用程式運行正常",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }
    }
}