using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 私聊訊息表
/// </summary>
public partial class DM_Messages
{
    /// <summary>
    /// 訊息ID
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// 對話ID
    /// </summary>
    public int ConversationId { get; set; }

    /// <summary>
    /// 發送者用戶ID
    /// </summary>
    public int? SenderUserId { get; set; }

    /// <summary>
    /// 發送者管理員ID
    /// </summary>
    public int? SenderManagerId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string MessageText { get; set; } = null!;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 已讀時間
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 已讀用戶ID
    /// </summary>
    public int? ReadByUserId { get; set; }

    /// <summary>
    /// 已讀管理員ID
    /// </summary>
    public int? ReadByManagerId { get; set; }

    /// <summary>
    /// 是否已編輯
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// 編輯時間
    /// </summary>
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// 是否已刪除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 刪除時間
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 刪除者用戶ID
    /// </summary>
    public int? DeletedByUserId { get; set; }

    /// <summary>
    /// 刪除者管理員ID
    /// </summary>
    public int? DeletedByManagerId { get; set; }

    /// <summary>
    /// 訊息類型
    /// </summary>
    public string MessageType { get; set; } = "text";

    /// <summary>
    /// 附件URL
    /// </summary>
    public string? AttachmentUrl { get; set; }

    /// <summary>
    /// 附件類型
    /// </summary>
    public string? AttachmentType { get; set; }

    /// <summary>
    /// 回覆的訊息ID
    /// </summary>
    public int? ReplyToMessageId { get; set; }

    /// <summary>
    /// 訊息狀態
    /// </summary>
    public string Status { get; set; } = "sent";

    /// <summary>
    /// 額外資料（JSON格式）
    /// </summary>
    public string? Metadata { get; set; }
}