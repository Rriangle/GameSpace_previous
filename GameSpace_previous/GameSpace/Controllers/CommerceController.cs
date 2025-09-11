using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Models;
using GameSpace.Core.Repositories;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 商城相關API端點
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommerceController : ControllerBase
    {
        private readonly ICommerceReadOnlyRepository _commerceRepository;
        private readonly ILogger<CommerceController> _logger;

        public CommerceController(ICommerceReadOnlyRepository commerceRepository, ILogger<CommerceController> logger)
        {
            _commerceRepository = commerceRepository;
            _logger = logger;
        }

        /// <summary>
        /// 獲取商品列表
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult<List<ProductInfoReadModel>>> GetProducts(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20, 
            [FromQuery] string? category = null)
        {
            try
            {
                var products = await _commerceRepository.GetProductsAsync(category, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取商品列表時發生錯誤");
                return StatusCode(500, "內部伺服器錯誤");
            }
        }

        /// <summary>
        /// 根據ID獲取商品
        /// </summary>
        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductInfoReadModel>> GetProduct(int id)
        {
            try
            {
                var product = await _commerceRepository.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound("商品不存在");
                
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取商品 ID {ProductId} 時發生錯誤", id);
                return StatusCode(500, "內部伺服器錯誤");
            }
        }

        /// <summary>
        /// 獲取用戶訂單
        /// </summary>
        [HttpGet("orders/user/{userId}")]
        public async Task<ActionResult<List<OrderInfoReadModel>>> GetUserOrders(
            int userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var orders = await _commerceRepository.GetUserOrdersAsync(userId, page, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶訂單 用戶ID {UserId} 時發生錯誤", userId);
                return StatusCode(500, "內部伺服器錯誤");
            }
        }

        /// <summary>
        /// 根據ID獲取訂單
        /// </summary>
        [HttpGet("orders/{id}")]
        public async Task<ActionResult<OrderInfoReadModel>> GetOrder(int id)
        {
            try
            {
                var order = await _commerceRepository.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound("訂單不存在");
                
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取訂單 ID {OrderId} 時發生錯誤", id);
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
    }
}
