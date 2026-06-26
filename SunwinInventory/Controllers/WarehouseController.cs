using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.Warehouse;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // GET: Warehouse
        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseService.GetAllAsync();
            return View(warehouses);
        }

        // GET: Warehouse/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWarehouseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _warehouseService.CreateAsync(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Warehouse name already exists.");
                return View(dto);
            }

            TempData["Success"] = "Warehouse created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Warehouse/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseService.GetByIdAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouse/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WarehouseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _warehouseService.UpdateAsync(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Unable to update warehouse.");
                return View(dto);
            }

            TempData["Success"] = "Warehouse updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Warehouse/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _warehouseService.DeleteAsync(id);

            if (!result)
            {
                TempData["Error"] = "Unable to delete warehouse.";
            }
            else
            {
                TempData["Success"] = "Warehouse deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}