using Microsoft.AspNetCore.Mvc;
using SunwinInventory.Authorization;
using SunwinInventory.DTOs.Role;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public RoleController(
            IRoleService roleService,
            IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        //==========================
        // Index
        //==========================
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllAsync();

            ViewBag.AdministratorRoleId = roles
                .FirstOrDefault(r => string.Equals(
                    r.RoleName,
                    MenuCatalog.AdministratorRole,
                    StringComparison.OrdinalIgnoreCase))
                ?.RoleId;

            return View(roles);
        }

        //==========================
        // Create GET
        //==========================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RoleDto
            {
                IsActive = true,
                Menus = MenuSelection.Parse(null)
            });
        }

        //==========================
        // Create POST
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Menus = MenuSelection.Parse(dto.Permissions);
                return View(dto);
            }

            if (await _roleService.ExistsAsync(dto.RoleName))
            {
                ModelState.AddModelError("RoleName", "Role already exists.");
                dto.Menus = MenuSelection.Parse(dto.Permissions);
                return View(dto);
            }

            int roleId = await _roleService.CreateAsync(dto);

            await _permissionService.SaveRoleMenusAsync(
                roleId,
                MenuSelection.Parse(dto.Permissions));

            TempData["Success"] = "Role created successfully.";

            return RedirectToAction(nameof(Index));
        }

        //==========================
        // Edit GET
        //==========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _roleService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            dto.Menus = await _permissionService.GetRoleMenusAsync(id);

            ViewBag.IsAdministrator =
                await _permissionService.IsAdministratorAsync(id);

            return View(dto);
        }

        //==========================
        // Edit POST
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDto dto)
        {
            bool isAdministrator =
                await _permissionService.IsAdministratorAsync(dto.RoleId);

            ViewBag.IsAdministrator = isAdministrator;

            if (!ModelState.IsValid)
            {
                dto.Menus = MenuSelection.Parse(dto.Permissions);
                return View(dto);
            }

            if (isAdministrator)
            {
                var existing = await _roleService.GetByIdAsync(dto.RoleId);

                if (existing != null &&
                    !string.Equals(existing.RoleName, dto.RoleName, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        nameof(dto.RoleName),
                        "The Administrator role name cannot be changed.");

                    dto.Menus = MenuSelection.Parse(dto.Permissions);
                    return View(dto);
                }

                dto.RoleName = existing?.RoleName ?? dto.RoleName;
            }

            if (await _roleService.ExistsAsync(dto.RoleName, dto.RoleId))
            {
                ModelState.AddModelError("RoleName", "Role already exists.");
                dto.Menus = MenuSelection.Parse(dto.Permissions);
                return View(dto);
            }

            await _roleService.UpdateAsync(dto);

            if (!isAdministrator)
            {
                await _permissionService.SaveRoleMenusAsync(
                    dto.RoleId,
                    MenuSelection.Parse(dto.Permissions));
            }

            TempData["Success"] = "Role updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        //==========================
        // Delete
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _permissionService.IsAdministratorAsync(id))
            {
                TempData["Error"] = "The Administrator role cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            await _roleService.DeleteAsync(id);

            TempData["Success"] = "Role deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

    }
}
