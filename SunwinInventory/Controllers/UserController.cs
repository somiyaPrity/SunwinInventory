using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.User;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IDesignationService _designationService;
        private readonly IWarehouseService _warehouseService;

        public UserController(
            IUserService userService,
            IRoleService roleService,
            IDesignationService designationService,
            IWarehouseService warehouseService)
        {
            _userService = userService;
            _roleService = roleService;
            _designationService = designationService;
            _warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userService.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            return View(new UserDto
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(dto);
            }

            if (await _userService.ExistsAsync(dto.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists.");
                await LoadDropdowns();
                return View(dto);
            }

            await _userService.CreateAsync(dto);

            TempData["Success"] = "User created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _userService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await LoadDropdowns();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(dto);
            }

            await _userService.UpdateAsync(dto);

            TempData["Success"] = "User updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            ViewBag.Roles = new SelectList(await _roleService.GetAllAsync(), "RoleId", "RoleName");
            ViewBag.Designations = new SelectList(await _designationService.GetAllAsync(), "DesignationId", "DesignationName");
            ViewBag.Warehouses = new SelectList(await _warehouseService.GetAllAsync(), "WarehouseId", "WarehouseName");
        }
    }
}