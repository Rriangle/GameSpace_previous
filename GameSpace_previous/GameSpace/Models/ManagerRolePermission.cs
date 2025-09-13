using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 管理員角色權限模型
    /// </summary>
    public partial class ManagerRolePermission
    {
        [Key]
        [Column("ManagerRole_Id")]
        public int ManagerRoleId { get; set; }

        [StringLength(100)]
        [Column("RoleName")]
        public string? RoleName { get; set; }

        [StringLength(500)]
        [Column("RoleDescription")]
        public string? RoleDescription { get; set; }

        [Column("AdministratorPrivilegesManagement")]
        public bool AdministratorPrivilegesManagement { get; set; }

        [Column("UserManagement")]
        public bool UserManagement { get; set; }

        [Column("ContentManagement")]
        public bool ContentManagement { get; set; }

        [Column("SystemMonitoring")]
        public bool SystemMonitoring { get; set; }

        [Column("OrderManagement")]
        public bool OrderManagement { get; set; }

        [Column("CouponManagement")]
        public bool CouponManagement { get; set; }

        [Column("EVoucherManagement")]
        public bool EVoucherManagement { get; set; }

        [Column("NotificationManagement")]
        public bool NotificationManagement { get; set; }

        [Column("ReportManagement")]
        public bool ReportManagement { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
    }
}