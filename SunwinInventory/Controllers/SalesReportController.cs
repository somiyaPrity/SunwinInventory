using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.SalesReport;

namespace SunwinInventory.Controllers
{
    public class SalesReportController : Controller
    {
        private readonly SunwinInventoryContext _context;

        public SalesReportController(SunwinInventoryContext context)
        {
            _context = context;
        }

        // =========================================================
        // SALES REPORT PAGE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? fromDate,
            DateTime? toDate,
            int? customerId,
            int? productNameId,
            int? warehouseId)
        {
            var from = fromDate?.Date ??
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var to = toDate?.Date ?? DateTime.Today;

            if (to < from)
                to = from;

            await LoadDropdowns();
            SetFilterViewBag(from, to, customerId, productNameId, warehouseId);

            var model = await BuildReportAsync(
                from, to, customerId, productNameId, warehouseId);

            return View(model);
        }

        // =========================================================
        // EXPORT EXCEL
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportExcel(
            DateTime? fromDate,
            DateTime? toDate,
            int? customerId,
            int? productNameId,
            int? warehouseId)
        {
            var from = fromDate?.Date ??
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var to = toDate?.Date ?? DateTime.Today;

            if (to < from)
                to = from;

            var model = await BuildReportAsync(
                from, to, customerId, productNameId, warehouseId);

            var file = BuildWorkbook(model);

            var fileName = from.Year == to.Year && from.Month == to.Month
                ? $"Sales-Report-{from:yyyy-MM}.xlsx"
                : $"Sales-Report-{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // =========================================================
        // SALES REGISTER (client workbook format)
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Register(
            DateTime? fromDate,
            DateTime? toDate,
            int? customerId)
        {
            var from = fromDate?.Date ??
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var to = toDate?.Date ?? DateTime.Today;

            if (to < from)
                to = from;

            await LoadDropdowns();

            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");
            ViewBag.CustomerId = customerId;

            var model = await BuildRegisterAsync(from, to, customerId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportRegisterExcel(
            DateTime? fromDate,
            DateTime? toDate,
            int? customerId)
        {
            var from = fromDate?.Date ??
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var to = toDate?.Date ?? DateTime.Today;

            if (to < from)
                to = from;

            var model = await BuildRegisterAsync(from, to, customerId);

            var file = BuildRegisterWorkbook(model);

            var fileName = $"Sales-Register-{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private async Task<SalesRegisterDto> BuildRegisterAsync(
            DateTime start,
            DateTime finish,
            int? customerId)
        {
            var end = finish.AddDays(1);

            var query =
                from i in _context.SalesInvoices

                join c in _context.Customers
                    on i.CustomerId equals c.CustomerId

                join h in _context.SalesHeaders
                    on i.InvoiceNo equals h.InvoiceNo
                    into headerGroup

                from h in headerGroup.DefaultIfEmpty()

                where i.InvoiceDate >= start && i.InvoiceDate < end

                select new
                {
                    i.SalesInvoiceId,
                    i.InvoiceNo,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.Discount,
                    i.CustomerId,
                    c.CustomerName,
                    ReceiveAmount = h != null ? h.ReceiveAmount : null,
                    ReceiveMethod = h != null ? h.ReceiveMethod : i.ReceiveMethod
                };

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            var invoices = await query
                .OrderBy(x => x.InvoiceDate)
                .ThenBy(x => x.InvoiceNo)
                .ToListAsync();

            var ids = invoices.Select(x => x.SalesInvoiceId).ToList();

            var items = await
                (
                    from d in _context.SalesDetails

                    join p in _context.ProductNames
                        on d.ProductNameId equals p.ProductNameId

                    where ids.Contains(d.SalesInvoiceId)

                    select new { d.SalesInvoiceId, p.Name }
                )
                .ToListAsync();

            var itemMap = items
                .GroupBy(x => x.SalesInvoiceId)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ",
                        g.Select(x => x.Name).Distinct()));

            var collections = await _context.CustomerPayments
                .Where(x => ids.Contains(x.SalesInvoiceId))
                .GroupBy(x => x.SalesInvoiceId)
                .Select(g => new
                {
                    SalesInvoiceId = g.Key,
                    Total = g.Sum(x => x.PaymentAmount)
                })
                .ToListAsync();

            var collectionMap = collections
                .ToDictionary(x => x.SalesInvoiceId, x => x.Total);

            var model = new SalesRegisterDto
            {
                FromDate = start,
                ToDate = finish
            };

            foreach (var inv in invoices)
            {
                var salesAmount = inv.TotalAmount ?? 0;
                var received = inv.ReceiveAmount ?? 0;
                var collected = collectionMap.TryGetValue(
                    inv.SalesInvoiceId, out var col) ? col : 0;

                var row = new SalesRegisterRowDto
                {
                    SalesInvoiceId = inv.SalesInvoiceId,
                    InvoiceDate = inv.InvoiceDate,
                    InvoiceNo = inv.InvoiceNo,
                    CustomerName = inv.CustomerName,
                    Items = itemMap.TryGetValue(
                        inv.SalesInvoiceId, out var names) ? names : "",
                    SalesAmount = salesAmount,
                    PaymentAmount = received,
                    Discount = inv.Discount ?? 0,
                    DueAmount = salesAmount - received,
                    DueCollection = collected,
                    NetDue = salesAmount - received - collected
                };

                switch (NormalizeMethod(inv.ReceiveMethod))
                {
                    case "Bank":
                        row.Bank = received;
                        break;
                    case "Bkash":
                        row.Bkash = received;
                        break;
                    case "Nagad":
                        row.Nagad = received;
                        break;
                    case "TapRoket":
                        row.TapRoket = received;
                        break;
                    case "Exchange":
                        row.ProductExchange = received;
                        break;
                    default:
                        row.Cash = received;
                        break;
                }

                model.Rows.Add(row);

                model.Totals.SalesAmount += row.SalesAmount;
                model.Totals.Cash += row.Cash;
                model.Totals.Bank += row.Bank;
                model.Totals.Bkash += row.Bkash;
                model.Totals.Nagad += row.Nagad;
                model.Totals.TapRoket += row.TapRoket;
                model.Totals.ProductExchange += row.ProductExchange;
                model.Totals.PaymentAmount += row.PaymentAmount;
                model.Totals.Discount += row.Discount;
                model.Totals.DueAmount += row.DueAmount;
                model.Totals.DueCollection += row.DueCollection;
                model.Totals.NetDue += row.NetDue;
            }

            return model;
        }

        private static string NormalizeMethod(string? method)
        {
            switch (method?.Trim().ToLowerInvariant())
            {
                case "bank":
                case "cheque":
                    return "Bank";
                case "bkash":
                    return "Bkash";
                case "nagad":
                    return "Nagad";
                case "tap":
                case "roket":
                    return "TapRoket";
                case "product exchange":
                    return "Exchange";
                default:
                    return "Cash";
            }
        }

        private static byte[] BuildRegisterWorkbook(SalesRegisterDto model)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Sales Register");

            const int lastColumn = 17;

            ws.Cell(1, 1).Value = model.Title;
            ws.Range(1, 1, 1, lastColumn).Merge();
            ws.Cell(1, 1)
                .Style.Font.SetBold()
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Totals above the headers, like the client workbook.
            ws.Cell(2, 6).Value = model.Totals.SalesAmount;
            ws.Cell(2, 7).Value = model.Totals.Cash;
            ws.Cell(2, 8).Value = model.Totals.Bank;
            ws.Cell(2, 9).Value = model.Totals.Bkash;
            ws.Cell(2, 10).Value = model.Totals.Nagad;
            ws.Cell(2, 11).Value = model.Totals.TapRoket;
            ws.Cell(2, 12).Value = model.Totals.ProductExchange;
            ws.Cell(2, 13).Value = model.Totals.PaymentAmount;
            ws.Cell(2, 14).Value = model.Totals.Discount;
            ws.Cell(2, 15).Value = model.Totals.DueAmount;
            ws.Cell(2, 16).Value = model.Totals.DueCollection;
            ws.Cell(2, 17).Value = model.Totals.NetDue;
            ws.Range(2, 6, 2, lastColumn).Style.Font.SetBold();

            var headers = new[]
            {
                "SL No", "Date", "Memo No", "Client Name", "Saled Item",
                "Sales Amount", "Net Payment", "", "", "", "", "",
                "Payment Amount", "Discount / Commission",
                "Due Amount", "Due Collection", "Net Due"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(3, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Alignment.SetWrapText();
            }

            ws.Range(3, 7, 3, 12).Merge();

            var subHeaders = new[]
            {
                "Cash", "Bank Account", "Bkash", "Nagad",
                "Tap/Roket", "Product Exchange"
            };

            for (int i = 0; i < subHeaders.Length; i++)
            {
                var cell = ws.Cell(4, 7 + i);
                cell.Value = subHeaders[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }

            ws.Range(3, 1, 4, lastColumn)
                .Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

            int rowIndex = 5;
            int sl = 1;

            foreach (var row in model.Rows)
            {
                ws.Cell(rowIndex, 1).Value = sl++;
                ws.Cell(rowIndex, 2).Value = row.InvoiceDate;
                ws.Cell(rowIndex, 3).Value = row.InvoiceNo;
                ws.Cell(rowIndex, 4).Value = row.CustomerName;
                ws.Cell(rowIndex, 5).Value = row.Items;
                ws.Cell(rowIndex, 6).Value = row.SalesAmount;
                ws.Cell(rowIndex, 7).Value = row.Cash;
                ws.Cell(rowIndex, 8).Value = row.Bank;
                ws.Cell(rowIndex, 9).Value = row.Bkash;
                ws.Cell(rowIndex, 10).Value = row.Nagad;
                ws.Cell(rowIndex, 11).Value = row.TapRoket;
                ws.Cell(rowIndex, 12).Value = row.ProductExchange;
                ws.Cell(rowIndex, 13).Value = row.PaymentAmount;
                ws.Cell(rowIndex, 14).Value = row.Discount;
                ws.Cell(rowIndex, 15).Value = row.DueAmount;
                ws.Cell(rowIndex, 16).Value = row.DueCollection;
                ws.Cell(rowIndex, 17).Value = row.NetDue;
                rowIndex++;
            }

            ws.Cell(rowIndex, 5).Value = "TOTAL";
            ws.Cell(rowIndex, 6).Value = model.Totals.SalesAmount;
            ws.Cell(rowIndex, 7).Value = model.Totals.Cash;
            ws.Cell(rowIndex, 8).Value = model.Totals.Bank;
            ws.Cell(rowIndex, 9).Value = model.Totals.Bkash;
            ws.Cell(rowIndex, 10).Value = model.Totals.Nagad;
            ws.Cell(rowIndex, 11).Value = model.Totals.TapRoket;
            ws.Cell(rowIndex, 12).Value = model.Totals.ProductExchange;
            ws.Cell(rowIndex, 13).Value = model.Totals.PaymentAmount;
            ws.Cell(rowIndex, 14).Value = model.Totals.Discount;
            ws.Cell(rowIndex, 15).Value = model.Totals.DueAmount;
            ws.Cell(rowIndex, 16).Value = model.Totals.DueCollection;
            ws.Cell(rowIndex, 17).Value = model.Totals.NetDue;

            ws.Range(rowIndex, 1, rowIndex, lastColumn)
                .Style.Font.SetBold()
                .Border.SetTopBorder(XLBorderStyleValues.Double);

            ws.Column(2).Style.DateFormat.Format = "dd-MM-yyyy";
            ws.Range(2, 6, rowIndex, lastColumn)
                .Style.NumberFormat.Format = "#,##0.00";

            ws.SheetView.FreezeRows(4);

            ws.Column(1).Width = 7;
            ws.Column(2).Width = 12;
            ws.Column(3).Width = 12;
            ws.Column(4).Width = 32;
            ws.Column(5).Width = 30;
            for (int col = 6; col <= lastColumn; col++)
                ws.Column(col).Width = 14;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        // =========================================================
        // BUILD REPORT DATA
        // =========================================================

        private async Task<SalesReportDto> BuildReportAsync(
            DateTime start,
            DateTime finish,
            int? customerId,
            int? productNameId,
            int? warehouseId)
        {
            var end = finish.AddDays(1);

            var query =
                from d in _context.SalesDetails

                join i in _context.SalesInvoices
                    on d.SalesInvoiceId equals i.SalesInvoiceId

                join c in _context.Customers
                    on i.CustomerId equals c.CustomerId

                join p in _context.ProductNames
                    on d.ProductNameId equals p.ProductNameId

                join col in _context.Colors
                    on d.ColorId equals (int?)col.ColorId
                    into colorGroup

                from col in colorGroup.DefaultIfEmpty()

                where i.InvoiceDate >= start && i.InvoiceDate < end

                select new
                {
                    i.SalesInvoiceId,
                    i.InvoiceNo,
                    i.InvoiceDate,
                    i.CreatedBy,
                    i.Discount,
                    i.TotalAmount,
                    i.PaidAmount,
                    i.DueAmount,
                    i.WarehouseId,

                    i.Vat,
                    i.Ait,
                    i.CuttingCharge,
                    i.DeliveryCharge,

                    c.CustomerId,
                    c.CustomerName,
                    c.Mobile,

                    d.SalesDetailId,
                    d.ProductNameId,
                    ProductName = p.Name,
                    ColorName = col != null ? col.ColorName : null,
                    d.Thickness,
                    d.ProductSize,
                    d.Quantity,
                    d.WeightKg,
                    d.LineTotal
                };

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            if (productNameId.HasValue)
                query = query.Where(x => x.ProductNameId == productNameId.Value);

            if (warehouseId.HasValue)
                query = query.Where(x => x.WarehouseId == warehouseId.Value);

            var data = await query
                .OrderBy(x => x.InvoiceDate)
                .ThenBy(x => x.InvoiceNo)
                .ThenBy(x => x.SalesDetailId)
                .ToListAsync();

            var model = new SalesReportDto
            {
                FromDate = start,
                ToDate = finish,
                InvoiceCount = data
                    .Select(x => x.SalesInvoiceId)
                    .Distinct()
                    .Count()
            };

            int? previousInvoiceId = null;

            foreach (var group in data.GroupBy(x => x.InvoiceDate.Date))
            {
                var day = new SalesReportDayDto
                {
                    Date = group.Key
                };

                foreach (var item in group)
                {
                    var isFirstLine =
                        item.SalesInvoiceId != previousInvoiceId;

                    previousInvoiceId = item.SalesInvoiceId;

                    // Taken from the invoice header so filtering by product
                    // does not move excluded lines into this column.
                    var otherCharges =
                        (item.Vat ?? 0) +
                        (item.Ait ?? 0) +
                        item.CuttingCharge +
                        item.DeliveryCharge;

                    var row = new SalesReportRowDto
                    {
                        SalesInvoiceId = item.SalesInvoiceId,
                        InvoiceDate = item.InvoiceDate,
                        InvoiceNo = item.InvoiceNo,
                        CustomerName = item.CustomerName,
                        CustomerMobile = item.Mobile,
                        SalesMan = item.CreatedBy,
                        ProductName = item.ProductName,
                        ColorName = item.ColorName,
                        Thickness = item.Thickness,
                        ProductSize = item.ProductSize,
                        Quantity = item.Quantity ?? 0,
                        WeightKg = item.WeightKg ?? 0,
                        LineTotal = item.LineTotal ?? 0,

                        // Invoice level figures are shown once per memo
                        // so day totals are not multiplied by line count.
                        Discount = isFirstLine ? item.Discount ?? 0 : null,

                        // VAT + AIT + cutting + delivery (invoice discount
                        // is reported separately, so add it back here).
                        OtherCharges = isFirstLine ? otherCharges : null,
                        Paid = isFirstLine ? item.PaidAmount ?? 0 : null,
                        Due = isFirstLine ? item.DueAmount ?? 0 : null
                    };

                    day.Rows.Add(row);

                    day.Totals.Quantity += row.Quantity;
                    day.Totals.WeightKg += row.WeightKg;
                    day.Totals.Taka += row.LineTotal;
                    day.Totals.OtherCharges += row.OtherCharges ?? 0;
                    day.Totals.Discount += row.Discount ?? 0;
                    day.Totals.Paid += row.Paid ?? 0;
                    day.Totals.Due += row.Due ?? 0;
                }

                model.Days.Add(day);

                model.Totals.Quantity += day.Totals.Quantity;
                model.Totals.WeightKg += day.Totals.WeightKg;
                model.Totals.Taka += day.Totals.Taka;
                model.Totals.OtherCharges += day.Totals.OtherCharges;
                model.Totals.Discount += day.Totals.Discount;
                model.Totals.Paid += day.Totals.Paid;
                model.Totals.Due += day.Totals.Due;
            }

            return model;
        }

        // =========================================================
        // EXCEL WORKBOOK
        // =========================================================

        private static byte[] BuildWorkbook(SalesReportDto model)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Sales Report");

            var headers = new[]
            {
                "Date",
                "Memo No",
                "Client Name",
                "Client Mobile No.",
                "Sales Man",
                "Product Name",
                "Colour",
                "Thickness (MM)",
                "Size (MM)",
                "Quantity (Pcs)",
                "Weight (LBS)",
                "Weight (KG)",
                "(LBS / TK)",
                "(KG / TK)",
                "Discount",
                "Taka (TK)",
                "Other Charges",
                "Paid (TK)",
                "Dues (Tk)"
            };

            const int lastColumn = 19;

            ws.Cell(1, 1).Value = model.Title;
            ws.Range(1, 1, 1, lastColumn).Merge();
            ws.Cell(1, 1)
                .Style.Font.SetBold()
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(2, i + 1);

                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Alignment.SetWrapText();
                cell.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            }

            int rowIndex = 3;

            foreach (var day in model.Days)
            {
                foreach (var row in day.Rows)
                {
                    ws.Cell(rowIndex, 1).Value = row.InvoiceDate;
                    ws.Cell(rowIndex, 2).Value = row.InvoiceNo;
                    ws.Cell(rowIndex, 3).Value = row.CustomerName;
                    ws.Cell(rowIndex, 4).Value = row.CustomerMobile;
                    ws.Cell(rowIndex, 5).Value = row.SalesMan;
                    ws.Cell(rowIndex, 6).Value = row.ProductName;
                    ws.Cell(rowIndex, 7).Value = row.ColorName;
                    ws.Cell(rowIndex, 8).Value = row.Thickness;
                    ws.Cell(rowIndex, 9).Value = row.ProductSize;
                    ws.Cell(rowIndex, 10).Value = row.Quantity;
                    ws.Cell(rowIndex, 11).Value = row.WeightLbs;
                    ws.Cell(rowIndex, 12).Value = row.WeightKg;
                    ws.Cell(rowIndex, 13).Value = row.LbsRate;
                    ws.Cell(rowIndex, 14).Value = row.KgRate;
                    ws.Cell(rowIndex, 15).Value = row.Discount;
                    ws.Cell(rowIndex, 16).Value = row.LineTotal;
                    ws.Cell(rowIndex, 17).Value = row.OtherCharges;
                    ws.Cell(rowIndex, 18).Value = row.Paid;
                    ws.Cell(rowIndex, 19).Value = row.Due;

                    rowIndex++;
                }

                WriteTotalRow(
                    ws,
                    rowIndex,
                    $"{day.Date:dd-MMM-yyyy} Total",
                    day.Totals,
                    lastColumn);

                rowIndex++;
            }

            WriteTotalRow(
                ws,
                rowIndex,
                "GRAND TOTAL",
                model.Totals,
                lastColumn);

            // Formats
            ws.Column(1).Style.DateFormat.Format = "dd-MM-yyyy";

            ws.Column(10).Style.NumberFormat.Format = "#,##0.##";

            for (int col = 11; col <= lastColumn; col++)
                ws.Column(col).Style.NumberFormat.Format = "#,##0.00";

            // Layout
            ws.SheetView.FreezeRows(2);

            ws.Column(1).Width = 12;
            ws.Column(2).Width = 10;
            ws.Column(3).Width = 32;
            ws.Column(4).Width = 16;
            ws.Column(5).Width = 14;
            ws.Column(6).Width = 22;
            ws.Column(7).Width = 12;
            ws.Column(8).Width = 12;
            ws.Column(9).Width = 16;
            ws.Column(10).Width = 13;
            ws.Column(11).Width = 12;
            ws.Column(12).Width = 12;
            ws.Column(13).Width = 11;
            ws.Column(14).Width = 11;
            ws.Column(15).Width = 12;
            ws.Column(16).Width = 14;
            ws.Column(17).Width = 14;
            ws.Column(18).Width = 12;
            ws.Column(19).Width = 14;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private static void WriteTotalRow(
            IXLWorksheet ws,
            int rowIndex,
            string label,
            SalesReportTotalsDto totals,
            int lastColumn)
        {
            var range = ws.Range(rowIndex, 1, rowIndex, lastColumn);

            range.Style.Font.SetBold();
            range.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
            range.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

            ws.Cell(rowIndex, 1).Value = label;
            ws.Cell(rowIndex, 10).Value = totals.Quantity;
            ws.Cell(rowIndex, 11).Value = totals.WeightLbs;
            ws.Cell(rowIndex, 12).Value = totals.WeightKg;
            ws.Cell(rowIndex, 15).Value = totals.Discount;
            ws.Cell(rowIndex, 16).Value = totals.Taka;
            ws.Cell(rowIndex, 17).Value = totals.OtherCharges;
            ws.Cell(rowIndex, 18).Value = totals.Paid;
            ws.Cell(rowIndex, 19).Value = totals.Due;
        }

        // =========================================================
        // DROPDOWNS / FILTER STATE
        // =========================================================

        private async Task LoadDropdowns()
        {
            ViewBag.Customers = await _context.Customers
                .Where(x => x.IsActive)
                .OrderBy(x => x.CustomerName)
                .Select(x => new SelectListItem
                {
                    Value = x.CustomerId.ToString(),
                    Text = x.CustomerName
                })
                .ToListAsync();

            ViewBag.Products = await _context.ProductNames
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.ProductNameId.ToString(),
                    Text = x.Name
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
        }

        private void SetFilterViewBag(
            DateTime from,
            DateTime to,
            int? customerId,
            int? productNameId,
            int? warehouseId)
        {
            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");
            ViewBag.CustomerId = customerId;
            ViewBag.ProductNameId = productNameId;
            ViewBag.WarehouseId = warehouseId;
        }
    }
}
