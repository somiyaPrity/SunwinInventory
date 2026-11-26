using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.Authorization;
using SunwinInventory.DTOs.MenuAccess;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class MenuAccessController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public MenuAccessController(
            IRoleService roleService,
            IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? roleId)
        {
            var roles = await _roleService.GetAllAsync();

            if (roles.Count == 0)
                return View(new MenuAccessViewModel());

            var selected = roleId.HasValue
                ? roles.FirstOrDefault(r => r.RoleId == roleId.Value)
                : roles.FirstOrDefault(r => !string.Equals(
                      r.RoleName,
                      MenuCatalog.AdministratorRole,
                      StringComparison.OrdinalIgnoreCase));

            selected ??= roles[0];

            var model = new MenuAccessViewModel
            {
                RoleId = selected.RoleId,
                RoleName = selected.RoleName,
                IsAdministrator =
                    await _permissionService.IsAdministratorAsync(selected.RoleId),
                Menus = await _permissionService.GetRoleMenusAsync(selected.RoleId)
            };

            model.Roles = roles
                .Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.RoleName
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(
            int roleId,
            List<string>? permissions)
        {
            if (roleId <= 0)
            {
                TempData["Error"] = "Please select a role.";

                return RedirectToAction(nameof(Index));
            }

            await _permissionService.SaveRoleMenusAsync(
                roleId,
                MenuSelection.Parse(permissions));

            TempData["Success"] = "Menu access updated successfully.";

            return RedirectToAction(nameof(Index), new { roleId });
        }
    }
}
