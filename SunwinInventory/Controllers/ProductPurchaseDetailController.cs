using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;
using SunwinInventory.Services;
using SunwinInventory.Services.Interfaces;
using System;

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
        private readonly IStockLedgerService _stockLedgerService;
        public ProductPurchaseDetailController(
            IProductPurchaseDetailService purchaseService,
            IShipmentService shipmentService,
            ISupplierService supplierService,
            IWarehouseService warehouseService,
            IProductNameService productNameService,
            ILookupService lookupService, IStockLedgerService stockLedgerService)
        {
            _purchaseService = purchaseService;
            _shipmentService = shipmentService;
            _supplierService = supplierService;
            _warehouseService = warehouseService;
            _productNameService = productNameService;
            _lookupService = lookupService;
            _stockLedgerService = stockLedgerService;
        }

        //=====================================================
        // INDEX
        //=====================================================

        public async Task<IActionResult> Index(PurchaseReportFilterDto filter)
        {
            await LoadDropDowns();

            var list = await _purchaseService.GetPurchaseReportAsync(filter);

            ViewBag.Filter = filter;

            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var list = await _purchaseService.GetPurchaseReportAsync();

            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Purchase Report");

            int row = 1;

            //==============================
            // Header
            //==============================

            ws.Cell(row, 1).Value = "Date";
            ws.Cell(row, 2).Value = "Shipment";
            ws.Cell(row, 3).Value = "Supplier";
            ws.Cell(row, 4).Value = "Warehouse";
            ws.Cell(row, 5).Value = "Product";
            ws.Cell(row, 6).Value = "Color";
            ws.Cell(row, 7).Value = "Thickness";
            ws.Cell(row, 8).Value = "Size";
            ws.Cell(row, 9).Value = "Quantity";
            ws.Cell(row, 10).Value = "Unit Weight";
            ws.Cell(row, 11).Value = "Total Weight";
            ws.Cell(row, 12).Value = "Exchange Rate";
            ws.Cell(row, 13).Value = "Unit Price TK";
            ws.Cell(row, 14).Value = "Unit Price RMB";
            ws.Cell(row, 15).Value = "Import Tax TK";
            ws.Cell(row, 16).Value = "Import Tax RMB";
            ws.Cell(row, 17).Value = "Cost/KG TK";
            ws.Cell(row, 18).Value = "Cost/LBS TK";
            ws.Cell(row, 19).Value = "Cost/PCS TK";
            ws.Cell(row, 20).Value = "Cost/KG RMB";
            ws.Cell(row, 21).Value = "Cost/LBS RMB";
            ws.Cell(row, 22).Value = "Cost/PCS RMB";
            ws.Cell(row, 23).Value = "Total Without Tax TK";
            ws.Cell(row, 24).Value = "Total With Tax TK";
            ws.Cell(row, 25).Value = "Total Without Tax RMB";
            ws.Cell(row, 26).Value = "Total With Tax RMB";
            ws.Cell(row, 27).Value = "Remarks";

            //============================================
            // STEP 4.2 (ADD HERE)
            //============================================

            ws.Range("A1:AA1").Style.Font.Bold = true;
            ws.Range("A1:AA1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Range("A1:AA1").Style.Font.FontColor = XLColor.White;
            ws.Range("A1:AA1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //============================================
            // STEP 4.4 (ADD HERE)
            //============================================

            ws.SheetView.FreezeRows(1);

            row++;

            foreach (var item in list)
            {
                ws.Cell(row, 1).Value = item.CreatedDate;
                ws.Cell(row, 2).Value = item.ShipmentNo;
                ws.Cell(row, 3).Value = item.SupplierName;
                ws.Cell(row, 4).Value = item.WarehouseName;
                ws.Cell(row, 5).Value = item.ProductName;
                ws.Cell(row, 6).Value = item.ColorName;
                ws.Cell(row, 7).Value = item.Thickness;
                ws.Cell(row, 8).Value = item.ProductSize;
                ws.Cell(row, 9).Value = item.InitialQuantity;
                ws.Cell(row, 10).Value = item.UnitWeightKg;
                ws.Cell(row, 11).Value = item.TotalWeightKg;
                ws.Cell(row, 12).Value = item.ExchangeRate;
                ws.Cell(row, 13).Value = item.UnitPriceTk;
                ws.Cell(row, 14).Value = item.UnitPriceRmb;
                ws.Cell(row, 15).Value = item.ImportTaxPerKgTk;
                ws.Cell(row, 16).Value = item.ImportTaxPerKgRmb;
                ws.Cell(row, 17).Value = item.CostPriceKgTk;
                ws.Cell(row, 18).Value = item.CostPriceLbsTk;
                ws.Cell(row, 19).Value = item.CostPricePerPieceTk;
                ws.Cell(row, 20).Value = item.CostPriceKgRmb;
                ws.Cell(row, 21).Value = item.CostPriceLbsRmb;
                ws.Cell(row, 22).Value = item.CostPricePcsRmb;
                ws.Cell(row, 23).Value = item.TotalAmountWithoutTaxTk;
                ws.Cell(row, 24).Value = item.TotalAmountIncludingTaxTk;
                ws.Cell(row, 25).Value = item.TotalAmountWithoutTaxRmb;
                ws.Cell(row, 26).Value = item.TotalAmountIncludingTaxRmb;
                ws.Cell(row, 27).Value = item.Remarks;

                row++;
            }

            //============================================
            // STEP 4.3 (ADD HERE)
            //============================================

            var range = ws.Range(1, 1, row - 1, 27);

            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"PurchaseReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
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
                await _stockLedgerService.CreateAsync(new StockLedger
                {
                    PurchaseDetailId = entity.PurchaseDetailId,

                    WarehouseId = entity.WarehouseId,

                    ProductNameId = entity.ProductNameId,

                    ColorId = entity.ColorId,

                    AvailableQty = entity.InitialQuantity,

                    AvailableWeight = entity.TotalWeightKg ?? 0,
                    UnitCost = entity.CostPriceKgTk ?? 0,

                    PurchaseDate = entity.CreatedDate
                });
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
                return BadRequest("No file selected.");

            var items = new List<ProductPurchaseDetailDto>();

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var ws = workbook.Worksheet(1);

            int lastRow = ws.LastRowUsed().RowNumber();

            for (int row = 3; row <= lastRow; row++)
            {
                // Skip blank product rows
                if (string.IsNullOrWhiteSpace(ws.Cell(row, 3).GetString()))
                    continue;

                var dto = new ProductPurchaseDetailDto();

                // =========================================
                // HEADER VALUES
                // =========================================

                dto.ShipmentId = shipmentId;
                dto.SupplierId = supplierId;
                dto.WarehouseId = warehouseId;


                // =========================================
                // BASIC PRODUCT INFORMATION
                // =========================================

                // C - Product Name
                dto.ProductName =
                    ws.Cell(row, 3)
                      .GetString()
                      .Trim();

                // D - Color
                dto.ColorName =
                    ws.Cell(row, 4)
                      .GetString()
                      .Trim();

                // E - Thickness
                dto.Thickness =
                    GetDecimal(ws, row, 5);

                // F - Product Size
                dto.ProductSize =
                    ws.Cell(row, 6)
                      .GetString()
                      .Trim();


                // =========================================
                // QUANTITY / WEIGHT
                // =========================================

                // G - Initial Quantity
                dto.InitialQuantity =
                    (int)GetDecimal(ws, row, 7);

                // H - Weight KG / PCS
                dto.UnitWeightKg =
                    GetDecimal(ws, row, 8);

                // I - Total Weight KG
                dto.TotalWeightKg =
                    GetDecimal(ws, row, 9);


                // =========================================
                // PRICE INFORMATION
                // =========================================

                // J - Unit Price TK
                dto.UnitPriceTk =
                    GetDecimal(ws, row, 10);

                // K - Unit Price RMB
                dto.UnitPriceRmb =
                    GetDecimal(ws, row, 11);


                // =========================================
                // EXCHANGE RATE
                // =========================================
                // Excel does not have a separate Exchange Rate
                // column.
                //
                // J = K * ExchangeRate
                //
                // Therefore:
                // ExchangeRate = UnitPriceTk / UnitPriceRmb

                dto.ExchangeRate =
                    dto.UnitPriceRmb != 0
                        ? dto.UnitPriceTk / dto.UnitPriceRmb
                        : 0;


                // =========================================
                // TAX / COST
                // =========================================

                // L - Tax / KG
                dto.ImportTaxPerKgTk =
                    GetDecimal(ws, row, 12);

                // M - Price / KG including TAX
                dto.CostPriceKgTk =
                    GetDecimal(ws, row, 13);

                // N - Price / LBS including TAX
                dto.CostPriceLbsTk =
                    GetDecimal(ws, row, 14);


                // =========================================
                // COST PER PIECE
                // =========================================
                // Excel Y contains Cost Price per PCS (TK)
                //
                // Y = column 25

                dto.CostPricePerPieceTk =
                    GetDecimal(ws, row, 25);


                // =========================================
                // RMB COST VALUES
                // =========================================

                // Cost KG RMB
                dto.CostPriceKgRmb =
                    dto.ExchangeRate != 0
                        ? dto.CostPriceKgTk / dto.ExchangeRate
                        : 0;

                // Cost LBS RMB
                dto.CostPriceLbsRmb =
                    dto.ExchangeRate != 0
                        ? dto.CostPriceLbsTk / dto.ExchangeRate
                        : 0;

                // Cost PCS RMB
                dto.CostPricePcsRmb =
                    dto.ExchangeRate != 0
                        ? dto.CostPricePerPieceTk / dto.ExchangeRate
                        : 0;


                // =========================================
                // TOTAL AMOUNTS
                // =========================================

                // O - Total Amount Without TAX (TK)
                dto.TotalAmountWithoutTaxTk =
                    GetDecimal(ws, row, 15);

                // P - Total Amount (RMB)
                dto.TotalAmountWithoutTaxRmb =
                    GetDecimal(ws, row, 16);

                // Q - TOTAL INCLUDING TAX (TK)
                // THIS IS THE IMPORTANT ONE
                dto.TotalAmountIncludingTaxTk =
                    GetDecimal(ws, row, 17);

                // R - Total Including TAX (RMB)
                dto.TotalAmountIncludingTaxRmb =
                    GetDecimal(ws, row, 18);


                // =========================================
                // LOOKUP PRODUCT / COLOR IDS
                // =========================================

                dto.ProductNameId =
                    await _lookupService
                        .GetProductIdAsync(dto.ProductName)
                    ?? 0;

                dto.ColorId =
                    await _lookupService
                        .GetColorIdAsync(dto.ColorName)
                    ?? 0;


                // =========================================
                // DEBUG
                // =========================================

                Console.WriteLine(
                    $"Row: {row} | " +
                    $"Product: {dto.ProductName} | " +
                    $"Weight: {dto.TotalWeightKg} | " +
                    $"UnitPriceTK: {dto.UnitPriceTk} | " +
                    $"TaxTK: {dto.ImportTaxPerKgTk} | " +
                    $"TotalWithoutTax: {dto.TotalAmountWithoutTaxTk} | " +
                    $"TotalWithTax(Q): {dto.TotalAmountIncludingTaxTk}"
                );


                items.Add(dto);
            }

            return Json(items);
        }


        private decimal GetDecimal(
            IXLWorksheet ws,
            int row,
            int column)
        {
            var cell = ws.Cell(row, column);

            // Get calculated value from Excel
            if (cell.DataType == XLDataType.Number)
            {
                return cell.GetValue<decimal>();
            }

            var value =
                cell.GetString()
                    .Replace(",", "")
                    .Trim();

            if (decimal.TryParse(
                value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }

            return 0;
        }
        //[HttpPost]
        //public async Task<IActionResult> UploadExcel(
        //    IFormFile file,
        //    int shipmentId,
        //    int supplierId,
        //    int warehouseId)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("Please select an Excel file.");

        //    var items = new List<ProductPurchaseDetailDto>();

        //    using var stream = file.OpenReadStream();
        //    using var workbook = new XLWorkbook(stream);

        //    var ws = workbook.Worksheet(1);

        //    int lastRow = ws.LastRowUsed().RowNumber();

        //    for (int row = 3; row <= lastRow; row++)
        //    {
        //        if (string.IsNullOrWhiteSpace(ws.Cell(row, 3).GetString()))
        //            continue;

        //        var dto = new ProductPurchaseDetailDto();

        //        dto.ShipmentId = shipmentId;
        //        dto.SupplierId = supplierId;
        //        dto.WarehouseId = warehouseId;

        //        dto.ProductName = ws.Cell(row, 3).GetString().Trim();
        //        dto.ColorName = ws.Cell(row, 4).GetString().Trim();

        //        dto.Thickness = GetDecimal(ws, row, 5);
        //        dto.ProductSize = ws.Cell(row, 6).GetString();

        //        dto.InitialQuantity = (int)GetDecimal(ws, row, 7);

        //        dto.UnitWeightKg = GetDecimal(ws, row, 8);
        //        dto.TotalWeightKg = GetDecimal(ws, row, 9);

        //        dto.UnitPriceTk = GetDecimal(ws, row, 10);
        //        dto.ExchangeRate = GetDecimal(ws, row, 11);
        //        dto.UnitPriceRmb = GetDecimal(ws, row, 12);

        //        dto.ImportTaxPerKgTk = GetDecimal(ws, row, 13);
        //        dto.ImportTaxPerKgRmb = GetDecimal(ws, row, 14);

        //        dto.CostPriceKgTk = GetDecimal(ws, row, 15);
        //        dto.CostPriceLbsTk = GetDecimal(ws, row, 16);
        //        dto.CostPricePerPieceTk = GetDecimal(ws, row, 17);

        //        dto.CostPriceKgRmb = GetDecimal(ws, row, 18);
        //        dto.CostPriceLbsRmb = GetDecimal(ws, row, 19);
        //        dto.CostPricePcsRmb = GetDecimal(ws, row, 20);

        //        dto.TotalAmountWithoutTaxTk = GetDecimal(ws, row, 21);
        //        dto.TotalAmountIncludingTaxTk = GetDecimal(ws, row, 22);

        //        dto.TotalAmountWithoutTaxRmb = GetDecimal(ws, row, 23);
        //        dto.TotalAmountIncludingTaxRmb = GetDecimal(ws, row, 24);

        //        dto.Remarks = ws.Cell(row, 25).GetString();

        //        dto.ProductNameId =
        //            await _lookupService.GetProductIdAsync(dto.ProductName) ?? 0;

        //        dto.ColorId =
        //            await _lookupService.GetColorIdAsync(dto.ColorName) ?? 0;

        //        items.Add(dto);
        //    }

        //    return Json(items);
        //}

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

        //private decimal GetDecimal(IXLWorksheet ws, int row, int column)
        //{
        //    var value = ws.Cell(row, column)
        //                  .GetString()
        //                  .Replace(",", "")
        //                  .Trim();

        //    if (decimal.TryParse(value, out decimal result))
        //        return result;

        //    return 0;
        //}       
        //=====================================================
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
            ViewBag.Shipments = new SelectList(await _shipmentService.GetAllAsync(), "ShipmentId", "ShipmentNo");

            ViewBag.Products = new SelectList(await _productNameService.GetAllAsync(), "ProductNameId", "Name");
        }

        //=====================================================
        // EDIT
        //=====================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(ProductPurchaseDetail model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await LoadDropDowns();
        //        return View(model);
        //    }

        //    await _purchaseService.UpdateAsync(model);

        //    TempData["Success"] = "Purchase Updated Successfully.";

        //    return RedirectToAction(nameof(Index));
        //}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _purchaseService.GetByIdAsync(id);

            if (model == null)
                return NotFound();

            await LoadDropDowns();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductPurchaseDetailDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDowns();
                return View(model);
            }

            // Load existing record first
            var entity = await _purchaseService.GetEntityByIdAsync(model.PurchaseDetailId);

            if (entity == null)
                return NotFound();

            // Update editable fields only
            entity.ShipmentId = model.ShipmentId;
            entity.SupplierId = model.SupplierId;
            entity.WarehouseId = model.WarehouseId;

            entity.ProductNameId = model.ProductNameId;
            entity.ColorId = model.ColorId;

            entity.Thickness = model.Thickness;
            entity.ProductSize = model.ProductSize;

            entity.InitialQuantity = model.InitialQuantity;

            entity.UnitWeightKg = model.UnitWeightKg;
            entity.TotalWeightKg = model.TotalWeightKg;

            entity.ExchangeRate = model.ExchangeRate;

            entity.UnitPriceTk = model.UnitPriceTk;
            entity.UnitPriceRmb = model.UnitPriceRmb;

            entity.ImportTaxPerKgTk = model.ImportTaxPerKgTk;
            entity.ImportTaxPerKgRmb = model.ImportTaxPerKgRmb;

            entity.CostPriceKgTk = model.CostPriceKgTk;
            entity.CostPriceLbsTk = model.CostPriceLbsTk;
            entity.CostPricePerPieceTk = model.CostPricePerPieceTk;

            entity.CostPriceKgRmb = model.CostPriceKgRmb;
            entity.CostPriceLbsRmb = model.CostPriceLbsRmb;
            entity.CostPricePerPieceRmb = model.CostPricePerPieceRmb;

            entity.TotalAmountWithoutTaxTk = model.TotalAmountWithoutTaxTk;
            entity.TotalAmountIncludingTaxTk = model.TotalAmountIncludingTaxTk;

            entity.TotalAmountWithoutTaxRmb = model.TotalAmountWithoutTaxRmb;
            entity.TotalAmountIncludingTaxRmb = model.TotalAmountIncludingTaxRmb;

            entity.Remarks = model.Remarks;

            // Preserve CreatedDate and CreatedBy
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = "Admin"; // Replace with logged-in username

            await _purchaseService.UpdateAsync(entity);

            TempData["Success"] = "Purchase updated successfully.";

            return RedirectToAction(nameof(Index));
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