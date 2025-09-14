using GameSpace.Data;
using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameSpace.Services
{
    public class RBACService
    {
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<RBACService> _logger;

        public RBACService(GameSpaceDbContext context, ILogger<RBACService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 檢查管理員是否具有特定權限
        /// </summary>
        /// <param name="managerId">管理員ID</param>
        /// <param name="permission">權限名稱</param>
        /// <returns>是否具有權限</returns>
        public async Task<bool> HasPermissionAsync(int managerId, string permission)
        {
            try
            {
                var hasPermission = await _context.ManagerRoles
                    .Include(mr => mr.RolePermission)
                    .Where(mr => mr.ManagerId == managerId)
                    .AnyAsync(mr => GetPermissionValue(mr.RolePermission, permission) == true);

                _logger.LogInformation("檢查管理員 {ManagerId} 權限 {Permission}: {Result}", 
                    managerId, permission, hasPermission);

                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查管理員 {ManagerId} 權限 {Permission} 時發生錯誤", 
                    managerId, permission);
                return false;
            }
        }

        /// <summary>
        /// 檢查管理員是否具有多個權限中的任意一個
        /// </summary>
        /// <param name="managerId">管理員ID</param>
        /// <param name="permissions">權限列表</param>
        /// <returns>是否具有任一權限</returns>
        public async Task<bool> HasAnyPermissionAsync(int managerId, params string[] permissions)
        {
            try
            {
                var hasAnyPermission = await _context.ManagerRoles
                    .Include(mr => mr.RolePermission)
                    .Where(mr => mr.ManagerId == managerId)
                    .AnyAsync(mr => permissions.Any(p => GetPermissionValue(mr.RolePermission, p) == true));

                _logger.LogInformation("檢查管理員 {ManagerId} 權限 {Permissions}: {Result}", 
                    managerId, string.Join(", ", permissions), hasAnyPermission);

                return hasAnyPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查管理員 {ManagerId} 權限 {Permissions} 時發生錯誤", 
                    managerId, string.Join(", ", permissions));
                return false;
            }
        }

        /// <summary>
        /// 檢查管理員是否具有所有指定權限
        /// </summary>
        /// <param name="managerId">管理員ID</param>
        /// <param name="permissions">權限列表</param>
        /// <returns>是否具有所有權限</returns>
        public async Task<bool> HasAllPermissionsAsync(int managerId, params string[] permissions)
        {
            try
            {
                var managerRoles = await _context.ManagerRoles
                    .Include(mr => mr.RolePermission)
                    .Where(mr => mr.ManagerId == managerId)
                    .Select(mr => mr.RolePermission)
                    .ToListAsync();

                var hasAllPermissions = permissions.All(permission =>
                    managerRoles.Any(role => GetPermissionValue(role, permission) == true));

                _logger.LogInformation("檢查管理員 {ManagerId} 所有權限 {Permissions}: {Result}", 
                    managerId, string.Join(", ", permissions), hasAllPermissions);

                return hasAllPermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查管理員 {ManagerId} 所有權限 {Permissions} 時發生錯誤", 
                    managerId, string.Join(", ", permissions));
                return false;
            }
        }

        /// <summary>
        /// 獲取管理員的所有權限
        /// </summary>
        /// <param name="managerId">管理員ID</param>
        /// <returns>權限列表</returns>
        public async Task<List<string>> GetManagerPermissionsAsync(int managerId)
        {
            try
            {
                var permissions = new List<string>();
                var managerRoles = await _context.ManagerRoles
                    .Include(mr => mr.RolePermission)
                    .Where(mr => mr.ManagerId == managerId)
                    .Select(mr => mr.RolePermission)
                    .ToListAsync();

                foreach (var role in managerRoles)
                {
                    if (role.AdministratorPrivilegesManagement == true)
                        permissions.Add("AdministratorPrivilegesManagement");
                    if (role.UserStatusManagement == true)
                        permissions.Add("UserStatusManagement");
                    if (role.ShoppingPermissionManagement == true)
                        permissions.Add("ShoppingPermissionManagement");
                    if (role.MessagePermissionManagement == true)
                        permissions.Add("MessagePermissionManagement");
                    if (role.PetRightsManagement == true)
                        permissions.Add("PetRightsManagement");
                    if (role.CustomerService == true)
                        permissions.Add("CustomerService");
                }

                _logger.LogInformation("管理員 {ManagerId} 具有權限: {Permissions}", 
                    managerId, string.Join(", ", permissions));

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取管理員 {ManagerId} 權限時發生錯誤", managerId);
                return new List<string>();
            }
        }

        /// <summary>
        /// 為管理員分配角色
        /// </summary>
        /// <param name="managerId">管理員ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AssignRoleAsync(int managerId, int roleId)
        {
            try
            {
                // 檢查是否已存在該角色分配
                var existingRole = await _context.ManagerRoles
                    .FirstOrDefaultAsync(mr => mr.ManagerId == managerId && mr.ManagerRoleId == roleId);

                if (existingRole != null)
                {
                    _logger.LogWarning("管理員 {ManagerId} 已具有角色 {RoleId}", managerId, roleId);
                    return true; // 已存在，視為成功
                }

                var managerRole = new ManagerRole
                {
                    ManagerId = managerId,
                    ManagerRoleId = roleId
                };

                _context.ManagerRoles.Add(managerRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功為管理員 {ManagerId} 分配角色 {RoleId}", managerId, roleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "為管理員 {ManagerId} 分配角色 {RoleId} 時發生錯誤", 
                    managerId, roleId);
                return false;
            }
        }

        /// <summary>
        /// 移除管理員的角色
        /// </summary>
        /// <param name="managerId">管理員ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> RemoveRoleAsync(int managerId, int roleId)
        {
            try
            {
                var managerRole = await _context.ManagerRoles
                    .FirstOrDefaultAsync(mr => mr.ManagerId == managerId && mr.ManagerRoleId == roleId);

                if (managerRole == null)
                {
                    _logger.LogWarning("管理員 {ManagerId} 不具有角色 {RoleId}", managerId, roleId);
                    return true; // 不存在，視為成功
                }

                _context.ManagerRoles.Remove(managerRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功移除管理員 {ManagerId} 的角色 {RoleId}", managerId, roleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除管理員 {ManagerId} 角色 {RoleId} 時發生錯誤", 
                    managerId, roleId);
                return false;
            }
        }

        /// <summary>
        /// 創建新的角色權限
        /// </summary>
        /// <param name="roleName">角色名稱</param>
        /// <param name="permissions">權限設定</param>
        /// <returns>角色ID</returns>
        public async Task<int?> CreateRoleAsync(string roleName, Dictionary<string, bool> permissions)
        {
            try
            {
                var rolePermission = new ManagerRolePermission
                {
                    RoleName = roleName,
                    AdministratorPrivilegesManagement = permissions.GetValueOrDefault("AdministratorPrivilegesManagement", false),
                    UserStatusManagement = permissions.GetValueOrDefault("UserStatusManagement", false),
                    ShoppingPermissionManagement = permissions.GetValueOrDefault("ShoppingPermissionManagement", false),
                    MessagePermissionManagement = permissions.GetValueOrDefault("MessagePermissionManagement", false),
                    PetRightsManagement = permissions.GetValueOrDefault("PetRightsManagement", false),
                    CustomerService = permissions.GetValueOrDefault("CustomerService", false)
                };

                _context.ManagerRolePermissions.Add(rolePermission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功創建角色 {RoleName}，ID: {RoleId}", 
                    roleName, rolePermission.ManagerRoleId);

                return rolePermission.ManagerRoleId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建角色 {RoleName} 時發生錯誤", roleName);
                return null;
            }
        }

        /// <summary>
        /// 從Claims中獲取管理員ID
        /// </summary>
        /// <param name="claimsPrincipal">Claims主體</param>
        /// <returns>管理員ID</returns>
        public int? GetManagerIdFromClaims(ClaimsPrincipal claimsPrincipal)
        {
            var managerIdClaim = claimsPrincipal.FindFirst("ManagerId");
            if (managerIdClaim != null && int.TryParse(managerIdClaim.Value, out int managerId))
            {
                return managerId;
            }
            return null;
        }

        /// <summary>
        /// 獲取權限值
        /// </summary>
        /// <param name="rolePermission">角色權限對象</param>
        /// <param name="permission">權限名稱</param>
        /// <returns>權限值</returns>
        private bool? GetPermissionValue(ManagerRolePermission rolePermission, string permission)
        {
            return permission switch
            {
                "AdministratorPrivilegesManagement" => rolePermission.AdministratorPrivilegesManagement,
                "UserStatusManagement" => rolePermission.UserStatusManagement,
                "ShoppingPermissionManagement" => rolePermission.ShoppingPermissionManagement,
                "MessagePermissionManagement" => rolePermission.MessagePermissionManagement,
                "PetRightsManagement" => rolePermission.PetRightsManagement,
                "CustomerService" => rolePermission.CustomerService,
                _ => null
            };
        }
    }
}