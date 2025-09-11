using System.ComponentModel.DataAnnotations;

namespace GameSpace.Models
{
    /// <summary>
    /// ���~Ū���ҫ�
    /// </summary>
    public class ProductReadModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// �q��Ū���ҫ�
    /// </summary>
    public class OrderReadModel
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
    }

    /// <summary>
    /// ���~��TŪ���ҫ�
    /// </summary>
    public class ProductInfoReadModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    /// <summary>
    /// �q���TŪ���ҫ�
    /// </summary>
    public class OrderInfoReadModel
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public List<OrderItemReadModel> Items { get; set; } = new();
    }

    /// <summary>
    /// �q�涵��Ū���ҫ�
    /// </summary>
    public class OrderItemReadModel
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// ������Ū���ҫ�
    /// </summary>
    public class SupplierReadModel
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ���a�������~Ū���ҫ�
    /// </summary>
    public class PlayerMarketProductReadModel
    {
        public int ProductID { get; set; }
        public int SellerID { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
    }

    /// <summary>
    /// ���a�����q��Ū���ҫ�
    /// </summary>
    public class PlayerMarketOrderReadModel
    {
        public int OrderID { get; set; }
        public int BuyerID { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int SellerID { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public List<PlayerMarketOrderItemReadModel> Items { get; set; } = new();
    }

    /// <summary>
    /// ���a�����q�涵��Ū���ҫ�
    /// </summary>
    public class PlayerMarketOrderItemReadModel
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// �C������Ū���ҫ�
    /// </summary>
    public class GameMetricsReadModel
    {
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public int TotalPlayers { get; set; }
        public int ActivePlayers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// �H�����Ū���ҫ�
    /// </summary>
    public class PopularityIndexReadModel
    {
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public decimal PopularityScore { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int ShareCount { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// �Ʀ�]�ַ�Ū���ҫ�
    /// </summary>
    public class LeaderboardSnapshotReadModel
    {
        public int SnapshotID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PlayerID { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int Rank { get; set; }
        public DateTime SnapshotDate { get; set; }
    }
}
