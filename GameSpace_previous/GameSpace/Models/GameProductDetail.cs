using System;

namespace GameSpace.Models
{
    /// <summary>
    /// 遊戲商品詳情資料表
    /// </summary>
    public partial class GameProductDetail
    {
        public int ProductDetailId { get; set; }
        public int GameId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductDescription { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public string? ProductType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;
    }
}