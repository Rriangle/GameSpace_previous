using GameSpace.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameSpace.Services.Store
{
    public interface IStoreService
    {
        // 商品管理
        Task<ProductResult> CreateProductAsync(string productName, string productType, decimal price, string currencyCode, int? shipmentQuantity, string createdBy);
        Task<ProductResult> GetProductAsync(int productId);
        Task<List<ProductInfo>> GetAllProductsAsync(int page = 1, int pageSize = 20);
        Task<List<ProductInfo>> GetProductsByTypeAsync(string productType, int page = 1, int pageSize = 20);
        Task<ProductResult> UpdateProductAsync(int productId, string productName, string productType, decimal price, string currencyCode, int? shipmentQuantity, string updatedBy);
        Task<ProductResult> DeleteProductAsync(int productId);
        
        // 購物車功能
        Task<CartResult> AddToCartAsync(int userId, int productId, int quantity);
        Task<CartResult> RemoveFromCartAsync(int userId, int productId);
        Task<CartResult> UpdateCartItemAsync(int userId, int productId, int quantity);
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task<CartResult> ClearCartAsync(int userId);
        
        // 訂單管理
        Task<OrderResult> CreateOrderAsync(int userId, List<OrderItem> orderItems, string shippingAddress, string paymentMethod);
        Task<OrderResult> GetOrderAsync(int orderId);
        Task<List<OrderInfo>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 20);
        Task<OrderResult> UpdateOrderStatusAsync(int orderId, string status, string updatedBy);
        Task<OrderResult> CancelOrderAsync(int orderId, int userId);
        
        // 優惠券使用
        Task<CouponResult> UseCouponAsync(int orderId, string couponCode);
        Task<CouponResult> RemoveCouponAsync(int orderId);
        Task<List<Coupon>> GetAvailableCouponsAsync(int userId);
        
        // 搜尋功能
        Task<SearchResult> SearchProductsAsync(string keyword, int page = 1, int pageSize = 20);
        Task<SearchResult> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int page = 1, int pageSize = 20);
        
        // 庫存管理
        Task<InventoryResult> UpdateInventoryAsync(int productId, int quantity);
        Task<InventoryResult> CheckInventoryAsync(int productId, int requestedQuantity);
    }

    public class ProductResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProductInfo? Product { get; set; }
    }

    public class CartResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CartItem? CartItem { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }

    public class OrderResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public OrderInfo? Order { get; set; }
    }

    public class CouponResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Coupon? Coupon { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public class SearchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ProductInfo>? Products { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class InventoryResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}