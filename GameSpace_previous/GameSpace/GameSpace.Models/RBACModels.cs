using System.Security.Claims;

namespace GameSpace.Models
{
    /// <summary>
    /// ����ҫ�
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<Permission> Permissions { get; set; } = new();
    }

    /// <summary>
    /// �v���ҫ�
    /// </summary>
    public class Permission
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// �Τᨤ�����p�ҫ�
    /// </summary>
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// �����v�����p�ҫ�
    /// </summary>
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
