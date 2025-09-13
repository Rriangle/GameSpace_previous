using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Controllers
{
    /// <summary>
    /// 商城控制器
    /// </summary>
    public class ShopController : Controller
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<ShopController> _logger;

        public ShopController(GameSpaceDbContext context, ILogger<ShopController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 顯示商城首頁
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            return View(products);
        }

        /// <summary>
        /// 顯示商品詳情
        /// </summary>
        public async Task<IActionResult> Product(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// 添加到購物車
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null)
            {
                return Json(new { success = false, message = "商品不存在" });
            }

            if (product.Stock < quantity)
            {
                return Json(new { success = false, message = "庫存不足" });
            }

            // 這裡可以實現購物車邏輯
            // 目前簡化為直接創建訂單

            return Json(new { success = true, message = "已添加到購物車" });
        }

        /// <summary>
        /// 顯示購物車
        /// </summary>
        public IActionResult Cart()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // 這裡可以實現購物車邏輯
            return View();
        }

        /// <summary>
        /// 結帳
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View("Cart", model);
            }

            try
            {
                // 創建訂單
                var order = new Order
                {
                    UserId = userId.Value,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = model.TotalAmount,
                    Status = "待付款",
                    ShippingAddress = model.ShippingAddress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 創建訂單項目
                foreach (var item in model.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("用戶 {UserId} 創建了訂單 {OrderId}", userId, order.OrderId);

                return RedirectToAction("OrderSuccess", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建訂單時發生錯誤");
                ModelState.AddModelError("", "創建訂單時發生錯誤，請稍後再試");
                return View("Cart", model);
            }
        }

        /// <summary>
        /// 訂單成功頁面
        /// </summary>
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        /// <summary>
        /// 我的訂單
        /// </summary>
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        /// <summary>
        /// 獲取當前用戶ID
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// 結帳視圖模型
    /// </summary>
    public class CheckoutViewModel
    {
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    }

    /// <summary>
    /// 購物車項目視圖模型
    /// </summary>
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}