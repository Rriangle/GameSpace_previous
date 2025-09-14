using GameSpace.Models;

namespace GameSpace.Core.Repositories
{
    /// <summary>
    /// �ӫ������uŪ�s�x�w
    /// </summary>
    public interface ICommerceReadOnlyRepository
    {
        // �x��ӫ�
        Task<List<OfficialStoreRankingReadModel>> GetOfficialStoreRankingsAsync(string? periodType = null, int limit = 20);
        Task<List<ProductInfoReadModel>> GetProductsAsync(string? productType = null, int page = 1, int pageSize = 20);
        Task<ProductInfoReadModel?> GetProductByIdAsync(int productId);
        Task<List<GameProductDetailsReadModel>> GetGameProductDetailsAsync(int productId);
        Task<List<OtherProductDetailReadModel>> GetOtherProductDetailsAsync(int productId);
        Task<List<SupplierReadModel>> GetSuppliersAsync();
        Task<SupplierReadModel?> GetSupplierByIdAsync(int supplierId);
        
        // �q�����
        Task<List<OrderInfoReadModel>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 20);
        Task<OrderInfoReadModel?> GetOrderByIdAsync(int orderId);
        Task<List<OrderItemReadModel>> GetOrderItemsAsync(int orderId);
        Task<List<OrderInfoReadModel>> GetOrdersAsync(int page = 1, int pageSize = 20);
        
        // �ۥѥ���
        Task<List<PlayerMarketRankingReadModel>> GetPlayerMarketRankingsAsync(string? periodType = null, int limit = 20);
        Task<List<PlayerMarketProductInfoReadModel>> GetPlayerMarketProductsAsync(string? productType = null, int? sellerId = null, int page = 1, int pageSize = 20);
        Task<PlayerMarketProductInfoReadModel?> GetPlayerMarketProductByIdAsync(int productId);
        Task<List<PlayerMarketProductImgReadModel>> GetPlayerMarketProductImagesAsync(int productId);
        Task<List<PlayerMarketOrderInfoReadModel>> GetPlayerMarketOrdersAsync(int? sellerId = null, int? buyerId = null, int page = 1, int pageSize = 20);
        Task<PlayerMarketOrderInfoReadModel?> GetPlayerMarketOrderByIdAsync(int orderId);
        Task<List<PlayerMarketOrderTradepageReadModel>> GetPlayerMarketOrderTradepagesAsync(int orderId);
        Task<List<PlayerMarketTradeMsgReadModel>> GetPlayerMarketTradeMessagesAsync(int tradepageId);
        
        // �ӫ~�ק��x
        Task<List<ProductInfoAuditLogReadModel>> GetProductAuditLogsAsync(int productId, int limit = 50);
        
        // �έp
        Task<int> GetProductCountAsync(string? productType = null);
        Task<int> GetOrderCountAsync(int? userId = null);
        Task<int> GetPlayerMarketProductCountAsync(string? productType = null, int? sellerId = null);
        Task<decimal> GetTotalSalesAsync(int? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
