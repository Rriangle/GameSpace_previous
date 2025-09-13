using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameSpace.Models
{
    /// <summary>
    /// 管理員數據模型
    /// </summary>
    public partial class ManagerData
    {
        [Key]
        [Column("Manager_Id")]
        public int ManagerId { get; set; }

        [StringLength(30)]
        [Column("Manager_Name")]
        public string? ManagerName { get; set; }

        [StringLength(30)]
        [Column("Manager_Account")]
        public string? ManagerAccount { get; set; }

        [StringLength(200)]
        [Column("Manager_Password")]
        public string? ManagerPassword { get; set; }

        [Column("Administrator_registration_date")]
        public DateTime? AdministratorRegistrationDate { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Manager_Email")]
        public string ManagerEmail { get; set; } = null!;

        [Column("Manager_EmailConfirmed")]
        public bool ManagerEmailConfirmed { get; set; }

        [Column("Manager_AccessFailedCount")]
        public int ManagerAccessFailedCount { get; set; }

        [Column("Manager_LockoutEnabled")]
        public bool ManagerLockoutEnabled { get; set; }

        [Column("Manager_LockoutEnd")]
        public DateTime? ManagerLockoutEnd { get; set; }

        // 導航屬性
        public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
    }
}