using Microsoft.AspNetCore.Mvc;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 健康檢查控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(GameSpaceDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 基本健康檢查
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetHealth()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var userCount = await _context.Users.CountAsync();
                
                return Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    database = canConnect ? "connected" : "disconnected",
                    userCount = userCount,
                    version = "1.0.0"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "健康檢查失敗");
                return StatusCode(500, new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 詳細系統狀態
        /// </summary>
        [HttpGet("detailed")]
        public async Task<ActionResult> GetDetailedHealth()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var userCount = await _context.Users.CountAsync();
                var petCount = await _context.Pet.CountAsync();
                var walletCount = await _context.UserWallet.CountAsync();

                return Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    database = new
                    {
                        connected = canConnect,
                        userCount = userCount,
                        petCount = petCount,
                        walletCount = walletCount
                    },
                    system = new
                    {
                        version = "1.0.0",
                        environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                        machineName = Environment.MachineName,
                        processorCount = Environment.ProcessorCount
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "詳細健康檢查失敗");
                return StatusCode(500, new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message
                });
            }
        }
    }
}
