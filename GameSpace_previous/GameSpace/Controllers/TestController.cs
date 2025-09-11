using Microsoft.AspNetCore.Mvc;
using GameSpace.Data;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 測試控制器 - 用於驗證讀取功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<TestController> _logger;

        public TestController(GameSpaceDbContext context, ILogger<TestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 測試數據庫連接
        /// </summary>
        [HttpGet("db-connection")]
        public async Task<ActionResult> TestDbConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Ok(new { 
                    success = true, 
                    message = "數據庫連接正常",
                    canConnect = canConnect,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "數據庫連接測試失敗");
                return StatusCode(500, new { 
                    success = false, 
                    message = "數據庫連接失敗",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 測試讀取用戶數據
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult> TestUsers()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                var sampleUsers = await _context.Users
                    .Take(5)
                    .Select(u => new { u.User_ID, u.User_name, u.User_Account })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    totalUsers = userCount,
                    sampleUsers = sampleUsers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "讀取用戶數據失敗");
                return StatusCode(500, new { 
                    success = false, 
                    message = "讀取用戶數據失敗",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 測試讀取寵物數據
        /// </summary>
        [HttpGet("pets")]
        public async Task<ActionResult> TestPets()
        {
            try
            {
                var petCount = await _context.Pet.CountAsync();
                var samplePets = await _context.Pet
                    .Take(5)
                    .Select(p => new { p.PetID, p.PetName, p.Level, p.Experience })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    totalPets = petCount,
                    samplePets = samplePets
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "讀取寵物數據失敗");
                return StatusCode(500, new { 
                    success = false, 
                    message = "讀取寵物數據失敗",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 測試讀取優惠券數據
        /// </summary>
        [HttpGet("coupons")]
        public async Task<ActionResult> TestCoupons()
        {
            try
            {
                var couponCount = await _context.Coupon.CountAsync();
                var couponTypeCount = await _context.CouponType.CountAsync();
                var sampleCoupons = await _context.Coupon
                    .Take(5)
                    .Select(c => new { c.CouponID, c.CouponCode, c.IsUsed, c.AcquiredTime })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    totalCoupons = couponCount,
                    totalCouponTypes = couponTypeCount,
                    sampleCoupons = sampleCoupons
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "讀取優惠券數據失敗");
                return StatusCode(500, new { 
                    success = false, 
                    message = "讀取優惠券數據失敗",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 測試讀取商品數據
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult> TestProducts()
        {
            try
            {
                var productCount = await _context.ProductInfo.CountAsync();
                var sampleProducts = await _context.ProductInfo
                    .Take(5)
                    .Select(p => new { p.product_id, p.product_name, p.product_type, p.price })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    totalProducts = productCount,
                    sampleProducts = sampleProducts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "讀取商品數據失敗");
                return StatusCode(500, new { 
                    success = false, 
                    message = "讀取商品數據失敗",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 獲取系統狀態
        /// </summary>
        [HttpGet("status")]
        public ActionResult GetStatus()
        {
            return Ok(new { 
                success = true, 
                message = "GameSpace API 運行正常",
                version = "1.0.0",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }
    }
}
