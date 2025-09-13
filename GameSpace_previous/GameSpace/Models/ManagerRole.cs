using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 管理員角色模型
    /// </summary>
    public partial class ManagerRole
    {
        [Key]
        [Column("Manager_Id")]
        public int ManagerId { get; set; }

        [Key]
        [Column("ManagerRole_Id")]
        public int ManagerRoleId { get; set; }

        // 導航屬性
        [ForeignKey("ManagerId")]
        public virtual ManagerData Manager { get; set; } = null!;

        [ForeignKey("ManagerRoleId")]
        public virtual ManagerRolePermission ManagerRolePermission { get; set; } = null!;
    }
}