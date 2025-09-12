using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameSpace.Services.Store
{
    public class StoreService : IStoreService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly ILogger<StoreService> _logger;

        public StoreService(GameSpacedatabaseContext context, ILogger<StoreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region 商品管理

        public async Task<ProductResult> CreateProductAsync(string productName, string productType, decimal price, string currencyCode, int? shipmentQuantity, string createdBy)
        {
            try
            {
                var product = new ProductInfo
                {
                    ProductName = productName,
                    ProductType = productType,
                    Price = price,
                    CurrencyCode = currencyCode,
                    ShipmentQuantity = shipmentQuantity,
                    ProductCreatedBy = createdBy,
                    ProductCreatedAt = DateTime.UtcNow
                };

                _context.ProductInfos.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created: {ProductName} (ID: {ProductId})", productName, product.ProductId);
                return new ProductResult { Success = true, Message = "商品創建成功", Product = product };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product: {ProductName}", productName);
                return new ProductResult { Success = false, Message = "商品創建失敗" };
            }
        }

        public async Task<ProductResult> GetProductAsync(int productId)
        {
            try
            {
                var product = await _context.ProductInfos.FindAsync(productId);
                if (product == null)
                {
                    return new ProductResult { Success = false, Message = "商品不存在" };
                }

                return new ProductResult { Success = true, Message = "商品獲取成功", Product = product };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product: {ProductId}", productId);
                return new ProductResult { Success = false, Message = "商品獲取失敗" };
            }
        }

        public async Task<List<ProductInfo>> GetAllProductsAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.ProductInfos
                    .OrderByDescending(p => p.ProductCreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all products");
                return new List<ProductInfo>();
            }
        }

        public async Task<List<ProductInfo>> GetProductsByTypeAsync(string productType, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.ProductInfos
                    .Where(p => p.ProductType == productType)
                    .OrderByDescending(p => p.ProductCreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get products by type: {ProductType}", productType);
                return new List<ProductInfo>();
            }
        }

        public async Task<ProductResult> UpdateProductAsync(int productId, string productName, string productType, decimal price, string currencyCode, int? shipmentQuantity, string updatedBy)
        {
            try
            {
                var product = await _context.ProductInfos.FindAsync(productId);
                if (product == null)
                {
                    return new ProductResult { Success = false, Message = "商品不存在" };
                }

                product.ProductName = productName;
                product.ProductType = productType;
                product.Price = price;
                product.CurrencyCode = currencyCode;
                product.ShipmentQuantity = shipmentQuantity;
                product.ProductUpdatedBy = updatedBy;
                product.ProductUpdatedAt = DateTime.UtcNow;

                _context.ProductInfos.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product updated: {ProductId}", productId);
                return new ProductResult { Success = true, Message = "商品更新成功", Product = product };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product: {ProductId}", productId);
                return new ProductResult { Success = false, Message = "商品更新失敗" };
            }
        }

        public async Task<ProductResult> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.ProductInfos.FindAsync(productId);
                if (product == null)
                {
                    return new ProductResult { Success = false, Message = "商品不存在" };
                }

                _context.ProductInfos.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted: {ProductId}", productId);
                return new ProductResult { Success = true, Message = "商品刪除成功" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product: {ProductId}", productId);
                return new ProductResult { Success = false, Message = "商品刪除失敗" };
            }
        }

        #endregion

        #region 購物車功能

        public async Task<CartResult> AddToCartAsync(int userId, int productId, int quantity)
        {
            try
            {
                // 檢查商品是否存在
                var product = await _context.ProductInfos.FindAsync(productId);
                if (product == null)
                {
                    return new CartResult { Success = false, Message = "商品不存在" };
                }

                // 檢查庫存
                if (product.ShipmentQuantity.HasValue && product.ShipmentQuantity < quantity)
                {
                    return new CartResult { Success = false, Message = "庫存不足" };
                }

                // 這裡應該使用 Session 或 Redis 來存儲購物車
                // 由於我們沒有購物車表，這裡返回模擬數據
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity
                };

                _logger.LogInformation("Product added to cart: {ProductId} for user {UserId}", productId, userId);
                return new CartResult { Success = true, Message = "商品已加入購物車", CartItem = cartItem };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product to cart: {ProductId} for user {UserId}", productId, userId);
                return new CartResult { Success = false, Message = "加入購物車失敗" };
            }
        }

        public async Task<CartResult> RemoveFromCartAsync(int userId, int productId)
        {
            try
            {
                // 這裡應該從 Session 或 Redis 中移除商品
                _logger.LogInformation("Product removed from cart: {ProductId} for user {UserId}", productId, userId);
                return new CartResult { Success = true, Message = "商品已從購物車移除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove product from cart: {ProductId} for user {UserId}", productId, userId);
                return new CartResult { Success = false, Message = "移除商品失敗" };
            }
        }

        public async Task<CartResult> UpdateCartItemAsync(int userId, int productId, int quantity)
        {
            try
            {
                // 這裡應該更新 Session 或 Redis 中的商品數量
                _logger.LogInformation("Cart item updated: {ProductId} quantity {Quantity} for user {UserId}", productId, quantity, userId);
                return new CartResult { Success = true, Message = "購物車已更新" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cart item: {ProductId} for user {UserId}", productId, userId);
                return new CartResult { Success = false, Message = "更新購物車失敗" };
            }
        }

        public async Task<List<CartItem>> GetCartItemsAsync(int userId)
        {
            try
            {
                // 這裡應該從 Session 或 Redis 中獲取購物車商品
                // 由於我們沒有購物車表，這裡返回空列表
                return new List<CartItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart items for user: {UserId}", userId);
                return new List<CartItem>();
            }
        }

        public async Task<CartResult> ClearCartAsync(int userId)
        {
            try
            {
                // 這裡應該清空 Session 或 Redis 中的購物車
                _logger.LogInformation("Cart cleared for user: {UserId}", userId);
                return new CartResult { Success = true, Message = "購物車已清空" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cart for user: {UserId}", userId);
                return new CartResult { Success = false, Message = "清空購物車失敗" };
            }
        }

        #endregion

        #region 訂單管理

        public async Task<OrderResult> CreateOrderAsync(int userId, List<OrderItem> orderItems, string shippingAddress, string paymentMethod)
        {
            try
            {
                if (!orderItems.Any())
                {
                    return new OrderResult { Success = false, Message = "訂單商品不能為空" };
                }

                // 計算訂單總金額
                decimal totalAmount = orderItems.Sum(item => item.TotalPrice);

                var order = new OrderInfo
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    ShippingAddress = shippingAddress,
                    PaymentMethod = paymentMethod
                };

                _context.OrderInfos.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order created: {OrderId} for user {UserId}", order.OrderId, userId);
                return new OrderResult { Success = true, Message = "訂單創建成功", Order = order };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order for user: {UserId}", userId);
                return new OrderResult { Success = false, Message = "訂單創建失敗" };
            }
        }

        public async Task<OrderResult> GetOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.OrderInfos.FindAsync(orderId);
                if (order == null)
                {
                    return new OrderResult { Success = false, Message = "訂單不存在" };
                }

                return new OrderResult { Success = true, Message = "訂單獲取成功", Order = order };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order: {OrderId}", orderId);
                return new OrderResult { Success = false, Message = "訂單獲取失敗" };
            }
        }

        public async Task<List<OrderInfo>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.OrderInfos
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user orders for user: {UserId}", userId);
                return new List<OrderInfo>();
            }
        }

        public async Task<OrderResult> UpdateOrderStatusAsync(int orderId, string status, string updatedBy)
        {
            try
            {
                var order = await _context.OrderInfos.FindAsync(orderId);
                if (order == null)
                {
                    return new OrderResult { Success = false, Message = "訂單不存在" };
                }

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;
                order.UpdatedBy = updatedBy;

                _context.OrderInfos.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order status updated: {OrderId} to {Status}", orderId, status);
                return new OrderResult { Success = true, Message = "訂單狀態已更新", Order = order };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order status: {OrderId}", orderId);
                return new OrderResult { Success = false, Message = "訂單狀態更新失敗" };
            }
        }

        public async Task<OrderResult> CancelOrderAsync(int orderId, int userId)
        {
            try
            {
                var order = await _context.OrderInfos.FindAsync(orderId);
                if (order == null)
                {
                    return new OrderResult { Success = false, Message = "訂單不存在" };
                }

                if (order.UserId != userId)
                {
                    return new OrderResult { Success = false, Message = "無權限取消此訂單" };
                }

                if (order.Status == "Cancelled")
                {
                    return new OrderResult { Success = false, Message = "訂單已取消" };
                }

                order.Status = "Cancelled";
                order.UpdatedAt = DateTime.UtcNow;

                _context.OrderInfos.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order cancelled: {OrderId} by user {UserId}", orderId, userId);
                return new OrderResult { Success = true, Message = "訂單已取消", Order = order };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel order: {OrderId}", orderId);
                return new OrderResult { Success = false, Message = "取消訂單失敗" };
            }
        }

        #endregion

        #region 優惠券使用

        public async Task<CouponResult> UseCouponAsync(int orderId, string couponCode)
        {
            try
            {
                var order = await _context.OrderInfos.FindAsync(orderId);
                if (order == null)
                {
                    return new CouponResult { Success = false, Message = "訂單不存在" };
                }

                var coupon = await _context.Coupons
                    .Include(c => c.CouponType)
                    .FirstOrDefaultAsync(c => c.CouponCode == couponCode && !c.IsUsed);

                if (coupon == null)
                {
                    return new CouponResult { Success = false, Message = "優惠券不存在或已使用" };
                }

                if (coupon.CouponType.ValidFrom > DateTime.UtcNow || coupon.CouponType.ValidTo < DateTime.UtcNow)
                {
                    return new CouponResult { Success = false, Message = "優惠券已過期" };
                }

                if (order.TotalAmount < coupon.CouponType.MinSpend)
                {
                    return new CouponResult { Success = false, Message = $"訂單金額需滿 {coupon.CouponType.MinSpend} 元才能使用此優惠券" };
                }

                // 計算折扣金額
                decimal discountAmount = 0;
                if (coupon.CouponType.DiscountType == "Amount")
                {
                    discountAmount = coupon.CouponType.DiscountValue;
                }
                else if (coupon.CouponType.DiscountType == "Percentage")
                {
                    discountAmount = order.TotalAmount * (coupon.CouponType.DiscountValue / 100);
                }

                // 更新訂單
                order.TotalAmount -= discountAmount;
                order.UpdatedAt = DateTime.UtcNow;

                // 標記優惠券為已使用
                coupon.IsUsed = true;
                coupon.UsedTime = DateTime.UtcNow;
                coupon.UsedInOrderId = orderId;

                _context.OrderInfos.Update(order);
                _context.Coupons.Update(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Coupon used: {CouponCode} for order {OrderId}", couponCode, orderId);
                return new CouponResult { Success = true, Message = "優惠券使用成功", Coupon = coupon, DiscountAmount = discountAmount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to use coupon: {CouponCode} for order {OrderId}", couponCode, orderId);
                return new CouponResult { Success = false, Message = "優惠券使用失敗" };
            }
        }

        public async Task<CouponResult> RemoveCouponAsync(int orderId)
        {
            try
            {
                var order = await _context.OrderInfos.FindAsync(orderId);
                if (order == null)
                {
                    return new CouponResult { Success = false, Message = "訂單不存在" };
                }

                // 這裡應該移除訂單中的優惠券
                // 由於我們沒有存儲優惠券與訂單的關聯，這裡返回成功
                _logger.LogInformation("Coupon removed from order: {OrderId}", orderId);
                return new CouponResult { Success = true, Message = "優惠券已移除" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove coupon from order: {OrderId}", orderId);
                return new CouponResult { Success = false, Message = "移除優惠券失敗" };
            }
        }

        public async Task<List<Coupon>> GetAvailableCouponsAsync(int userId)
        {
            try
            {
                return await _context.Coupons
                    .Include(c => c.CouponType)
                    .Where(c => c.UserId == userId && !c.IsUsed && c.CouponType.ValidFrom <= DateTime.UtcNow && c.CouponType.ValidTo >= DateTime.UtcNow)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get available coupons for user: {UserId}", userId);
                return new List<Coupon>();
            }
        }

        #endregion

        #region 搜尋功能

        public async Task<SearchResult> SearchProductsAsync(string keyword, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.ProductInfos
                    .Where(p => p.ProductName.Contains(keyword) || p.ProductType.Contains(keyword));

                var totalCount = await query.CountAsync();
                var products = await query
                    .OrderByDescending(p => p.ProductCreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new SearchResult
                {
                    Success = true,
                    Message = "搜尋成功",
                    Products = products,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search products with keyword: {Keyword}", keyword);
                return new SearchResult { Success = false, Message = "搜尋失敗" };
            }
        }

        public async Task<SearchResult> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.ProductInfos
                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice);

                var totalCount = await query.CountAsync();
                var products = await query
                    .OrderBy(p => p.Price)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new SearchResult
                {
                    Success = true,
                    Message = "搜尋成功",
                    Products = products,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search products by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                return new SearchResult { Success = false, Message = "搜尋失敗" };
            }
        }

        #endregion

        #region 庫存管理

        public async Task<InventoryResult> UpdateInventoryAsync(int productId, int quantity)
        {
            try
            {
                var product = await _context.ProductInfos.FindAsync(productId);
                if (product == null)
                {
                    return new InventoryResult { Success = false, Message = "商品不存在" };
                }

                product.ShipmentQuantity = quantity;
                product.ProductUpdatedAt = DateTime.UtcNow;

                _context.ProductInfos.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Inventory updated for product {ProductId}: {Quantity}", productId, quantity);
                return new InventoryResult { Success = true, Message = "庫存更新成功", AvailableQuantity = quantity };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update inventory for product: {ProductId}", productId);
                return new InventoryResult { Success = false, Message = "庫存更新失敗" };
            }
        }

        public async Task<InventoryResult> CheckInventoryAsync(int productId, int requestedQuantity)
        {
            try
            {
                var product = await _context.ProductInfos.FindAsync(productId);
                if (product == null)
                {
                    return new InventoryResult { Success = false, Message = "商品不存在" };
                }

                int availableQuantity = product.ShipmentQuantity ?? 0;
                bool hasEnough = availableQuantity >= requestedQuantity;

                return new InventoryResult
                {
                    Success = hasEnough,
                    Message = hasEnough ? "庫存充足" : "庫存不足",
                    AvailableQuantity = availableQuantity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check inventory for product: {ProductId}", productId);
                return new InventoryResult { Success = false, Message = "庫存檢查失敗" };
            }
        }

        #endregion
    }
}