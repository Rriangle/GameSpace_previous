using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 通知表
/// </summary>
public partial class Notification
{
    /// <summary>
    /// 通知ID
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// 來源ID
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 通知內容
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 通知類型
    /// </summary>
    public string NotificationType { get; set; } = null!;

    /// <summary>
    /// 通知優先級
    /// </summary>
    public string Priority { get; set; } = "normal";

    /// <summary>
    /// 通知狀態
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 已讀時間
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 已讀者ID
    /// </summary>
    public int? ReadBy { get; set; }

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// 發送者用戶ID
    /// </summary>
    public int? SenderUserId { get; set; }

    /// <summary>
    /// 發送者管理員ID
    /// </summary>
    public int? SenderManagerId { get; set; }

    /// <summary>
    /// 目標類型
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// 目標ID
    /// </summary>
    public int? TargetId { get; set; }

    /// <summary>
    /// 通知動作
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 動作URL
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// 通知資料（JSON格式）
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// 通知標籤
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 通知分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 通知子分類
    /// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    /// 通知頻道
    /// </summary>
    public string? Channel { get; set; }

    /// <summary>
    /// 通知平台
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// 通知語言
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// 通知時區
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 過期時間
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// 是否已過期
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// 是否已發送
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// 發送失敗次數
    /// </summary>
    public int? SendFailureCount { get; set; }

    /// <summary>
    /// 最後發送嘗試時間
    /// </summary>
    public DateTime? LastSendAttemptAt { get; set; }

    /// <summary>
    /// 發送錯誤訊息
    /// </summary>
    public string? SendErrorMessage { get; set; }

    /// <summary>
    /// 是否已取消
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// 取消時間
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// 取消者ID
    /// </summary>
    public int? CancelledBy { get; set; }

    /// <summary>
    /// 取消原因
    /// </summary>
    public string? CancelReason { get; set; }

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

    /// <summary>
    /// 通知備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 通知設定（JSON格式）
    /// </summary>
    public string? Settings { get; set; }
}