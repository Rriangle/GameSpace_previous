using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    [Table("ManagerRole")]
    public class ManagerRole
    {
        [Key]
        [Column("Manager_Id")]
        public int ManagerId { get; set; }

        [Key]
        [Column("ManagerRole_Id")]
        public int ManagerRoleId { get; set; }

        // Navigation properties
        [ForeignKey("ManagerId")]
        public virtual ManagerData Manager { get; set; } = null!;

        [ForeignKey("ManagerRoleId")]
        public virtual ManagerRolePermission RolePermission { get; set; } = null!;
    }
}