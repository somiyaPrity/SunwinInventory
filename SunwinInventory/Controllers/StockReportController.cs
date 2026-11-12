using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.StockReport;

namespace SunwinInventory.Controllers
{
    public class StockReportController : Controller
    {
        private readonly SunwinInventoryContext _context;

        public StockReportController(SunwinInventoryContext context)
        {
            _context = context;
        }

        // =========================================================
        // CURRENT STOCK REPORT PAGE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            int? productNameId,
            int? colorId,
            int? warehouseId,
            int? shipmentId,
            bool includeZeroStock = false)
        {
            await LoadDropdowns();
            SetFilterViewBag(productNameId, colorId, warehouseId, shipmentId, includeZeroStock);

            var model = await BuildReportAsync(
                productNameId, colorId, warehouseId, shipmentId, includeZeroStock);

            return View(model);
        }

        // =========================================================
        // EXPORT EXCEL
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportExcel(
            int? productNameId,
            int? colorId,
            int? warehouseId,
            int? shipmentId,
            bool includeZeroStock = false)
        {
            var model = await BuildReportAsync(
                productNameId, colorId, warehouseId, shipmentId, includeZeroStock);

            var bytes = BuildWorkbook(model);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Stock-Report-{model.AsOf:yyyy-MM-dd}.xlsx");
        }

        // =========================================================
        // REPORT DATA
        // =========================================================

        private async Task<StockReportDto> BuildReportAsync(
            int? productNameId,
            int? colorId,
            int? warehouseId,
            int? shipmentId,
            bool includeZeroStock)
        {
            var query =
                from sl in _context.StockLedgers

                join ppd in _context.ProductPurchaseDetails
                    on sl.PurchaseDetailId equals ppd.PurchaseDetailId

                join s in _context.Shipments
                    on ppd.ShipmentId equals s.ShipmentId

                join p in _context.ProductNames
                    on sl.ProductNameId equals p.ProductNameId

                join w in _context.Warehouses
                    on sl.WarehouseId equals w.WarehouseId

                join c in _context.Colors
                    on sl.ColorId equals (int?)c.ColorId into colors

                from c in colors.DefaultIfEmpty()

                select new StockLine
                {
                    ShipmentId = s.ShipmentId,
                    ShipmentNo = s.ShipmentNo,
                    ArrivalDate = s.ArrivalDate,
                    ProductNameId = sl.ProductNameId,
                    ProductName = p.Name,
                    ColorId = sl.ColorId,
                    ColorName = c == null ? null : c.ColorName,
                    Thickness = ppd.Thickness,
                    ProductSize = ppd.ProductSize,
                    UnitWeightKg = ppd.UnitWeightKg,
                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.WarehouseName,
                    Quantity = sl.AvailableQty,
                    WeightKg = sl.AvailableWeight
                };

            if (productNameId.HasValue)
                query = query.Where(x => x.ProductNameId == productNameId.Value);

            if (colorId.HasValue)
                query = query.Where(x => x.ColorId == colorId.Value);

            if (shipmentId.HasValue)
                query = query.Where(x => x.ShipmentId == shipmentId.Value);

            if (!includeZeroStock)
                query = query.Where(x => x.Quantity > 0);

            var data = await query.ToListAsync();

            // Warehouse columns come from the whole selection so the pivot
            // keeps its shape when a single warehouse is filtered.
            var warehouses = data
                .GroupBy(x => new { x.WarehouseId, x.WarehouseName })
                .OrderBy(x => x.Key.WarehouseName)
                .Select(x => new StockWarehouseColumnDto
                {
                    WarehouseId = x.Key.WarehouseId,
                    WarehouseName = x.Key.WarehouseName
                })
                .ToList();

            if (warehouseId.HasValue)
            {
                data = data
                    .Where(x => x.WarehouseId == warehouseId.Value)
                    .ToList();
            }

            var rows = data
                .GroupBy(x => new
                {
                    x.ShipmentId,
                    x.ShipmentNo,
                    x.ArrivalDate,
                    x.ProductNameId,
                    x.ProductName,
                    x.ColorId,
                    x.ColorName,
                    x.Thickness,
                    x.ProductSize
                })
                .Select(g =>
                {
                    var quantity = g.Sum(x => x.Quantity);
                    var weightKg = g.Sum(x => x.WeightKg);

                    var unitWeight = g
                        .Where(x => x.UnitWeightKg.HasValue && x.UnitWeightKg > 0)
                        .Select(x => x.UnitWeightKg!.Value)
                        .FirstOrDefault();

                    if (unitWeight <= 0 && quantity > 0)
                        unitWeight = weightKg / quantity;

                    return new StockReportRowDto
                    {
                        ShipmentId = g.Key.ShipmentId,
                        ShipmentNo = g.Key.ShipmentNo,
                        ArrivalDate = g.Key.ArrivalDate.HasValue
                            ? g.Key.ArrivalDate.Value.ToDateTime(TimeOnly.MinValue)
                            : null,
                        ProductName = g.Key.ProductName,
                        ColorName = g.Key.ColorName,
                        Thickness = g.Key.Thickness,
                        ProductSize = g.Key.ProductSize,
                        Quantity = quantity,
                        UnitWeightKg = unitWeight,
                        TotalWeightKg = weightKg,
                        QuantityByWarehouse = g
                            .GroupBy(x => x.WarehouseId)
                            .ToDictionary(w => w.Key, w => w.Sum(x => x.Quantity))
                    };
                })
                .OrderBy(x => x.ProductName)
                .ThenBy(x => x.Thickness)
                .ThenBy(x => x.ProductSize)
                .ThenBy(x => x.ShipmentNo)
                .ToList();

            var model = new StockReportDto
            {
                AsOf = DateTime.Today,
                ProductCount = rows
                    .Select(x => x.ProductName)
                    .Distinct()
                    .Count(),
                Warehouses = warehouses,
                Rows = rows
            };

            foreach (var row in rows)
            {
                model.Totals.Quantity += row.Quantity;
                model.Totals.TotalWeightKg += row.TotalWeightKg;

                foreach (var item in row.QuantityByWarehouse)
                {
                    model.Totals.QuantityByWarehouse[item.Key] =
                        model.Totals.QuantityByWarehouse.GetValueOrDefault(item.Key)
                        + item.Value;
                }
            }

            return model;
        }

