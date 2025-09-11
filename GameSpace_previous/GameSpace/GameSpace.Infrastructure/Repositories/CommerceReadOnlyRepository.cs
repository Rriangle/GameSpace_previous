using Microsoft.EntityFrameworkCore;
using GameSpace.Models;
using GameSpace.Core.Repositories;
using GameSpace.Data;

namespace GameSpace.Infrastructure.Repositories
{
    public class CommerceReadOnlyRepository : ICommerceReadOnlyRepository
    {
        private readonly GameSpaceDbContext _context;

        public CommerceReadOnlyRepository(GameSpaceDbContext context)
        {
            _context = context;
        }

        // 官方商城相關 - 暫時返回空列表
        public async Task<List<OfficialStoreRankingReadModel>> GetOfficialStoreRankingsAsync(string? periodType = null, int limit = 20)
        {
            return await Task.FromResult(new List<OfficialStoreRankingReadModel>());
        }

        public async Task<List<ProductInfoReadModel>> GetProductsAsync(string? productType = null, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<ProductInfoReadModel>());
        }

        public async Task<ProductInfoReadModel?> GetProductByIdAsync(int productId)
        {
            return await Task.FromResult<ProductInfoReadModel?>(null);
        }

        public async Task<List<GameProductDetailsReadModel>> GetGameProductDetailsAsync(int productId)
        {
            return await Task.FromResult(new List<GameProductDetailsReadModel>());
        }

        public async Task<List<OtherProductDetailsReadModel>> GetOtherProductDetailsAsync(int productId)
        {
            return await Task.FromResult(new List<OtherProductDetailsReadModel>());
        }

        // 供應商相關 - 暫時返回空列表
        public async Task<List<SupplierReadModel>> GetSuppliersAsync()
        {
            return await Task.FromResult(new List<SupplierReadModel>());
        }

        public async Task<SupplierReadModel?> GetSupplierByIdAsync(int supplierId)
        {
            return await Task.FromResult<SupplierReadModel?>(null);
        }

        // 訂單相關 - 暫時返回空列表
        public async Task<List<OrderInfoReadModel>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<OrderInfoReadModel>());
        }

        public async Task<OrderInfoReadModel?> GetOrderByIdAsync(int orderId)
        {
            return await Task.FromResult<OrderInfoReadModel?>(null);
        }

        public async Task<List<OrderItemReadModel>> GetOrderItemsAsync(int orderId)
        {
            return await Task.FromResult(new List<OrderItemReadModel>());
        }

        public async Task<List<OrderInfoReadModel>> GetOrdersAsync(int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<OrderInfoReadModel>());
        }

        // 自由市場相關 - 暫時返回空列表
        public async Task<List<PlayerMarketRankingReadModel>> GetPlayerMarketRankingsAsync(string? periodType = null, int limit = 20)
        {
            return await Task.FromResult(new List<PlayerMarketRankingReadModel>());
        }

        public async Task<List<PlayerMarketProductInfoReadModel>> GetPlayerMarketProductsAsync(string? productType = null, int? sellerId = null, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<PlayerMarketProductInfoReadModel>());
        }

        public async Task<PlayerMarketProductInfoReadModel?> GetPlayerMarketProductByIdAsync(int productId)
        {
            return await Task.FromResult<PlayerMarketProductInfoReadModel?>(null);
        }

        public async Task<List<PlayerMarketProductImgReadModel>> GetPlayerMarketProductImagesAsync(int productId)
        {
            return await Task.FromResult(new List<PlayerMarketProductImgReadModel>());
        }

        public async Task<List<PlayerMarketOrderInfoReadModel>> GetPlayerMarketOrdersAsync(int? sellerId = null, int? buyerId = null, int page = 1, int pageSize = 20)
        {
            return await Task.FromResult(new List<PlayerMarketOrderInfoReadModel>());
        }

        public async Task<PlayerMarketOrderInfoReadModel?> GetPlayerMarketOrderByIdAsync(int orderId)
        {
            return await Task.FromResult<PlayerMarketOrderInfoReadModel?>(null);
        }

        public async Task<List<PlayerMarketOrderTradepageReadModel>> GetPlayerMarketOrderTradepagesAsync(int orderId)
        {
            return await Task.FromResult(new List<PlayerMarketOrderTradepageReadModel>());
        }

        public async Task<List<PlayerMarketTradeMsgReadModel>> GetPlayerMarketTradeMessagesAsync(int tradepageId)
        {
            return await Task.FromResult(new List<PlayerMarketTradeMsgReadModel>());
        }

        // 商品修改日誌相關 - 暫時返回空列表
        public async Task<List<ProductInfoAuditLogReadModel>> GetProductAuditLogsAsync(int productId, int limit = 50)
        {
            return await Task.FromResult(new List<ProductInfoAuditLogReadModel>());
        }

        // 統計相關 - 暫時返回0
        public async Task<int> GetProductCountAsync(string? productType = null)
        {
            return await Task.FromResult(0);
        }

        public async Task<int> GetOrderCountAsync(int? userId = null)
        {
            return await Task.FromResult(0);
        }

        public async Task<int> GetPlayerMarketProductCountAsync(string? productType = null, int? sellerId = null)
        {
            return await Task.FromResult(0);
        }

        public async Task<decimal> GetTotalSalesAsync(int? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await Task.FromResult(0m);
        }
    }
}
