using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 遊戲商品詳情表
/// </summary>
public partial class GameProductDetails
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; } = null!;

    /// <summary>
    /// 遊戲ID
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 商品類型
    /// </summary>
    public string? ProductType { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 價格
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// 貨幣代碼
    /// </summary>
    public string? CurrencyCode { get; set; }

    /// <summary>
    /// 商品圖片URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 商品影片URL
    /// </summary>
    public string? VideoUrl { get; set; }

    /// <summary>
    /// 商品標籤
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 商品分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 商品子分類
    /// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    /// 商品狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 是否上架
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 上架時間
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// 下架時間
    /// </summary>
    public DateTime? UnpublishedAt { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 建立者ID
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// 更新者ID
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// 商品規格（JSON格式）
    /// </summary>
    public string? Specifications { get; set; }

    /// <summary>
    /// 商品特色
    /// </summary>
    public string? Features { get; set; }

    /// <summary>
    /// 商品需求
    /// </summary>
    public string? Requirements { get; set; }

    /// <summary>
    /// 商品評分
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    /// 評分人數
    /// </summary>
    public int? RatingCount { get; set; }

    /// <summary>
    /// 瀏覽次數
    /// </summary>
    public int? ViewCount { get; set; }

    /// <summary>
    /// 購買次數
    /// </summary>
    public int? PurchaseCount { get; set; }

    /// <summary>
    /// 商品重量
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// 商品尺寸
    /// </summary>
    public string? Dimensions { get; set; }

    /// <summary>
    /// 商品材質
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// 商品顏色
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// 商品品牌
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// 商品型號
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 商品條碼
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// 商品SKU
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// 商品庫存
    /// </summary>
    public int? Stock { get; set; }

    /// <summary>
    /// 最低庫存警告
    /// </summary>
    public int? MinStock { get; set; }

    /// <summary>
    /// 商品備註
    /// </summary>
    public string? Notes { get; set; }
}