        // =========================================================
        // EXCEL WORKBOOK
        // =========================================================

        private static byte[] BuildWorkbook(StockReportDto model)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Current Stock");

            var headers = new List<string>
            {
                "Arrival Date",
                "Shipment No",
                "Product Name",
                "Colour",
                "MM",
                "Size"
            };

            headers.AddRange(model.Warehouses.Select(x => x.WarehouseName));

            headers.Add("Total Quantity");
            headers.Add("Weight Per Pcs (LBS)");
            headers.Add("Total Weight (KG)");
            headers.Add("Total Weight (LBS)");

            var lastColumn = headers.Count;

            ws.Cell(1, 1).Value = model.Title;
            ws.Range(1, 1, 1, lastColumn).Merge();
            ws.Cell(1, 1)
                .Style.Font.SetBold()
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            for (int i = 0; i < headers.Count; i++)
            {
                var cell = ws.Cell(2, i + 1);

                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Alignment.SetWrapText();
                cell.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            }

            var firstWarehouseColumn = 7;
            var totalColumn = firstWarehouseColumn + model.Warehouses.Count;
            var weightPerPcsColumn = totalColumn + 1;
            var weightKgColumn = totalColumn + 2;
            var weightLbsColumn = totalColumn + 3;

            int rowIndex = 3;

            foreach (var row in model.Rows)
            {
                ws.Cell(rowIndex, 1).Value = row.ArrivalDate;
                ws.Cell(rowIndex, 2).Value = row.ShipmentNo;
                ws.Cell(rowIndex, 3).Value = row.ProductName;
                ws.Cell(rowIndex, 4).Value = row.ColorName;
                ws.Cell(rowIndex, 5).Value = row.Thickness;
                ws.Cell(rowIndex, 6).Value = row.ProductSize;

                for (int i = 0; i < model.Warehouses.Count; i++)
                {
                    ws.Cell(rowIndex, firstWarehouseColumn + i).Value =
                        row.QuantityByWarehouse.GetValueOrDefault(
                            model.Warehouses[i].WarehouseId);
                }

                ws.Cell(rowIndex, totalColumn).Value = row.Quantity;
                ws.Cell(rowIndex, weightPerPcsColumn).Value = row.WeightPerPcsLbs;
                ws.Cell(rowIndex, weightKgColumn).Value = row.TotalWeightKg;
                ws.Cell(rowIndex, weightLbsColumn).Value = row.TotalWeightLbs;

                rowIndex++;
            }

