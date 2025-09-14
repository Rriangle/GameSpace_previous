using System.ComponentModel.DataAnnotations;

namespace GameSpace.Models
{
    /// <summary>
    /// �g�A�C��Ū���ҫ�
    /// </summary>
    public class MiniGameReadModel
    {
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string GameType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsReward { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// �Τ���]Ū���ҫ�
    /// </summary>
    public class UserWalletReadModel
    {
        public int WalletID { get; set; }
        public int UserID { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// �Τ�ñ��έpŪ���ҫ�
    /// </summary>
    public class UserSignInStatsReadModel
    {
        public int StatID { get; set; }
        public int UserID { get; set; }
        public DateTime SignInDate { get; set; }
        public int PointsEarned { get; set; }
        public int ConsecutiveDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// �Q�צ�K��Ū���ҫ�
    /// </summary>
    public class ThreadPostReadModel
    {
        public long PostID { get; set; }
        public long ThreadID { get; set; }
        public string ThreadTitle { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// �K����Чַ�Ū���ҫ�
    /// </summary>
    public class PostMetricSnapshotReadModel
    {
        public long SnapshotID { get; set; }
        public long PostID { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int ShareCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime SnapshotDate { get; set; }
    }

    /// <summary>
    /// �K��ӷ�Ū���ҫ�
    /// </summary>
    public class PostSourceReadModel
    {
        public long SourceID { get; set; }
        public long PostID { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;
        public string SourceTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// �s�ղ��Ū���ҫ�
    /// </summary>
    public class GroupChatReadModel
    {
        public long ChatID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int SenderID { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// �s��Ū���ҫ�
    /// </summary>
    public class GroupReadModel
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public int CreatorID { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// �s�զ���Ū���ҫ�
    /// </summary>
    public class GroupMemberReadModel
    {
        public long MemberID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// �s�ի���Ū���ҫ�
    /// </summary>
    public class GroupBlockReadModel
    {
        public long BlockID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int BlockedUserID { get; set; }
        public string BlockedUserName { get; set; } = string.Empty;
        public int BlockerID { get; set; }
        public string BlockerName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime BlockedAt { get; set; }
    }

    /// <summary>
    /// ���YŪ���ҫ�
    /// </summary>
    public class RelationReadModel
    {
        public long RelationID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RelatedUserID { get; set; }
        public string RelatedUserName { get; set; } = string.Empty;
        public string RelationType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ���Y���AŪ���ҫ�
    /// </summary>
    public class RelationStatusReadModel
    {
        public long StatusID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// �x��ө��ƦWŪ���ҫ�
    /// </summary>
    public class OfficialStoreRankingReadModel
    {
        public int RankingID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Rank { get; set; }
        public decimal Score { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime RankingDate { get; set; }
    }

    /// <summary>
    /// �C�����~�Ա�Ū���ҫ�
    /// </summary>
    public class GameProductDetailsReadModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ��L���~�Ա�Ū���ҫ�
    /// </summary>
    public class OtherProductDetailReadModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ���a�����ƦWŪ���ҫ�
    /// </summary>
    public class PlayerMarketRankingReadModel
    {
        public int RankingID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int SellerID { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int Rank { get; set; }
        public decimal Score { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime RankingDate { get; set; }
    }

    /// <summary>
    /// ���a�������~��TŪ���ҫ�
    /// </summary>
    public class PlayerMarketProductInfoReadModel
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
        public int GameID { get; set; }
        public string GameName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
    }

    /// <summary>
    /// ���a�������~�Ϥ�Ū���ҫ�
    /// </summary>
    public class PlayerMarketProductImgReadModel
    {
        public long ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ���a�����q���TŪ���ҫ�
    /// </summary>
    public class PlayerMarketOrderInfoReadModel
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
    /// ���a�����q��������Ū���ҫ�
    /// </summary>
    public class PlayerMarketOrderTradepageReadModel
    {
        public long TradePageID { get; set; }
        public int OrderID { get; set; }
        public string TradeCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// ���a��������T��Ū���ҫ�
    /// </summary>
    public class PlayerMarketTradeMsgReadModel
    {
        public long MessageID { get; set; }
        public int OrderID { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    /// <summary>
    /// ���~��T�f�p��xŪ���ҫ�
    /// </summary>
    public class ProductInfoAuditLogReadModel
    {
        public long LogID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// �u�f������Ū���ҫ�
    /// </summary>
    public class CouponTypeReadModel
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// �q�l§������Ū���ҫ�
    /// </summary>
    public class EVoucherTypeReadModel
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
