using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.Shipment;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class ShipmentController : Controller
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        public async Task<IActionResult> Index()
        {
            var shipments = await _shipmentService.GetAllAsync();

            return View(shipments);
        }

        public async Task<IActionResult> Create()
        {
            var dto = new ShipmentDto
            {
                ShipmentNo = await _shipmentService.GenerateShipmentNoAsync(),
                ShipmentDate = DateTime.Today,
                IsActive = true
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShipmentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _shipmentService.CreateAsync(dto);

            TempData["Success"] = "Shipment created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);

            if (shipment == null)
                return NotFound();

            return View(shipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShipmentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _shipmentService.UpdateAsync(dto);

            TempData["Success"] = "Shipment updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _shipmentService.DeleteAsync(id);

            TempData["Success"] = "Shipment deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}