            var range = ws.Range(rowIndex, 1, rowIndex, lastColumn);

            range.Style.Font.SetBold();
            range.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
            range.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

            ws.Cell(rowIndex, 1).Value = "GRAND TOTAL";

            for (int i = 0; i < model.Warehouses.Count; i++)
            {
                ws.Cell(rowIndex, firstWarehouseColumn + i).Value =
                    model.Totals.QuantityByWarehouse.GetValueOrDefault(
                        model.Warehouses[i].WarehouseId);
            }

            ws.Cell(rowIndex, totalColumn).Value = model.Totals.Quantity;
            ws.Cell(rowIndex, weightKgColumn).Value = model.Totals.TotalWeightKg;
            ws.Cell(rowIndex, weightLbsColumn).Value = model.Totals.TotalWeightLbs;

            // Formats
            ws.Column(1).Style.DateFormat.Format = "dd-MM-yyyy";

            for (int col = firstWarehouseColumn; col <= totalColumn; col++)
                ws.Column(col).Style.NumberFormat.Format = "#,##0";

            for (int col = weightPerPcsColumn; col <= weightLbsColumn; col++)
                ws.Column(col).Style.NumberFormat.Format = "#,##0.00";

            // Layout
            ws.SheetView.FreezeRows(2);

            ws.Column(1).Width = 12;
            ws.Column(2).Width = 14;
            ws.Column(3).Width = 22;
            ws.Column(4).Width = 12;
            ws.Column(5).Width = 8;
            ws.Column(6).Width = 16;

            for (int col = firstWarehouseColumn; col <= lastColumn; col++)
                ws.Column(col).Width = 14;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        // =========================================================
        // DROPDOWNS / FILTER STATE
        // =========================================================

        private async Task LoadDropdowns()
        {
            ViewBag.Products = await _context.ProductNames
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.ProductNameId.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            ViewBag.Colors = await _context.Colors
                .Where(x => x.IsActive)
                .OrderBy(x => x.ColorName)
                .Select(x => new SelectListItem
                {
                    Value = x.ColorId.ToString(),
                    Text = x.ColorName
                })
                .ToListAsync();

            ViewBag.Warehouses = await _context.Warehouses
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.WarehouseName)
                .Select(x => new SelectListItem
                {
                    Value = x.WarehouseId.ToString(),
                    Text = x.WarehouseName
                })
                .ToListAsync();

            ViewBag.Shipments = await _context.Shipments
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.ShipmentDate)
                .Select(x => new SelectListItem
                {
                    Value = x.ShipmentId.ToString(),
                    Text = x.ShipmentNo
                })
                .ToListAsync();
        }

        private void SetFilterViewBag(
            int? productNameId,
            int? colorId,
            int? warehouseId,
            int? shipmentId,
            bool includeZeroStock)
        {
            ViewBag.ProductNameId = productNameId;
            ViewBag.ColorId = colorId;
            ViewBag.WarehouseId = warehouseId;
            ViewBag.ShipmentId = shipmentId;
            ViewBag.IncludeZeroStock = includeZeroStock;
        }

        private class StockLine
        {
            public int ShipmentId { get; set; }

            public string ShipmentNo { get; set; } = null!;

            public DateOnly? ArrivalDate { get; set; }

            public int ProductNameId { get; set; }

            public string ProductName { get; set; } = null!;

            public int? ColorId { get; set; }

            public string? ColorName { get; set; }

            public decimal? Thickness { get; set; }

            public string? ProductSize { get; set; }

            public decimal? UnitWeightKg { get; set; }

            public int WarehouseId { get; set; }

            public string WarehouseName { get; set; } = null!;

            public int Quantity { get; set; }

            public decimal WeightKg { get; set; }
        }
    }
}
