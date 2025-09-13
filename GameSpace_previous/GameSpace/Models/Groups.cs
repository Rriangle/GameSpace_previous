using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 群組表
/// </summary>
public partial class Groups
{
    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 群組擁有者用戶ID
    /// </summary>
    public int OwnerUserId { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    public string GroupName { get; set; } = null!;

    /// <summary>
    /// 群組描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 群組頭像URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 群組封面URL
    /// </summary>
    public string? CoverUrl { get; set; }

    /// <summary>
    /// 群組類型
    /// </summary>
    public string? GroupType { get; set; }

    /// <summary>
    /// 群組分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 群組標籤
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// 群組狀態
    /// </summary>
    public string Status { get; set; } = "active";

    /// <summary>
    /// 是否公開
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 是否需要審核加入
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// 最大成員數
    /// </summary>
    public int? MaxMembers { get; set; }

    /// <summary>
    /// 當前成員數
    /// </summary>
    public int CurrentMembers { get; set; }

    /// <summary>
    /// 群組規則
    /// </summary>
    public string? Rules { get; set; }

    /// <summary>
    /// 群組設定（JSON格式）
    /// </summary>
    public string? Settings { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

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
    /// 群組等級
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// 群組經驗值
    /// </summary>
    public int? Experience { get; set; }

    /// <summary>
    /// 群組積分
    /// </summary>
    public int? Points { get; set; }

    /// <summary>
    /// 群組排名
    /// </summary>
    public int? Ranking { get; set; }

    /// <summary>
    /// 群組評分
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    /// 評分人數
    /// </summary>
    public int? RatingCount { get; set; }

    /// <summary>
    /// 群組瀏覽次數
    /// </summary>
    public int? ViewCount { get; set; }

    /// <summary>
    /// 群組加入次數
    /// </summary>
    public int? JoinCount { get; set; }

    /// <summary>
    /// 群組離開次數
    /// </summary>
    public int? LeaveCount { get; set; }

    /// <summary>
    /// 群組位置
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 群組網站
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// 群組聯絡方式
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// 群組語言
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// 群組時區
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// 群組備註
    /// </summary>
    public string? Notes { get; set; }
}