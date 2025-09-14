using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 禁言表
/// </summary>
public partial class Mute
{
    /// <summary>
    /// 禁言ID
    /// </summary>
    public int MuteId { get; set; }

    /// <summary>
    /// 禁言詞彙
    /// </summary>
    public string Word { get; set; } = null!;

    /// <summary>
    /// 禁言類型
    /// </summary>
    public string? MuteType { get; set; }

    /// <summary>
    /// 禁言原因
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 禁言狀態
    /// </summary>
    public string Status { get; set; } = "active";

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 建立者管理員ID
    /// </summary>
    public int? CreatedByManagerId { get; set; }

    /// <summary>
    /// 建立者用戶ID
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新者管理員ID
    /// </summary>
    public int? UpdatedByManagerId { get; set; }

    /// <summary>
    /// 更新者用戶ID
    /// </summary>
    public int? UpdatedByUserId { get; set; }

    /// <summary>
    /// 禁言開始時間
    /// </summary>
    public DateTime? MuteStartAt { get; set; }

    /// <summary>
    /// 禁言結束時間
    /// </summary>
    public DateTime? MuteEndAt { get; set; }

    /// <summary>
    /// 是否永久禁言
    /// </summary>
    public bool IsPermanent { get; set; }

    /// <summary>
    /// 禁言範圍
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// 禁言目標類型
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// 禁言目標ID
    /// </summary>
    public int? TargetId { get; set; }

    /// <summary>
    /// 禁言嚴重程度
    /// </summary>
    public string? Severity { get; set; }

    /// <summary>
    /// 禁言次數
    /// </summary>
    public int? MuteCount { get; set; }

    /// <summary>
    /// 最後觸發時間
    /// </summary>
    public DateTime? LastTriggeredAt { get; set; }

    /// <summary>
    /// 觸發次數
    /// </summary>
    public int? TriggerCount { get; set; }

    /// <summary>
    /// 是否自動禁言
    /// </summary>
    public bool IsAutoMute { get; set; }

    /// <summary>
    /// 自動禁言條件
    /// </summary>
    public string? AutoMuteCondition { get; set; }

    /// <summary>
    /// 禁言備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 禁言標籤
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 禁言分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 禁言優先級
    /// </summary>
    public int? Priority { get; set; }

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
    /// 刪除原因
    /// </summary>
    public string? DeleteReason { get; set; }
}