using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 私聊對話表
/// </summary>
public partial class DM_Conversations
{
    /// <summary>
    /// 對話ID
    /// </summary>
    public int ConversationId { get; set; }

    /// <summary>
    /// 是否為管理員私聊
    /// </summary>
    public bool IsManagerDm { get; set; }

    /// <summary>
    /// 參與者1 ID
    /// </summary>
    public int Party1Id { get; set; }

    /// <summary>
    /// 參與者2 ID
    /// </summary>
    public int Party2Id { get; set; }

    /// <summary>
    /// 最後訊息時間
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 是否為私人對話
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// 對話名稱
    /// </summary>
    public string? ConversationName { get; set; }

    /// <summary>
    /// 對話描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 發送者是否為參與者1
    /// </summary>
    public bool SenderIsParty1 { get; set; }

    /// <summary>
    /// 最後訊息ID
    /// </summary>
    public int? LastMessageId { get; set; }

    /// <summary>
    /// 訊息計數
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// 是否已封存
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// 封存時間
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// 封存原因
    /// </summary>
    public string? ArchiveReason { get; set; }
}