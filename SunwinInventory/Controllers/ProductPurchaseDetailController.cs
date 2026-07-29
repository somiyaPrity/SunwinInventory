using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;
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
            _lookupService = lookupService;
        }

        //=====================================================
        // INDEX
        //=====================================================

        public async Task<IActionResult> Index()
        {
            var list = await _purchaseService.GetAllAsync();
            return View(list);
        }

        //=====================================================
        // CREATE (GET)
        //=====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropDowns();

            return View(new PurchaseEntryDto());
        }

        //=====================================================
        // CREATE (POST)
        //=====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseEntryDto model)
        {
            if (model.Items == null || model.Items.Count == 0)
            {
                TempData["Error"] = "Please add at least one item.";

                await LoadDropDowns();

                return View(model);
            }

            foreach (var item in model.Items)
            {
                var entity = new ProductPurchaseDetail
                {
                    ShipmentId = item.ShipmentId,
                    SupplierId = item.SupplierId,
                    WarehouseId = item.WarehouseId,

                    ProductNameId = item.ProductNameId,
                    ColorId = item.ColorId,

                    Thickness = item.Thickness,
                    ProductSize = item.ProductSize.ToString(),

                    InitialQuantity = item.InitialQuantity,

                    UnitWeightKg = item.UnitWeightKg,
                    TotalWeightKg = item.TotalWeightKg,

                    ExchangeRate = item.ExchangeRate,

                    UnitPriceTk = item.UnitPriceTk,
                    UnitPriceRmb = item.UnitPriceRmb,

                    ImportTaxPerKgTk = item.ImportTaxPerKgTk,
                    ImportTaxPerKgRmb = item.ImportTaxPerKgRmb,

                    CostPriceKgTk = item.CostPriceKgTk,
                    CostPriceLbsTk = item.CostPriceLbsTk,

                    CostPricePerPieceTk = item.CostPricePerPieceTk,

                    CostPriceKgRmb = item.CostPriceKgRmb,
                    CostPriceLbsRmb = item.CostPriceLbsRmb,

                    TotalAmountWithoutTaxTk = item.TotalAmountWithoutTaxTk,
                    TotalAmountIncludingTaxTk = item.TotalAmountIncludingTaxTk,

                    TotalAmountWithoutTaxRmb = item.TotalAmountWithoutTaxRmb,
                    TotalAmountIncludingTaxRmb = item.TotalAmountIncludingTaxRmb,

                    Remarks = item.Remarks
                };

                await _purchaseService.CreateAsync(entity);
            }

            TempData["Success"] = "Purchase saved successfully.";

            return RedirectToAction(nameof(Create));
        }        //=====================================================
        // UPLOAD EXCEL
        //=====================================================

        [HttpPost]
        public async Task<IActionResult> UploadExcel(
            IFormFile file,
            int shipmentId,
            int supplierId,
            int warehouseId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select an Excel file.");

            var items = new List<ProductPurchaseDetailDto>();

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var ws = workbook.Worksheet(1);

            int lastRow = ws.LastRowUsed().RowNumber();

            for (int row = 3; row <= lastRow; row++)
            {
                if (string.IsNullOrWhiteSpace(ws.Cell(row, 3).GetString()))
                    continue;

                var dto = new ProductPurchaseDetailDto();

                dto.ShipmentId = shipmentId;
                dto.SupplierId = supplierId;
                dto.WarehouseId = warehouseId;

                dto.ProductName = ws.Cell(row, 3).GetString().Trim();
                dto.ColorName = ws.Cell(row, 4).GetString().Trim();

                dto.Thickness = GetDecimal(ws, row, 5);
                dto.ProductSize = ws.Cell(row, 6).GetString();

                dto.InitialQuantity = (int)GetDecimal(ws, row, 7);

                dto.UnitWeightKg = GetDecimal(ws, row, 8);
                dto.TotalWeightKg = GetDecimal(ws, row, 9);

                dto.UnitPriceTk = GetDecimal(ws, row, 10);
                dto.ExchangeRate = GetDecimal(ws, row, 11);
                dto.UnitPriceRmb = GetDecimal(ws, row, 12);

                dto.ImportTaxPerKgTk = GetDecimal(ws, row, 13);
                dto.ImportTaxPerKgRmb = GetDecimal(ws, row, 14);

                dto.CostPriceKgTk = GetDecimal(ws, row, 15);
                dto.CostPriceLbsTk = GetDecimal(ws, row, 16);
                dto.CostPricePerPieceTk = GetDecimal(ws, row, 17);

                dto.CostPriceKgRmb = GetDecimal(ws, row, 18);
                dto.CostPriceLbsRmb = GetDecimal(ws, row, 19);
                dto.CostPricePcsRmb = GetDecimal(ws, row, 20);

                dto.TotalAmountWithoutTaxTk = GetDecimal(ws, row, 21);
                dto.TotalAmountIncludingTaxTk = GetDecimal(ws, row, 22);

                dto.TotalAmountWithoutTaxRmb = GetDecimal(ws, row, 23);
                dto.TotalAmountIncludingTaxRmb = GetDecimal(ws, row, 24);

                dto.Remarks = ws.Cell(row, 25).GetString();

                dto.ProductNameId =
                    await _lookupService.GetProductIdAsync(dto.ProductName) ?? 0;

                dto.ColorId =
                    await _lookupService.GetColorIdAsync(dto.ColorName) ?? 0;

                items.Add(dto);
            }

            return Json(items);
        }

        //=====================================================
        // SAVE EXCEL PURCHASE
        //=====================================================

        [HttpPost]
        public async Task<IActionResult> SaveExcelPurchase(PurchaseEntryDto model)
        {
            if (model.Items == null || model.Items.Count == 0)
            {
                TempData["Error"] = "No Excel data found.";

                return RedirectToAction(nameof(Create));
            }

            foreach (var item in model.Items)
            {
                var entity = new ProductPurchaseDetail
                {
                    ShipmentId = item.ShipmentId,
                    SupplierId = item.SupplierId,
                    WarehouseId = item.WarehouseId,

                    ProductNameId = item.ProductNameId,
                    ColorId = item.ColorId,

                    Thickness = item.Thickness,
                    ProductSize = item.ProductSize.ToString(),

                    InitialQuantity = item.InitialQuantity,

                    UnitWeightKg = item.UnitWeightKg,
                    TotalWeightKg = item.TotalWeightKg,

                    ExchangeRate = item.ExchangeRate,

                    UnitPriceTk = item.UnitPriceTk,
                    UnitPriceRmb = item.UnitPriceRmb,

                    ImportTaxPerKgTk = item.ImportTaxPerKgTk,
                    ImportTaxPerKgRmb = item.ImportTaxPerKgRmb,

                    CostPriceKgTk = item.CostPriceKgTk,
                    CostPriceLbsTk = item.CostPriceLbsTk,
                    CostPricePerPieceTk = item.CostPricePerPieceTk,

                    CostPriceKgRmb = item.CostPriceKgRmb,
                    CostPriceLbsRmb = item.CostPriceLbsRmb,

                    TotalAmountWithoutTaxTk = item.TotalAmountWithoutTaxTk,
                    TotalAmountIncludingTaxTk = item.TotalAmountIncludingTaxTk,

                    TotalAmountWithoutTaxRmb = item.TotalAmountWithoutTaxRmb,
                    TotalAmountIncludingTaxRmb = item.TotalAmountIncludingTaxRmb,

                    Remarks = item.Remarks
                };

                await _purchaseService.CreateAsync(entity);
            }

            TempData["Success"] = "Excel Purchase Saved Successfully.";

            return RedirectToAction(nameof(Create));
        }

        //=====================================================
        // HELPER
        //=====================================================

        private decimal GetDecimal(IXLWorksheet ws, int row, int column)
        {
            var value = ws.Cell(row, column)
                          .GetString()
                          .Replace(",", "")
                          .Trim();

            if (decimal.TryParse(value, out decimal result))
                return result;

            return 0;
        }        //=====================================================
                 // LOAD DROPDOWNS
                 //=====================================================

        private async Task LoadDropDowns()
        {
            var products = await _productNameService.GetAllAsync();

            if (products == null)
                throw new Exception("Products returned NULL.");

            ViewBag.Products = new SelectList(
                products,
                "ProductNameId",
                "Name");

            ViewBag.Colors = await _lookupService.GetColorListAsync();

            ViewBag.Suppliers = new SelectList(
                await _supplierService.GetAllAsync(),
                "SupplierId",
                "SupplierName");

            ViewBag.Warehouses = new SelectList(
                await _warehouseService.GetAllAsync(),
                "WarehouseId",
                "WarehouseName");

            ViewBag.Shipments = new SelectList(
                await _shipmentService.GetAllAsync(),
                "ShipmentId",
                "ShipmentNo");
        }

        //=====================================================
        // EDIT
        //=====================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _purchaseService.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            await LoadDropDowns();

            return View(item);
        }

        //=====================================================
        // DELETE
        //=====================================================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _purchaseService.DeleteAsync(id);

            TempData["Success"] = "Purchase deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}