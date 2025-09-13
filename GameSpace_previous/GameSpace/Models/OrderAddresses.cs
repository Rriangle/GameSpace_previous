using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 訂單地址表
/// </summary>
public partial class OrderAddresses
{
    /// <summary>
    /// 訂單ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 收件人姓名
    /// </summary>
    public string Recipient { get; set; } = null!;

    /// <summary>
    /// 收件人電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 收件人電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 地址類型
    /// </summary>
    public string? AddressType { get; set; }

    /// <summary>
    /// 國家
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// 地址第一行
    /// </summary>
    public string Address1 { get; set; } = null!;

    /// <summary>
    /// 地址第二行
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// 完整地址
    /// </summary>
    public string? FullAddress { get; set; }

    /// <summary>
    /// 地址座標（緯度）
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 地址座標（經度）
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// 地址驗證狀態
    /// </summary>
    public string? ValidationStatus { get; set; }

    /// <summary>
    /// 地址驗證時間
    /// </summary>
    public DateTime? ValidatedAt { get; set; }

    /// <summary>
    /// 地址驗證者ID
    /// </summary>
    public int? ValidatedBy { get; set; }

    /// <summary>
    /// 地址備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 是否為預設地址
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 是否為帳單地址
    /// </summary>
    public bool IsBillingAddress { get; set; }

    /// <summary>
    /// 是否為配送地址
    /// </summary>
    public bool IsShippingAddress { get; set; }

    /// <summary>
    /// 配送時間偏好
    /// </summary>
    public string? DeliveryTimePreference { get; set; }

    /// <summary>
    /// 配送備註
    /// </summary>
    public string? DeliveryNotes { get; set; }

    /// <summary>
    /// 配送特殊要求
    /// </summary>
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// 配送費用
    /// </summary>
    public decimal? DeliveryFee { get; set; }

    /// <summary>
    /// 配送方式
    /// </summary>
    public string? DeliveryMethod { get; set; }

    /// <summary>
    /// 配送公司
    /// </summary>
    public string? DeliveryCompany { get; set; }

    /// <summary>
    /// 配送追蹤號碼
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// 配送狀態
    /// </summary>
    public string? DeliveryStatus { get; set; }

    /// <summary>
    /// 配送時間
    /// </summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// 配送簽收人
    /// </summary>
    public string? DeliveredTo { get; set; }

    /// <summary>
    /// 配送簽收時間
    /// </summary>
    public DateTime? SignedAt { get; set; }

    /// <summary>
    /// 配送簽收備註
    /// </summary>
    public string? SignatureNotes { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

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
    /// 是否已刪除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 刪除時間
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 刪除者ID
    /// </summary>
    public int? DeletedBy { get; set; }

    /// <summary>
    /// 地址設定（JSON格式）
    /// </summary>
    public string? Settings { get; set; }
}