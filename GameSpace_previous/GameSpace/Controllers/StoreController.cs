using Microsoft.AspNetCore.Mvc;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers
{
    public class StoreController : Controller
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<StoreController> _logger;

        public StoreController(GameSpacedatabaseContext context, ILogger<StoreController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Store
        public async Task<IActionResult> Index(string? category, string? search, string? sortBy)
        {
            var products = _context.ProductInfos
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.ProductType == category);
            }

            // Search functionality
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search) || 
                    p.ProductDescription.Contains(search));
            }

            // Sorting
            products = sortBy switch
            {
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "name_asc" => products.OrderBy(p => p.ProductName),
                "name_desc" => products.OrderByDescending(p => p.ProductName),
                "newest" => products.OrderByDescending(p => p.ProductCreatedAt),
                _ => products.OrderBy(p => p.ProductName)
            };

            var productList = await products.Take(50).ToListAsync();
            var categories = await _context.ProductInfos
                .Select(p => p.ProductType)
                .Distinct()
                .ToListAsync();

            var viewModel = new StoreIndexViewModel
            {
                Products = productList,
                Categories = categories,
                SelectedCategory = category,
                SearchQuery = search,
                SortBy = sortBy
            };

            return View(viewModel);
        }

        // GET: Store/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.ProductInfos
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products
            var relatedProducts = await _context.ProductInfos
                .Include(p => p.ProductImages)
                .Where(p => p.ProductType == product.ProductType && p.ProductId != product.ProductId)
                .Take(4)
                .ToListAsync();

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }

        // GET: Store/Cart
        public IActionResult Cart()
        {
            // Get cart from session
            var cart = GetCartFromSession();
            return View(cart);
        }

        // POST: Store/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.ProductInfos.Find(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var cart = GetCartFromSession();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ProductImages?.FirstOrDefault()?.ProductimgUrl ?? ""
                });
            }

            SaveCartToSession(cart);

            return Json(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        // POST: Store/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCartToSession(cart);
            }

            return Json(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        // POST: Store/UpdateCartQuantity
        [HttpPost]
        public IActionResult UpdateCartQuantity(int productId, int quantity)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCartToSession(cart);
            }

            return Json(new { success = true, cartCount = cart.Items.Sum(i => i.Quantity) });
        }

        // GET: Store/Checkout
        public IActionResult Checkout()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCartFromSession();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Cart");
            }

            return View(cart);
        }

        // POST: Store/ProcessOrder
        [HttpPost]
        public async Task<IActionResult> ProcessOrder([Bind("ShippingAddress,PaymentMethod,Notes")] CheckoutRequest request)
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "User ID not found" });
            }

            var cart = GetCartFromSession();
            if (!cart.Items.Any())
            {
                return Json(new { success = false, message = "Cart is empty" });
            }

            // Create order
            var order = new OrderInfo
            {
                UserId = userId.Value,
                OrderDate = DateTime.UtcNow,
                OrderTotal = cart.Items.Sum(i => i.Price * i.Quantity),
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid"
            };

            _context.OrderInfos.Add(order);
            await _context.SaveChangesAsync();

            // Create order items
            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    LineNo = cart.Items.IndexOf(item) + 1
                };

                _context.OrderItems.Add(orderItem);
            }

            // Create shipping address
            var address = new OrderAddress
            {
                OrderId = order.OrderId,
                Recipient = request.ShippingAddress.Recipient,
                Address1 = request.ShippingAddress.Address1,
                Address2 = request.ShippingAddress.Address2,
                City = request.ShippingAddress.City,
                Country = request.ShippingAddress.Country,
                Zipcode = request.ShippingAddress.Zipcode,
                Phone = request.ShippingAddress.Phone
            };

            _context.OrderAddresses.Add(address);

            // Create payment transaction
            var payment = new PaymentTransaction
            {
                OrderId = order.OrderId,
                Amount = order.OrderTotal,
                Provider = request.PaymentMethod,
                Status = "Pending",
                TxnType = "Purchase",
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(payment);

            await _context.SaveChangesAsync();

            // Clear cart
            ClearCart();

            return Json(new { success = true, orderId = order.OrderId });
        }

        // GET: Store/Orders
        public async Task<IActionResult> Orders()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.OrderInfos
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: Store/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.OrderInfos
                .Include(o => o.OrderItems)
                .Include(o => o.OrderAddresses)
                .Include(o => o.PaymentTransactions)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private ShoppingCart GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("ShoppingCart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new ShoppingCart();
            }

            return System.Text.Json.JsonSerializer.Deserialize<ShoppingCart>(cartJson) ?? new ShoppingCart();
        }

        private void SaveCartToSession(ShoppingCart cart)
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("ShoppingCart", cartJson);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("ShoppingCart");
        }

        private int? GetCurrentUserId()
        {
            // This is a placeholder - implement actual user ID retrieval
            // based on your authentication system
            return null;
        }
    }

    public class StoreIndexViewModel
    {
        public List<ProductInfo> Products { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string? SelectedCategory { get; set; }
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public ProductInfo Product { get; set; } = null!;
        public List<ProductInfo> RelatedProducts { get; set; } = new();
    }

    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = "";
    }

    public class CheckoutRequest
    {
        public ShippingAddress ShippingAddress { get; set; } = new();
        public string PaymentMethod { get; set; } = "";
        public string? Notes { get; set; }
    }

    public class ShippingAddress
    {
        public string Recipient { get; set; } = "";
        public string Address1 { get; set; } = "";
        public string Address2 { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string Zipcode { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}