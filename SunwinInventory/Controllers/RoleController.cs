using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.Role;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        //==========================
        // Index
        //==========================
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllAsync();
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
                IsActive = true
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
                return View(dto);

            if (await _roleService.ExistsAsync(dto.RoleName))
            {
                ModelState.AddModelError("RoleName", "Role already exists.");
                return View(dto);
            }

            await _roleService.CreateAsync(dto);

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

            return View(dto);
        }

        //==========================
        // Edit POST
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (await _roleService.ExistsAsync(dto.RoleName, dto.RoleId))
            {
                ModelState.AddModelError("RoleName", "Role already exists.");
                return View(dto);
            }

            await _roleService.UpdateAsync(dto);

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
            await _roleService.DeleteAsync(id);

            TempData["Success"] = "Role deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}