using System;
using System.Collections.Generic;

namespace GameSpace.Models;

/// <summary>
/// 管理員資料表
/// </summary>
public partial class ManagerData
{
    /// <summary>
    /// 管理員ID
    /// </summary>
    public int ManagerId { get; set; }

    /// <summary>
    /// 管理員名稱
    /// </summary>
    public string? ManagerName { get; set; }

    /// <summary>
    /// 管理員帳號
    /// </summary>
    public string ManagerAccount { get; set; } = null!;

    /// <summary>
    /// 管理員密碼
    /// </summary>
    public string ManagerPassword { get; set; } = null!;

    /// <summary>
    /// 管理員電子郵件
    /// </summary>
    public string? ManagerEmail { get; set; }

    /// <summary>
    /// 電子郵件是否已確認
    /// </summary>
    public bool ManagerEmailConfirmed { get; set; }

    /// <summary>
    /// 管理員電話
    /// </summary>
    public string? ManagerPhone { get; set; }

    /// <summary>
    /// 電話是否已確認
    /// </summary>
    public bool ManagerPhoneConfirmed { get; set; }

    /// <summary>
    /// 管理員狀態
    /// </summary>
    public string ManagerStatus { get; set; } = "active";

    /// <summary>
    /// 管理員角色
    /// </summary>
    public string? ManagerRole { get; set; }

    /// <summary>
    /// 管理員權限
    /// </summary>
    public string? ManagerPermissions { get; set; }

    /// <summary>
    /// 管理員部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 管理員職位
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 管理員等級
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// 管理員頭像URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 管理員個人資料
    /// </summary>
    public string? Profile { get; set; }

    /// <summary>
    /// 管理員備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 註冊日期
    /// </summary>
    public DateTime? AdministratorRegistrationDate { get; set; }

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 最後登入IP
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 登入失敗次數
    /// </summary>
    public int ManagerAccessFailedCount { get; set; }

    /// <summary>
    /// 是否啟用鎖定
    /// </summary>
    public bool ManagerLockoutEnabled { get; set; }

    /// <summary>
    /// 鎖定結束時間
    /// </summary>
    public DateTime? ManagerLockoutEnd { get; set; }

    /// <summary>
    /// 是否啟用雙重驗證
    /// </summary>
    public bool ManagerTwoFactorEnabled { get; set; }

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
    /// 管理員設定（JSON格式）
    /// </summary>
    public string? Settings { get; set; }

    /// <summary>
    /// 管理員偏好設定（JSON格式）
    /// </summary>
    public string? Preferences { get; set; }

    /// <summary>
    /// 管理員工作時間
    /// </summary>
    public string? WorkSchedule { get; set; }

    /// <summary>
    /// 管理員聯絡方式
    /// </summary>
    public string? ContactInfo { get; set; }

    /// <summary>
    /// 管理員緊急聯絡人
    /// </summary>
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// 管理員地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 管理員生日
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 管理員性別
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 管理員身分證號
    /// </summary>
    public string? IdNumber { get; set; }

    /// <summary>
    /// 管理員銀行帳號
    /// </summary>
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// 管理員銀行代碼
    /// </summary>
    public string? BankCode { get; set; }
}