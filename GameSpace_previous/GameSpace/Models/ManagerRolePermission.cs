using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    [Table("ManagerRolePermission")]
    public class ManagerRolePermission
    {
        [Key]
        [Column("ManagerRole_Id")]
        public int ManagerRoleId { get; set; }

        [Column("role_name")]
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        [Column("AdministratorPrivilegesManagement")]
        public bool? AdministratorPrivilegesManagement { get; set; }

        [Column("UserStatusManagement")]
        public bool? UserStatusManagement { get; set; }

        [Column("ShoppingPermissionManagement")]
        public bool? ShoppingPermissionManagement { get; set; }

        [Column("MessagePermissionManagement")]
        public bool? MessagePermissionManagement { get; set; }

        [Column("Pet_Rights_Management")]
        public bool? PetRightsManagement { get; set; }

        [Column("customer_service")]
        public bool? CustomerService { get; set; }

        // Navigation properties
        public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
    }
}