using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.Supplier;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();

            return View(suppliers);
        }

        public async Task<IActionResult> Create()
        {
            var dto = new SupplierDto
            {
                SupplierCode = await _supplierService.GenerateSupplierCodeAsync(),
                IsActive = true
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (await _supplierService.CodeExistsAsync(dto.SupplierCode))
            {
                ModelState.AddModelError("SupplierCode", "Supplier Code already exists.");
                return View(dto);
            }

            if (await _supplierService.ExistsAsync(dto.SupplierName))
            {
                ModelState.AddModelError("SupplierName", "Supplier Name already exists.");
                return View(dto);
            }

            await _supplierService.CreateAsync(dto);

            TempData["Success"] = "Supplier created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (await _supplierService.CodeExistsAsync(dto.SupplierCode, dto.SupplierId))
            {
                ModelState.AddModelError("SupplierCode", "Supplier Code already exists.");
                return View(dto);
            }

            if (await _supplierService.ExistsAsync(dto.SupplierName, dto.SupplierId))
            {
                ModelState.AddModelError("SupplierName", "Supplier Name already exists.");
                return View(dto);
            }

            await _supplierService.UpdateAsync(dto);

            TempData["Success"] = "Supplier updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _supplierService.DeleteAsync(id);

            TempData["Success"] = "Supplier deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}