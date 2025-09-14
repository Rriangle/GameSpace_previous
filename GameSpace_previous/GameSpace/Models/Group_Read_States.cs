using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 群組已讀狀態表
/// </summary>
public partial class Group_Read_States
{
    /// <summary>
    /// 狀態ID
    /// </summary>
    public int StateId { get; set; }

    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 用戶ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 最後已讀訊息ID
    /// </summary>
    public int? LastReadMessageId { get; set; }

    /// <summary>
    /// 最後已讀時間
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    /// <summary>
    /// 未讀訊息數量
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 是否已靜音
    /// </summary>
    public bool IsMuted { get; set; }

    /// <summary>
    /// 靜音開始時間
    /// </summary>
    public DateTime? MutedAt { get; set; }

    /// <summary>
    /// 靜音結束時間
    /// </summary>
    public DateTime? MuteExpiresAt { get; set; }

    /// <summary>
    /// 是否已封鎖群組
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// 封鎖時間
    /// </summary>
    public DateTime? BlockedAt { get; set; }

    /// <summary>
    /// 封鎖原因
    /// </summary>
    public string? BlockReason { get; set; }

    /// <summary>
    /// 是否已離開群組
    /// </summary>
    public bool HasLeft { get; set; }

    /// <summary>
    /// 離開時間
    /// </summary>
    public DateTime? LeftAt { get; set; }

    /// <summary>
    /// 離開原因
    /// </summary>
    public string? LeaveReason { get; set; }

    /// <summary>
    /// 是否已加入群組
    /// </summary>
    public bool HasJoined { get; set; }

    /// <summary>
    /// 加入時間
    /// </summary>
    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// 用戶在群組中的角色
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// 用戶在群組中的權限
    /// </summary>
    public string? Permissions { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// 是否接收通知
    /// </summary>
    public bool ReceiveNotifications { get; set; }

    /// <summary>
    /// 通知設定（JSON格式）
    /// </summary>
    public string? NotificationSettings { get; set; }

    /// <summary>
    /// 群組暱稱
    /// </summary>
    public string? GroupNickname { get; set; }

    /// <summary>
    /// 個人備註
    /// </summary>
    public string? PersonalNote { get; set; }

    /// <summary>
    /// 是否已置頂
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// 置頂時間
    /// </summary>
    public DateTime? PinnedAt { get; set; }

    /// <summary>
    /// 置頂順序
    /// </summary>
    public int? PinOrder { get; set; }
}