using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    [Table("ManagerData")]
    public class ManagerData
    {
        [Key]
        [Column("Manager_Id")]
        public int ManagerId { get; set; }

        [Column("Manager_Name")]
        [StringLength(30)]
        public string? ManagerName { get; set; }

        [Column("Manager_Account")]
        [StringLength(30)]
        public string? ManagerAccount { get; set; }

        [Column("Manager_Password")]
        [StringLength(200)]
        public string? ManagerPassword { get; set; }

        [Column("Manager_Email")]
        [Required]
        [StringLength(255)]
        public string ManagerEmail { get; set; } = null!;

        [Column("Manager_EmailConfirmed")]
        public bool ManagerEmailConfirmed { get; set; }

        [Column("Manager_AccessFailedCount")]
        public int ManagerAccessFailedCount { get; set; }

        [Column("Manager_LockoutEnabled")]
        public bool ManagerLockoutEnabled { get; set; }

        [Column("Manager_LockoutEnd")]
        public DateTime? ManagerLockoutEnd { get; set; }

        // Navigation properties
        public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
    }
}