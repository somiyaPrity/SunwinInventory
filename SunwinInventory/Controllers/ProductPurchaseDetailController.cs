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
using System.Text.RegularExpressions;

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
        //[HttpGet]
        //public async Task<IActionResult> ExportExcel()
        //{
        //    var list = await _purchaseService.GetPurchaseReportAsync();

        //    using var workbook = new XLWorkbook();

        //    var ws = workbook.Worksheets.Add("Purchase Report");

        //    int row = 1;

        //    //==============================
        //    // Header
        //    //==============================

        //    ws.Cell(row, 1).Value = "Date";
        //    ws.Cell(row, 2).Value = "Shipment";
        //    ws.Cell(row, 3).Value = "Supplier";
        //    ws.Cell(row, 4).Value = "Warehouse";
        //    ws.Cell(row, 5).Value = "Product";
        //    ws.Cell(row, 6).Value = "Color";
        //    ws.Cell(row, 7).Value = "Thickness";
        //    ws.Cell(row, 8).Value = "Size";
        //    ws.Cell(row, 9).Value = "Quantity";
        //    ws.Cell(row, 10).Value = "Unit Weight";
        //    ws.Cell(row, 11).Value = "Total Weight";
        //    ws.Cell(row, 12).Value = "Exchange Rate";
        //    ws.Cell(row, 13).Value = "Unit Price TK";
        //    ws.Cell(row, 14).Value = "Unit Price RMB";
        //    ws.Cell(row, 15).Value = "Import Tax TK";
        //    ws.Cell(row, 16).Value = "Import Tax RMB";
        //    ws.Cell(row, 17).Value = "Cost/KG TK";
        //    ws.Cell(row, 18).Value = "Cost/LBS TK";
        //    ws.Cell(row, 19).Value = "Cost/PCS TK";
        //    ws.Cell(row, 20).Value = "Cost/KG RMB";
        //    ws.Cell(row, 21).Value = "Cost/LBS RMB";
        //    ws.Cell(row, 22).Value = "Cost/PCS RMB";
        //    ws.Cell(row, 23).Value = "Total Without Tax TK";
        //    ws.Cell(row, 24).Value = "Total With Tax TK";
        //    ws.Cell(row, 25).Value = "Total Without Tax RMB";
        //    ws.Cell(row, 26).Value = "Total With Tax RMB";
        //    ws.Cell(row, 27).Value = "Remarks";

        //    //============================================
        //    // STEP 4.2 (ADD HERE)
        //    //============================================

        //    ws.Range("A1:AA1").Style.Font.Bold = true;
        //    ws.Range("A1:AA1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
        //    ws.Range("A1:AA1").Style.Font.FontColor = XLColor.White;
        //    ws.Range("A1:AA1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //    //============================================
        //    // STEP 4.4 (ADD HERE)
        //    //============================================

        //    ws.SheetView.FreezeRows(1);

        //    row++;

        //    foreach (var item in list)
        //    {
        //        ws.Cell(row, 1).Value = item.CreatedDate;
        //        ws.Cell(row, 2).Value = item.ShipmentNo;
        //        ws.Cell(row, 3).Value = item.SupplierName;
        //        ws.Cell(row, 4).Value = item.WarehouseName;
        //        ws.Cell(row, 5).Value = item.ProductName;
        //        ws.Cell(row, 6).Value = item.ColorName;
        //        ws.Cell(row, 7).Value = item.Thickness;
        //        ws.Cell(row, 8).Value = item.ProductSize;
        //        ws.Cell(row, 9).Value = item.InitialQuantity;
        //        ws.Cell(row, 10).Value = item.UnitWeightKg;
        //        ws.Cell(row, 11).Value = item.TotalWeightKg;
        //        ws.Cell(row, 12).Value = item.ExchangeRate;
        //        ws.Cell(row, 13).Value = item.UnitPriceTk;
        //        ws.Cell(row, 14).Value = item.UnitPriceRmb;
        //        ws.Cell(row, 15).Value = item.ImportTaxPerKgTk;
        //        ws.Cell(row, 16).Value = item.ImportTaxPerKgRmb;
        //        ws.Cell(row, 17).Value = item.CostPriceKgTk;
        //        ws.Cell(row, 18).Value = item.CostPriceLbsTk;
        //        ws.Cell(row, 19).Value = item.CostPricePerPieceTk;
        //        ws.Cell(row, 20).Value = item.CostPriceKgRmb;
        //        ws.Cell(row, 21).Value = item.CostPriceLbsRmb;
        //        ws.Cell(row, 22).Value = item.CostPricePcsRmb;
        //        ws.Cell(row, 23).Value = item.TotalAmountWithoutTaxTk;
        //        ws.Cell(row, 24).Value = item.TotalAmountIncludingTaxTk;
        //        ws.Cell(row, 25).Value = item.TotalAmountWithoutTaxRmb;
        //        ws.Cell(row, 26).Value = item.TotalAmountIncludingTaxRmb;
        //        ws.Cell(row, 27).Value = item.Remarks;

        //        row++;
        //    }

        //    //============================================
        //    // STEP 4.3 (ADD HERE)
        //    //============================================

        //    var range = ws.Range(1, 1, row - 1, 27);

        //    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        //    ws.Columns().AdjustToContents();

        //    using var stream = new MemoryStream();

        //    workbook.SaveAs(stream);

        //    stream.Position = 0;

        //    return File(
        //        stream.ToArray(),
        //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        $"PurchaseReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        //}
        [HttpGet]
        public async Task<IActionResult> ExportExcel(PurchaseReportFilterDto filter)
        {
            // Get filtered data
            var list = await _purchaseService.GetPurchaseReportAsync(filter);

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

            // Header style
            ws.Range("A1:AA1").Style.Font.Bold = true;
            ws.Range("A1:AA1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Range("A1:AA1").Style.Font.FontColor = XLColor.White;
            ws.Range("A1:AA1").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            ws.SheetView.FreezeRows(1);

            row++;

            //==============================
            // Data
            //==============================

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

            //==============================
            // Formatting
            //==============================

            var range = ws.Range(1, 1, row - 1, 27);

            range.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            range.Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();

            // Number formatting
            ws.Column(7).Style.NumberFormat.Format = "0.00";
            ws.Column(9).Style.NumberFormat.Format = "#,##0";
            ws.Column(10).Style.NumberFormat.Format = "0.0000";
            ws.Column(11).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(12).Style.NumberFormat.Format = "0.00";
            ws.Column(13).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(14).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(15).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(16).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(17).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(18).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(19).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(20).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(21).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(22).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(23).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(24).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(25).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(26).Style.NumberFormat.Format = "#,##0.00";

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

                    CostPricePerPieceRmb = item.CostPricePcsRmb,

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

            XLWorkbook workbook;

            try
            {
                workbook = new XLWorkbook(stream);
            }
            catch
            {
                return BadRequest(
                    "Unable to read the Excel file. " +
                    "Please upload the client Purchase format file.");
            }

            using (workbook)
            {
                var ws = workbook.Worksheet(1);

                // Client Purchase format: headers on row 2, data from row 3
                if (!ws.Cell(2, 3).GetString()
                        .Contains("Product Name", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(
                        "Invalid Excel format. " +
                        "Please upload the client Purchase format file.");
                }

                int lastRow = ws.LastRowUsed()?.RowNumber() ?? 2;

                var selectedShipment =
                    await _shipmentService.GetByIdAsync(shipmentId);

                if (selectedShipment == null)
                {
                    return BadRequest(
                        "Selected shipment was not found.");
                }

                string selectedShipmentKey =
                    NormalizeShipmentNo(selectedShipment.ShipmentNo);

                for (int row = 3; row <= lastRow; row++)
                {
                    // Skip blank product rows
                    if (string.IsNullOrWhiteSpace(ws.Cell(row, 3).GetString()))
                        continue;

                    // B - Shipment no
                    // The selected shipment must match the file, otherwise
                    // every row would be silently saved under the wrong
                    // shipment.
                    string excelShipment =
                        ws.Cell(row, 2).GetString().Trim();

                    if (!string.IsNullOrWhiteSpace(excelShipment) &&
                        NormalizeShipmentNo(excelShipment) != selectedShipmentKey)
                    {
                        return BadRequest(
                            $"Excel row {row} belongs to shipment " +
                            $"'{excelShipment}', but " +
                            $"'{selectedShipment.ShipmentNo}' is selected. " +
                            "Please select the matching shipment and upload again.");
                    }

                    var dto = new ProductPurchaseDetailDto();

                    // =========================================
                    // HEADER VALUES
                    // =========================================

                    dto.ShipmentId = shipmentId;
                    dto.SupplierId = supplierId;
                    dto.WarehouseId = warehouseId;


                    // =========================================
                    // PRODUCT INFORMATION
                    // The client template has no color column.
                    // =========================================

                    // C - Product Name
                    dto.ProductName =
                        ws.Cell(row, 3)
                          .GetString()
                          .Trim();

                    dto.ColorId = 0;
                    dto.ColorName = "";

                    // D - Thickness (MM)
                    dto.Thickness =
                        GetDecimal(ws, row, 4);

                    // E - Size (MM)
                    dto.ProductSize =
                        ws.Cell(row, 5)
                          .GetString()
                          .Trim();


                    // =========================================
                    // QUANTITY / WEIGHT
                    // =========================================

                    // F - Initial Quantity
                    dto.InitialQuantity =
                        (int)GetDecimal(ws, row, 6);

                    // G - Weight KG / PCS
                    dto.UnitWeightKg =
                        GetDecimal(ws, row, 7);

                    // H - Total Weight KG
                    dto.TotalWeightKg =
                        GetDecimal(ws, row, 8);


                    // =========================================
                    // PRICE INFORMATION
                    // =========================================

                    // I - Unit Price TK
                    dto.UnitPriceTk =
                        GetDecimal(ws, row, 9);

                    // J - Transaction Rate
                    dto.ExchangeRate =
                        GetDecimal(ws, row, 10);

                    // K - Unit Price RMB
                    dto.UnitPriceRmb =
                        GetDecimal(ws, row, 11);

                    if (dto.ExchangeRate <= 0 &&
                        dto.UnitPriceRmb != 0)
                    {
                        dto.ExchangeRate =
                            dto.UnitPriceTk / dto.UnitPriceRmb;
                    }


                    // =========================================
                    // TAX / COST
                    // =========================================

                    // L - Tax / KG (TK)
                    dto.ImportTaxPerKgTk =
                        GetDecimal(ws, row, 12);

                    // M - Cost Price / KG including TAX (TK)
                    dto.CostPriceKgTk =
                        GetDecimal(ws, row, 13);

                    // N - Cost Price / LBS including TAX (TK)
                    dto.CostPriceLbsTk =
                        GetDecimal(ws, row, 14);

                    // O - Cost Price / PCS including TAX (TK)
                    dto.CostPricePerPieceTk =
                        GetDecimal(ws, row, 15);


                    // =========================================
                    // TOTAL AMOUNTS
                    // =========================================

                    // P - Total amount without TAX (TK)
                    dto.TotalAmountWithoutTaxTk =
                        GetDecimal(ws, row, 16);

                    // Q - Total amount (RMB)
                    dto.TotalAmountWithoutTaxRmb =
                        GetDecimal(ws, row, 17);

                    // R - Total including TAX (TK)
                    dto.TotalAmountIncludingTaxTk =
                        GetDecimal(ws, row, 18);

                    // S - Total including TAX (RMB)
                    dto.TotalAmountIncludingTaxRmb =
                        GetDecimal(ws, row, 19);


                    // =========================================
                    // RMB CONVERSIONS
                    // =========================================

                    decimal rate = dto.ExchangeRate ?? 0;

                    dto.ImportTaxPerKgRmb =
                        rate != 0
                            ? (dto.ImportTaxPerKgTk ?? 0) / rate
                            : 0;

                    dto.CostPriceKgRmb =
                        rate != 0
                            ? (dto.CostPriceKgTk ?? 0) / rate
                            : 0;

                    dto.CostPriceLbsRmb =
                        rate != 0
                            ? (dto.CostPriceLbsTk ?? 0) / rate
                            : 0;

                    decimal costPerPieceRmb =
                        rate != 0
                            ? (dto.CostPricePerPieceTk ?? 0) / rate
                            : 0;

                    dto.CostPricePerPieceRmb = costPerPieceRmb;
                    dto.CostPricePcsRmb = costPerPieceRmb;


                    // =========================================
                    // LOOKUP PRODUCT ID
                    // =========================================

                    dto.ProductNameId =
                        await _lookupService
                            .GetProductIdAsync(dto.ProductName)
                        ?? 0;

                    if (dto.ProductNameId == 0)
                    {
                        return BadRequest(
                            $"Product '{dto.ProductName}' " +
                            $"(Excel row {row}) was not found. " +
                            "Please add it to Product Names first.");
                    }


                    items.Add(dto);
                }
            }

            return Json(items);
        }


        private static string NormalizeShipmentNo(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = value.Trim().ToLowerInvariant();

            // "Shipment No - 43", "Shipment-2" and "2" all compare as "43"/"2".
            normalized = Regex.Replace(
                normalized,
                @"^\s*shipment(\s+no\.?)?\s*[-–—:]?\s*",
                "");

            return Regex.Replace(normalized, @"[^a-z0-9]", "");
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

                    CostPricePerPieceRmb = item.CostPricePcsRmb,

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

            TempData["Success"] = "Excel Purchase Saved Successfully.";

            return RedirectToAction(nameof(Create));
        }

        
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

            int previousQuantity = entity.InitialQuantity;
            decimal previousWeight = entity.TotalWeightKg ?? 0;

            // Update only the fields posted by the Edit view.
            // Rates, taxes and cost columns are not on the form, so they
            // keep their stored values.
            entity.ShipmentId = model.ShipmentId;
            entity.SupplierId = model.SupplierId;
            entity.WarehouseId = model.WarehouseId;

            entity.ProductNameId = model.ProductNameId;
            entity.ColorId = model.ColorId;

            entity.Thickness = model.Thickness;
            entity.ProductSize = model.ProductSize;

            entity.InitialQuantity = model.InitialQuantity;

            entity.TotalWeightKg = model.TotalWeightKg;

            entity.UnitPriceTk = model.UnitPriceTk;

            entity.TotalAmountIncludingTaxTk = model.TotalAmountIncludingTaxTk;

            entity.Remarks = model.Remarks;

            // =========================================
            // SYNC STOCK LEDGER
            // =========================================

            var ledger =
                await _stockLedgerService
                    .GetByPurchaseDetailIdAsync(entity.PurchaseDetailId);

            int newAvailableQty = 0;
            decimal newAvailableWeight = 0;

            if (ledger != null)
            {
                newAvailableQty =
                    ledger.AvailableQty +
                    (entity.InitialQuantity - previousQuantity);

                newAvailableWeight =
                    ledger.AvailableWeight +
                    ((entity.TotalWeightKg ?? 0) - previousWeight);

                if (newAvailableQty < 0)
                {
                    TempData["Error"] =
                        $"Quantity cannot be reduced below " +
                        $"{previousQuantity - ledger.AvailableQty}, " +
                        "which is already sold.";

                    await LoadDropDowns();

                    return View(model);
                }

                if (newAvailableWeight < 0)
                {
                    TempData["Error"] =
                        $"Total weight cannot be reduced below " +
                        $"{previousWeight - ledger.AvailableWeight:0.####} KG, " +
                        "which is already sold.";

                    await LoadDropDowns();

                    return View(model);
                }
            }

            // Preserve CreatedDate and CreatedBy
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = "Admin"; // Replace with logged-in username

            await _purchaseService.UpdateAsync(entity);

            if (ledger == null)
            {
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
            else
            {
                ledger.WarehouseId = entity.WarehouseId;
                ledger.ProductNameId = entity.ProductNameId;
                ledger.ColorId = entity.ColorId;

                ledger.AvailableQty = newAvailableQty;
                ledger.AvailableWeight = newAvailableWeight;

                await _stockLedgerService.UpdateAsync(ledger);
            }

            TempData["Success"] = "Purchase updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        //=====================================================
        // DELETE
        //=====================================================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _purchaseService.GetEntityByIdAsync(id);

            if (entity == null)
            {
                TempData["Error"] = "Purchase not found.";

                return RedirectToAction(nameof(Index));
            }

            var ledger = await _stockLedgerService
                .GetByPurchaseDetailIdAsync(id);

            if (ledger != null)
            {
                int soldQty = entity.InitialQuantity - ledger.AvailableQty;

                if (soldQty > 0)
                {
                    TempData["Error"] =
                        $"This purchase cannot be deleted. " +
                        $"{soldQty} of {entity.InitialQuantity} " +
                        "unit(s) have already been sold.";

                    return RedirectToAction(nameof(Index));
                }
            }

            await _purchaseService.DeleteAsync(id);

            TempData["Success"] = "Purchase deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}