using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.Designation;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class DesignationController : Controller
    {
        private readonly IDesignationService _designationService;

        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        //=========================
        // Index
        //=========================
        public async Task<IActionResult> Index()
        {
            var model = await _designationService.GetAllAsync();
            return View(model);
        }

        //=========================
        // Create GET
        //=========================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadReportTo();

            return View(new DesignationDto
            {
                IsActive = true
            });
        }

        //=========================
        // Create POST
        //=========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DesignationDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadReportTo();
                return View(dto);
            }

            if (await _designationService.ExistsAsync(dto.DesignationName))
            {
                ModelState.AddModelError("DesignationName", "Designation already exists.");
                await LoadReportTo();
                return View(dto);
            }

            await _designationService.CreateAsync(dto);

            TempData["Success"] = "Designation created successfully.";

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // Edit GET
        //=========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _designationService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await LoadReportTo();

            return View(dto);
        }

        //=========================
        // Edit POST
        //=========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DesignationDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadReportTo();
                return View(dto);
            }

            if (await _designationService.ExistsAsync(dto.DesignationName, dto.DesignationId))
            {
                ModelState.AddModelError("DesignationName", "Designation already exists.");
                await LoadReportTo();
                return View(dto);
            }

            await _designationService.UpdateAsync(dto);

            TempData["Success"] = "Designation updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // Delete
        //=========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _designationService.DeleteAsync(id);

            TempData["Success"] = "Designation deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        //=========================
        // Load ReportTo Dropdown
        //=========================
        private async Task LoadReportTo()
        {
            var designations = await _designationService.GetAllAsync();

            ViewBag.ReportTo = new SelectList(
                designations,
                "DesignationId",
                "DesignationName");
        }
    }
}