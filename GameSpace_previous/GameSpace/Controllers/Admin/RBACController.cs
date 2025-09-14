using GameSpace.Attributes;
using GameSpace.Models;
using GameSpace.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSpace.Controllers.Admin
{
    /// <summary>
    /// RBAC權限管理控制器
    /// </summary>
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [SuperAdmin]
    public class RBACController : Controller
    {
        private readonly RBACService _rbacService;
        private readonly GameSpaceDbContext _context;
        private readonly ILogger<RBACController> _logger;

        public RBACController(RBACService rbacService, GameSpaceDbContext context, ILogger<RBACController> logger)
        {
            _rbacService = rbacService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 角色管理頁面
        /// </summary>
        [HttpGet("Roles")]
        public async Task<IActionResult> Roles()
        {
            try
            {
                var roles = await _context.ManagerRolePermissions
                    .OrderBy(r => r.RoleName)
                    .ToListAsync();

                return View(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入角色列表時發生錯誤");
                return View(new List<ManagerRolePermission>());
            }
        }

        /// <summary>
        /// 創建角色頁面
        /// </summary>
        [HttpGet("CreateRole")]
        public IActionResult CreateRole()
        {
            return View();
        }

        /// <summary>
        /// 創建角色
        /// </summary>
        [HttpPost("CreateRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var permissions = new Dictionary<string, bool>
                {
                    ["AdministratorPrivilegesManagement"] = model.AdministratorPrivilegesManagement,
                    ["UserStatusManagement"] = model.UserStatusManagement,
                    ["ShoppingPermissionManagement"] = model.ShoppingPermissionManagement,
                    ["MessagePermissionManagement"] = model.MessagePermissionManagement,
                    ["PetRightsManagement"] = model.PetRightsManagement,
                    ["CustomerService"] = model.CustomerService
                };

                var roleId = await _rbacService.CreateRoleAsync(model.RoleName, permissions);
                if (roleId.HasValue)
                {
                    TempData["SuccessMessage"] = $"角色「{model.RoleName}」創建成功";
                    return RedirectToAction(nameof(Roles));
                }
                else
                {
                    ModelState.AddModelError("", "創建角色失敗，請稍後再試");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建角色時發生錯誤");
                ModelState.AddModelError("", "創建角色時發生錯誤，請稍後再試");
            }

            return View(model);
        }

        /// <summary>
        /// 編輯角色頁面
        /// </summary>
        [HttpGet("EditRole/{id}")]
        public async Task<IActionResult> EditRole(int id)
        {
            try
            {
                var role = await _context.ManagerRolePermissions.FindAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                var model = new EditRoleViewModel
                {
                    ManagerRoleId = role.ManagerRoleId,
                    RoleName = role.RoleName,
                    AdministratorPrivilegesManagement = role.AdministratorPrivilegesManagement ?? false,
                    UserStatusManagement = role.UserStatusManagement ?? false,
                    ShoppingPermissionManagement = role.ShoppingPermissionManagement ?? false,
                    MessagePermissionManagement = role.MessagePermissionManagement ?? false,
                    PetRightsManagement = role.PetRightsManagement ?? false,
                    CustomerService = role.CustomerService ?? false
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入角色 {RoleId} 時發生錯誤", id);
                return NotFound();
            }
        }

        /// <summary>
        /// 編輯角色
        /// </summary>
        [HttpPost("EditRole/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(int id, EditRoleViewModel model)
        {
            if (id != model.ManagerRoleId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var role = await _context.ManagerRolePermissions.FindAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                role.RoleName = model.RoleName;
                role.AdministratorPrivilegesManagement = model.AdministratorPrivilegesManagement;
                role.UserStatusManagement = model.UserStatusManagement;
                role.ShoppingPermissionManagement = model.ShoppingPermissionManagement;
                role.MessagePermissionManagement = model.MessagePermissionManagement;
                role.PetRightsManagement = model.PetRightsManagement;
                role.CustomerService = model.CustomerService;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"角色「{model.RoleName}」更新成功";
                return RedirectToAction(nameof(Roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新角色 {RoleId} 時發生錯誤", id);
                ModelState.AddModelError("", "更新角色時發生錯誤，請稍後再試");
            }

            return View(model);
        }

        /// <summary>
        /// 管理員角色分配頁面
        /// </summary>
        [HttpGet("ManagerRoles")]
        public async Task<IActionResult> ManagerRoles()
        {
            try
            {
                var managers = await _context.ManagerData
                    .Include(m => m.ManagerRoles)
                        .ThenInclude(mr => mr.RolePermission)
                    .OrderBy(m => m.ManagerName)
                    .ToListAsync();

                return View(managers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入管理員角色分配時發生錯誤");
                return View(new List<ManagerData>());
            }
        }

        /// <summary>
        /// 分配角色給管理員
        /// </summary>
        [HttpPost("AssignRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int managerId, int roleId)
        {
            try
            {
                var success = await _rbacService.AssignRoleAsync(managerId, roleId);
                if (success)
                {
                    TempData["SuccessMessage"] = "角色分配成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "角色分配失敗，請稍後再試";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "分配角色給管理員 {ManagerId} 時發生錯誤", managerId);
                TempData["ErrorMessage"] = "角色分配時發生錯誤，請稍後再試";
            }

            return RedirectToAction(nameof(ManagerRoles));
        }

        /// <summary>
        /// 移除管理員角色
        /// </summary>
        [HttpPost("RemoveRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(int managerId, int roleId)
        {
            try
            {
                var success = await _rbacService.RemoveRoleAsync(managerId, roleId);
                if (success)
                {
                    TempData["SuccessMessage"] = "角色移除成功";
                }
                else
                {
                    TempData["ErrorMessage"] = "角色移除失敗，請稍後再試";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除管理員 {ManagerId} 角色時發生錯誤", managerId);
                TempData["ErrorMessage"] = "角色移除時發生錯誤，請稍後再試";
            }

            return RedirectToAction(nameof(ManagerRoles));
        }

        /// <summary>
        /// 檢查權限API
        /// </summary>
        [HttpGet("CheckPermission")]
        public async Task<IActionResult> CheckPermission(int managerId, string permission)
        {
            try
            {
                var hasPermission = await _rbacService.HasPermissionAsync(managerId, permission);
                return Json(new { hasPermission });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查管理員 {ManagerId} 權限 {Permission} 時發生錯誤", 
                    managerId, permission);
                return Json(new { hasPermission = false });
            }
        }

        /// <summary>
        /// 獲取管理員權限API
        /// </summary>
        [HttpGet("GetManagerPermissions")]
        public async Task<IActionResult> GetManagerPermissions(int managerId)
        {
            try
            {
                var permissions = await _rbacService.GetManagerPermissionsAsync(managerId);
                return Json(new { permissions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取管理員 {ManagerId} 權限時發生錯誤", managerId);
                return Json(new { permissions = new List<string>() });
            }
        }
    }

    /// <summary>
    /// 創建角色視圖模型
    /// </summary>
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "角色名稱不能為空")]
        [StringLength(50, ErrorMessage = "角色名稱不能超過50個字符")]
        [Display(Name = "角色名稱")]
        public string RoleName { get; set; } = null!;

        [Display(Name = "管理員權限管理")]
        public bool AdministratorPrivilegesManagement { get; set; }

        [Display(Name = "用戶狀態管理")]
        public bool UserStatusManagement { get; set; }

        [Display(Name = "購物權限管理")]
        public bool ShoppingPermissionManagement { get; set; }

        [Display(Name = "訊息權限管理")]
        public bool MessagePermissionManagement { get; set; }

        [Display(Name = "寵物權限管理")]
        public bool PetRightsManagement { get; set; }

        [Display(Name = "客服權限")]
        public bool CustomerService { get; set; }
    }

    /// <summary>
    /// 編輯角色視圖模型
    /// </summary>
    public class EditRoleViewModel : CreateRoleViewModel
    {
        public int ManagerRoleId { get; set; }
    }
}