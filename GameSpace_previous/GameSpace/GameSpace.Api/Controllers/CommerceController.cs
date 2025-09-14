using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Api.Controllers
{
    /// <summary>
    /// 商城相關 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommerceController : ControllerBase
    {
        private readonly ICommerceReadOnlyRepository _commerceRepository;

        public CommerceController(ICommerceReadOnlyRepository commerceRepository)
        {
            _commerceRepository = commerceRepository;
        }

        /// <summary>
        /// 取得官方商城排行榜
        /// </summary>
        [HttpGet("rankings")]
        public async Task<ActionResult<List<OfficialStoreRankingReadModel>>> GetOfficialStoreRankings(
            [FromQuery] string? periodType = null,
            [FromQuery] int limit = 20)
        {
            try
            {
                var rankings = await _commerceRepository.GetOfficialStoreRankingsAsync(periodType, limit);
                return Ok(rankings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得排行榜失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得商品列表
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult<List<ProductInfoReadModel>>> GetProducts(
            [FromQuery] string? productType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var products = await _commerceRepository.GetProductsAsync(productType, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得商品列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得商品詳情
        /// </summary>
        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ProductInfoReadModel>> GetProductById(int productId)
        {
            try
            {
                var product = await _commerceRepository.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound($"找不到商品 ID: {productId}");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得商品詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得遊戲商品詳細資訊
        /// </summary>
        [HttpGet("products/{productId}/game-details")]
        public async Task<ActionResult<List<GameProductDetailsReadModel>>> GetGameProductDetails(int productId)
        {
            try
            {
                var details = await _commerceRepository.GetGameProductDetailsAsync(productId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得遊戲商品詳細資訊失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得非遊戲商品詳細資訊
        /// </summary>
        [HttpGet("products/{productId}/other-details")]
        public async Task<ActionResult<List<OtherProductDetailReadModel>>> GetOtherProductDetails(int productId)
        {
            try
            {
                var details = await _commerceRepository.GetOtherProductDetailsAsync(productId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得非遊戲商品詳細資訊失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得供應商列表
        /// </summary>
        [HttpGet("suppliers")]
        public async Task<ActionResult<List<SupplierReadModel>>> GetSuppliers()
        {
            try
            {
                var suppliers = await _commerceRepository.GetSuppliersAsync();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得供應商列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得用戶訂單列表
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
                return StatusCode(500, $"取得用戶訂單列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得訂單詳情
        /// </summary>
        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<OrderInfoReadModel>> GetOrderById(int orderId)
        {
            try
            {
                var order = await _commerceRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound($"找不到訂單 ID: {orderId}");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得訂單詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得訂單項目
        /// </summary>
        [HttpGet("orders/{orderId}/items")]
        public async Task<ActionResult<List<OrderItemReadModel>>> GetOrderItems(int orderId)
        {
            try
            {
                var items = await _commerceRepository.GetOrderItemsAsync(orderId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得訂單項目失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得自由市場排行榜
        /// </summary>
        [HttpGet("player-market/rankings")]
        public async Task<ActionResult<List<PlayerMarketRankingReadModel>>> GetPlayerMarketRankings(
            [FromQuery] string? periodType = null,
            [FromQuery] int limit = 20)
        {
            try
            {
                var rankings = await _commerceRepository.GetPlayerMarketRankingsAsync(periodType, limit);
                return Ok(rankings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得自由市場排行榜失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得自由市場商品列表
        /// </summary>
        [HttpGet("player-market/products")]
        public async Task<ActionResult<List<PlayerMarketProductInfoReadModel>>> GetPlayerMarketProducts(
            [FromQuery] string? productType = null,
            [FromQuery] int? sellerId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var products = await _commerceRepository.GetPlayerMarketProductsAsync(productType, sellerId, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得自由市場商品列表失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得自由市場商品詳情
        /// </summary>
        [HttpGet("player-market/products/{productId}")]
        public async Task<ActionResult<PlayerMarketProductInfoReadModel>> GetPlayerMarketProductById(int productId)
        {
            try
            {
                var product = await _commerceRepository.GetPlayerMarketProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound($"找不到自由市場商品 ID: {productId}");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得自由市場商品詳情失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得商品統計資訊
        /// </summary>
        [HttpGet("stats/products")]
        public async Task<ActionResult<object>> GetProductStats([FromQuery] string? productType = null)
        {
            try
            {
                var count = await _commerceRepository.GetProductCountAsync(productType);
                return Ok(new { productCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得商品統計失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得訂單統計資訊
        /// </summary>
        [HttpGet("stats/orders")]
        public async Task<ActionResult<object>> GetOrderStats([FromQuery] int? userId = null)
        {
            try
            {
                var count = await _commerceRepository.GetOrderCountAsync(userId);
                return Ok(new { orderCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"取得訂單統計失敗: {ex.Message}");
            }
        }
    }
}
