using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class ProductPurchaseDetailController : Controller
    {
        private readonly IProductPurchaseDetailService _purchaseService;
        private readonly IShipmentService _shipmentService;
        private readonly ISupplierService _supplierService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductNameService _productNameService;
        private readonly ILookupService _lookupService;

        public ProductPurchaseDetailController(
            IProductPurchaseDetailService purchaseService,
            IShipmentService shipmentService,
            ISupplierService supplierService,
            IWarehouseService warehouseService,
            IProductNameService productNameService,
            ILookupService lookupService)
        {
            _purchaseService = purchaseService;
            _shipmentService = shipmentService;
            _supplierService = supplierService;
            _warehouseService = warehouseService;
            _productNameService = productNameService;
            _lookupService = lookupService; ;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropDowns();

            return View(new PurchaseEntryDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseEntryDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDowns();
                return View(dto);
            }

            if (dto.Items == null || dto.Items.Count == 0)
            {
                TempData["Error"] = "Please add at least one product.";

                await LoadDropDowns();

                return View(dto);
            }

            foreach (var item in dto.Items)
            {
                item.ShipmentId = dto.ShipmentId;

                item.SupplierId = dto.SupplierId;

                item.WarehouseId = dto.WarehouseId;
            }

            await _purchaseService.CreateAsync(dto);

            TempData["Success"] = "Purchase saved successfully.";

            return RedirectToAction(nameof(Create));
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(PurchaseEntryDto dto)
        //{
        //    if (dto.Items == null || !dto.Items.Any())
        //    {
        //        TempData["Error"] = "Please add at least one product.";

        //        await LoadDropDowns();

        //        return View(dto);
        //    }

        //    await _purchaseService.CreateAsync(dto);

        //    TempData["Success"] = "Purchase saved successfully.";

        //    return RedirectToAction(nameof(Create));
        //}

        private async Task LoadDropDowns()
        {
            ViewBag.Shipments = new SelectList(
                await _shipmentService.GetAllAsync(),
                "ShipmentId",
                "ShipmentNo");

            ViewBag.Suppliers = new SelectList(
                await _supplierService.GetAllAsync(),
                "SupplierId",
                "SupplierName");

            ViewBag.Warehouses = new SelectList(
                await _warehouseService.GetAllAsync(),
                "WarehouseId",
                "WarehouseName");

            var products = await _productNameService.GetAllAsync();

            if (products == null)
            {
                throw new Exception("Products is NULL");
            }

            ViewBag.Products = new SelectList(
                products,
                "ProductNameId",
                "Name");

            ViewBag.Colors = await _lookupService.GetColorListAsync();
        }
    }
}