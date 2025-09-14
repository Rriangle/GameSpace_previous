using GameSpace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace GameSpace.Attributes
{
    /// <summary>
    /// RBAC權限控制屬性
    /// </summary>
    public class RBACAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _requiredPermissions;
        private readonly bool _requireAll;

        /// <summary>
        /// 初始化RBAC權限控制屬性
        /// </summary>
        /// <param name="requiredPermissions">需要的權限列表</param>
        /// <param name="requireAll">是否需要所有權限（預設為false，即任一權限即可）</param>
        public RBACAuthorizeAttribute(bool requireAll = false, params string[] requiredPermissions)
        {
            _requiredPermissions = requiredPermissions;
            _requireAll = requireAll;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 檢查是否已通過身份驗證
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 檢查是否為管理員
            var isManager = context.HttpContext.User.IsInRole("Manager");
            if (!isManager)
            {
                context.Result = new ForbidResult();
                return;
            }

            // 如果沒有指定權限要求，則只需要管理員身份即可
            if (_requiredPermissions == null || _requiredPermissions.Length == 0)
            {
                return;
            }

            // 獲取RBAC服務
            var rbacService = context.HttpContext.RequestServices.GetService<RBACService>();
            if (rbacService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // 獲取管理員ID
            var managerId = rbacService.GetManagerIdFromClaims(context.HttpContext.User);
            if (managerId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // 檢查權限
            bool hasPermission;
            if (_requireAll)
            {
                hasPermission = await rbacService.HasAllPermissionsAsync(managerId.Value, _requiredPermissions);
            }
            else
            {
                hasPermission = await rbacService.HasAnyPermissionAsync(managerId.Value, _requiredPermissions);
            }

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    /// <summary>
    /// 超級管理員權限屬性
    /// </summary>
    public class SuperAdminAttribute : RBACAuthorizeAttribute
    {
        public SuperAdminAttribute() : base(true, "AdministratorPrivilegesManagement")
        {
        }
    }

    /// <summary>
    /// 用戶管理權限屬性
    /// </summary>
    public class UserManagementAttribute : RBACAuthorizeAttribute
    {
        public UserManagementAttribute() : base(false, "AdministratorPrivilegesManagement", "UserStatusManagement")
        {
        }
    }

    /// <summary>
    /// 商城管理權限屬性
    /// </summary>
    public class ShoppingManagementAttribute : RBACAuthorizeAttribute
    {
        public ShoppingManagementAttribute() : base(false, "AdministratorPrivilegesManagement", "ShoppingPermissionManagement")
        {
        }
    }

    /// <summary>
    /// 論壇管理權限屬性
    /// </summary>
    public class MessageManagementAttribute : RBACAuthorizeAttribute
    {
        public MessageManagementAttribute() : base(false, "AdministratorPrivilegesManagement", "MessagePermissionManagement")
        {
        }
    }

    /// <summary>
    /// 寵物系統管理權限屬性
    /// </summary>
    public class PetManagementAttribute : RBACAuthorizeAttribute
    {
        public PetManagementAttribute() : base(false, "AdministratorPrivilegesManagement", "PetRightsManagement")
        {
        }
    }

    /// <summary>
    /// 客服權限屬性
    /// </summary>
    public class CustomerServiceAttribute : RBACAuthorizeAttribute
    {
        public CustomerServiceAttribute() : base(false, "AdministratorPrivilegesManagement", "CustomerService")
        {
        }
    }
}