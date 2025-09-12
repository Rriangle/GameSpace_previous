using Microsoft.AspNetCore.Mvc;
using GameSpace.Services.Store;
using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GameSpace.Areas.OnlineStore.Controllers
{
    [Area("OnlineStore")]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(IStoreService storeService, ILogger<StoreController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string category = "", int page = 1)
        {
            List<ProductInfo> products;
            
            if (!string.IsNullOrEmpty(category))
            {
                products = await _storeService.GetProductsByTypeAsync(category, page, 20);
            }
            else
            {
                products = await _storeService.GetAllProductsAsync(page, 20);
            }

            var categories = await GetProductCategoriesAsync();
            
            var viewModel = new StoreIndexViewModel
            {
                Products = products,
                Categories = categories,
                CurrentCategory = category,
                CurrentPage = page
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _storeService.GetProductAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            return View(result.Product);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _storeService.AddToCartAsync(userId, productId, quantity);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var cartItems = await _storeService.GetCartItemsAsync(userId);
            var availableCoupons = await _storeService.GetAvailableCouponsAsync(userId);

            var viewModel = new CartViewModel
            {
                CartItems = cartItems,
                AvailableCoupons = availableCoupons
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int productId, int quantity)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _storeService.UpdateCartItemAsync(userId, productId, quantity);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _storeService.RemoveFromCartAsync(userId, productId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var result = await _storeService.ClearCartAsync(userId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            // 這裡應該從購物車獲取商品
            var orderItems = new List<OrderItem>(); // 模擬訂單商品

            var result = await _storeService.CreateOrderAsync(userId, orderItems, model.ShippingAddress, model.PaymentMethod);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("OrderConfirmation", new { id = result.Order!.OrderId });
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var result = await _storeService.GetOrderAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            return View(result.Order);
        }

        [HttpGet]
        public async Task<IActionResult> Orders(int page = 1)
        {
            // TODO: Get current user ID from authentication
            int userId = 1; // Placeholder

            var orders = await _storeService.GetUserOrdersAsync(userId, page, 20);
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UseCoupon(int orderId, string couponCode)
        {
            var result = await _storeService.UseCouponAsync(orderId, couponCode);
            return Json(new { success = result.Success, message = result.Message, discountAmount = result.DiscountAmount });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(int orderId)
        {
            var result = await _storeService.RemoveCouponAsync(orderId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchProducts(string keyword, int page = 1)
        {
            var result = await _storeService.SearchProductsAsync(keyword, page, 20);
            return Json(new { success = result.Success, products = result.Products, totalCount = result.TotalCount });
        }

        [HttpPost]
        public async Task<IActionResult> SearchByPriceRange(decimal minPrice, decimal maxPrice, int page = 1)
        {
            var result = await _storeService.SearchProductsByPriceRangeAsync(minPrice, maxPrice, page, 20);
            return Json(new { success = result.Success, products = result.Products, totalCount = result.TotalCount });
        }

        private async Task<List<string>> GetProductCategoriesAsync()
        {
            try
            {
                return await _context.ProductInfos
                    .Select(p => p.ProductType)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product categories");
                return new List<string>();
            }
        }
    }

    public class StoreIndexViewModel
    {
        public List<ProductInfo> Products { get; set; } = new List<ProductInfo>();
        public List<string> Categories { get; set; } = new List<string>();
        public string CurrentCategory { get; set; } = string.Empty;
        public int CurrentPage { get; set; }
    }

    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<Coupon> AvailableCoupons { get; set; } = new List<Coupon>();
    }

    public class CheckoutViewModel
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
    }
}