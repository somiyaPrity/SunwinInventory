using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.FinancialReport;

namespace SunwinInventory.Controllers
{
    public class FinancialReportController : Controller
    {
        private readonly SunwinInventoryContext _context;

        public FinancialReportController(SunwinInventoryContext context)
        {
            _context = context;
        }

        // =========================================================
        // INCOME STATEMENT PAGE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> IncomeStatement(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var from = fromDate?.Date ??
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var to = toDate?.Date ?? DateTime.Today;

            if (to < from)
                to = from;

            ViewBag.FromDate = from.ToString("yyyy-MM-dd");
            ViewBag.ToDate = to.ToString("yyyy-MM-dd");

            var model = await BuildIncomeStatementAsync(from, to);

            return View(model);
        }

        // =========================================================
        // EXPORT EXCEL
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportIncomeStatementExcel(
            DateTime? fromDate,
            DateTime? toDate)
        {
            var from = fromDate?.Date ??
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var to = toDate?.Date ?? DateTime.Today;

            if (to < from)
                to = from;

            var model = await BuildIncomeStatementAsync(from, to);

            var bytes = BuildWorkbook(model);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Income-Statement-{from:yyyy-MM-dd}-to-{to:yyyy-MM-dd}.xlsx");
        }

        // =========================================================
        // BALANCE SHEET PAGE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> BalanceSheet(DateTime? asOfDate)
        {
            var asOf = asOfDate?.Date ?? ViewerToday();

            ViewBag.AsOfDate = asOf.ToString("yyyy-MM-dd");

            var model = await BuildBalanceSheetAsync(asOf);

            return View(model);
        }

        // =========================================================
        // BALANCE SHEET EXCEL
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportBalanceSheetExcel(
            DateTime? asOfDate)
        {
            var asOf = asOfDate?.Date ?? ViewerToday();

            var model = await BuildBalanceSheetAsync(asOf);

            var bytes = BuildBalanceSheetWorkbook(model);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Balance-Sheet-{asOf:yyyy-MM-dd}.xlsx");
        }

        // =========================================================
        // REPORT DATA
        // =========================================================

        private async Task<IncomeStatementDto> BuildIncomeStatementAsync(
            DateTime from,
            DateTime to)
        {
            var toExclusive = to.AddDays(1);

            var model = new IncomeStatementDto
            {
                FromDate = from,
                ToDate = to
            };

            var invoices = await _context.SalesInvoices
                .Where(x => x.InvoiceDate >= from && x.InvoiceDate < toExclusive)
                .ToListAsync();

            model.InvoiceCount = invoices.Count;
            model.Discount = invoices.Sum(x => x.Discount ?? 0);
            model.Vat = invoices.Sum(x => x.Vat ?? 0);
            model.Ait = invoices.Sum(x => x.Ait ?? 0);
            model.CuttingDelivery =
                invoices.Sum(x => x.CuttingCharge + x.DeliveryCharge);

            var invoiceIds = invoices
                .Select(x => x.SalesInvoiceId)
                .ToList();

            var soldLines = await (
                from sd in _context.SalesDetails

                join ppd in _context.ProductPurchaseDetails
                    on sd.PurchaseDetailId equals ppd.PurchaseDetailId

                join p in _context.ProductNames
                    on sd.ProductNameId equals p.ProductNameId

                where invoiceIds.Contains(sd.SalesInvoiceId)

                select new
                {
                    ProductName = p.Name,
                    Quantity = sd.Quantity ?? 0,
                    WeightKg = sd.WeightKg ?? 0,
                    LineTotal = sd.LineTotal ?? 0,
                    CostPerPiece = ppd.CostPricePerPieceTk,
                    CostPerKg = ppd.CostPriceKgTk
                })
                .ToListAsync();

            var cogsLines = soldLines
                .Select(x =>
                {
                    var cogs = x.Quantity * (x.CostPerPiece ?? 0);

                    if (cogs == 0)
                        cogs = x.WeightKg * (x.CostPerKg ?? 0);

                    if (cogs == 0)
                        model.CostlessLineCount++;

                    return new { x.ProductName, x.Quantity, x.WeightKg, x.LineTotal, Cogs = cogs };
                })
                .ToList();

            model.Sales = cogsLines.Sum(x => x.LineTotal);
            model.SoldQuantity = cogsLines.Sum(x => x.Quantity);
            model.Cogs = cogsLines.Sum(x => x.Cogs);

            model.CogsByProduct = cogsLines
                .GroupBy(x => x.ProductName)
                .Select(g => new IncomeStatementCogsRowDto
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    WeightKg = g.Sum(x => x.WeightKg),
                    SalesAmount = g.Sum(x => x.LineTotal),
                    Cogs = g.Sum(x => x.Cogs)
                })
                .OrderBy(x => x.ProductName)
                .ToList();

            var fundEntries = await _context.FundEntries
                .Where(x => x.EntryDate >= from && x.EntryDate < toExclusive
                    && (x.Head == "Other Income"
                        || x.Head == "Operating Expense"
                        || x.Head == "Other Expense"))
                .OrderBy(x => x.EntryDate)
                .ThenBy(x => x.FundEntryId)
                .ToListAsync();

            model.OtherIncome = fundEntries
                .Where(x => x.Head == "Other Income")
                .Sum(x => x.Amount);

            model.OperatingExpenses = fundEntries
                .Where(x => x.Head == "Operating Expense")
                .Sum(x => x.Amount);

            model.OtherExpenses = fundEntries
                .Where(x => x.Head == "Other Expense")
                .Sum(x => x.Amount);

            model.FundRows = fundEntries
                .Select(x => new IncomeStatementFundRowDto
                {
                    EntryDate = x.EntryDate,
                    Head = x.Head,
                    Details = x.Details,
                    Account = x.Account,
                    Amount = x.Amount
                })
                .ToList();

            return model;
        }

        // =========================================================
        // EXCEL WORKBOOK
        // =========================================================

        private static byte[] BuildWorkbook(IncomeStatementDto model)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Income Statement");

            ws.Cell(1, 1).Value = model.Title;
            ws.Range(1, 1, 1, 5).Merge();
            ws.Cell(1, 1)
                .Style.Font.SetBold()
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            int rowIndex = 3;

            void StatementRow(string label, decimal amount, bool bold = false)
            {
                ws.Cell(rowIndex, 1).Value = label;
                ws.Range(rowIndex, 1, rowIndex, 4).Merge();
                ws.Cell(rowIndex, 5).Value = amount;

                if (bold)
                {
                    ws.Range(rowIndex, 1, rowIndex, 5).Style.Font.SetBold();
                    ws.Range(rowIndex, 1, rowIndex, 5)
                        .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
                }

                rowIndex++;
            }

            StatementRow("Sales", model.Sales);
            StatementRow("Less: Discount", model.Discount);
            StatementRow("NET SALES", model.NetSales, true);
            StatementRow("Less: Cost of Goods Sold", model.Cogs);
            StatementRow("GROSS PROFIT", model.GrossProfit, true);
            StatementRow("Add: Cutting & Delivery Charges", model.CuttingDelivery);
            StatementRow("Add: Other Income", model.OtherIncome);
            StatementRow("Less: Operating Expenses", model.OperatingExpenses);
            StatementRow("Less: Other Expenses", model.OtherExpenses);
            StatementRow("NET PROFIT", model.NetProfit, true);

            rowIndex++;

            StatementRow("VAT collected (not income)", model.Vat);
            StatementRow("AIT collected (not income)", model.Ait);

            rowIndex++;

            ws.Cell(rowIndex, 1).Value = "OTHER INCOME & EXPENSES DETAIL";
            ws.Range(rowIndex, 1, rowIndex, 5).Merge();
            ws.Cell(rowIndex, 1).Style.Font.SetBold();
            rowIndex++;

            var detailHeaders = new[] { "Date", "Head", "Details", "Account", "Amount" };

            for (int i = 0; i < detailHeaders.Length; i++)
            {
                var cell = ws.Cell(rowIndex, i + 1);
                cell.Value = detailHeaders[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
            }

            rowIndex++;

            foreach (var row in model.FundRows)
            {
                ws.Cell(rowIndex, 1).Value = row.EntryDate;
                ws.Cell(rowIndex, 2).Value = row.Head;
                ws.Cell(rowIndex, 3).Value = row.Details;
                ws.Cell(rowIndex, 4).Value = row.Account;
                ws.Cell(rowIndex, 5).Value = row.Amount;
                rowIndex++;
            }

            if (model.FundRows.Count == 0)
            {
                ws.Cell(rowIndex, 1).Value = "No entries in this period.";
                rowIndex++;
            }

            rowIndex++;

            ws.Cell(rowIndex, 1).Value = "COST OF GOODS SOLD BY PRODUCT";
            ws.Range(rowIndex, 1, rowIndex, 5).Merge();
            ws.Cell(rowIndex, 1).Style.Font.SetBold();
            rowIndex++;

            var cogsHeaders = new[] { "Product", "Quantity", "Weight (KG)", "Sales", "COGS" };

            for (int i = 0; i < cogsHeaders.Length; i++)
            {
                var cell = ws.Cell(rowIndex, i + 1);
                cell.Value = cogsHeaders[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
            }

            rowIndex++;

            foreach (var row in model.CogsByProduct)
            {
                ws.Cell(rowIndex, 1).Value = row.ProductName;
                ws.Cell(rowIndex, 2).Value = row.Quantity;
                ws.Cell(rowIndex, 3).Value = row.WeightKg;
                ws.Cell(rowIndex, 4).Value = row.SalesAmount;
                ws.Cell(rowIndex, 5).Value = row.Cogs;
                rowIndex++;
            }

            ws.Column(1).Width = 32;
            ws.Column(2).Width = 18;
            ws.Column(3).Width = 30;
            ws.Column(4).Width = 14;
            ws.Column(5).Width = 18;

            ws.Column(1).Style.DateFormat.Format = "dd-MM-yyyy";
            ws.Column(5).Style.NumberFormat.Format = "#,##0.00;(#,##0.00)";

            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        // =========================================================
        // BALANCE SHEET DATA
        // =========================================================

        private async Task<BalanceSheetDto> BuildBalanceSheetAsync(
            DateTime asOf)
        {
            var end = asOf.AddDays(1);
            var endOnly = DateOnly.FromDateTime(end);

            var model = new BalanceSheetDto { AsOfDate = asOf };

            // -----------------------------------------------------
            // Stock in trade at landed cost (arrivals - sales)
            // -----------------------------------------------------

            var batches = (await (
                from ppd in _context.ProductPurchaseDetails

                join s in _context.Shipments
                    on ppd.ShipmentId equals s.ShipmentId

                select new
                {
                    ppd.PurchaseDetailId,
                    Date = s.ArrivalDate ?? DateOnly.FromDateTime(ppd.CreatedDate),
                    ppd.InitialQuantity,
                    ppd.TotalWeightKg,
                    ppd.UnitWeightKg,
                    CostPerPiece = ppd.CostPricePerPieceTk,
                    CostPerKg = ppd.CostPriceKgTk
                })
                .ToListAsync())
                .Where(x => x.Date < endOnly)
                .ToList();

            var soldByBatch = await (
                from sd in _context.SalesDetails

                join i in _context.SalesInvoices
                    on sd.SalesInvoiceId equals i.SalesInvoiceId

                where i.InvoiceDate < end

                group sd by sd.PurchaseDetailId into g

                select new
                {
                    PurchaseDetailId = g.Key,
                    Quantity = g.Sum(x => x.Quantity ?? 0),
                    WeightKg = g.Sum(x => x.WeightKg ?? 0)
                })
                .ToListAsync();

            var soldMap = soldByBatch
                .ToDictionary(x => x.PurchaseDetailId);

            foreach (var batch in batches)
            {
                var sold = soldMap.GetValueOrDefault(batch.PurchaseDetailId);

                var remainingQty =
                    batch.InitialQuantity - (sold?.Quantity ?? 0);

                if (remainingQty <= 0)
                    continue;

                var remainingWeight =
                    (batch.TotalWeightKg
                        ?? batch.InitialQuantity * (batch.UnitWeightKg ?? 0))
                    - (sold?.WeightKg ?? 0);

                var value = remainingQty * (batch.CostPerPiece ?? 0);

                if (value == 0)
                    value = remainingWeight * (batch.CostPerKg ?? 0);

                model.StockValue += value;
                model.StockQuantity += remainingQty;
            }

            // -----------------------------------------------------
            // Receivables (positive customer balances)
            // -----------------------------------------------------

            var invoices = await _context.SalesInvoices
                .Where(x => x.InvoiceDate < end)
                .ToListAsync();

            var salesByCustomer = invoices
                .GroupBy(x => x.CustomerId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.TotalAmount ?? 0));

            var receiptsByCustomer = await _context.SalesHeaders
                .Where(x => x.SalesDate < end)
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Amount = g.Sum(x => x.ReceiveAmount ?? 0)
                })
                .ToListAsync();

            var receiptMap = receiptsByCustomer
                .ToDictionary(x => x.CustomerId, x => x.Amount);

            var paymentsByCustomer = await _context.CustomerPayments
                .Where(x => x.PaymentDate < end)
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Amount = g.Sum(x => x.PaymentAmount)
                })
                .ToListAsync();

            var paymentMap = paymentsByCustomer
                .ToDictionary(x => x.CustomerId, x => x.Amount);

            var customers = await _context.Customers
                .Select(x => new { x.CustomerId, x.PreviousDue })
                .ToListAsync();

            foreach (var customer in customers)
            {
                var net = customer.PreviousDue
                    + salesByCustomer.GetValueOrDefault(customer.CustomerId)
                    - receiptMap.GetValueOrDefault(customer.CustomerId)
                    - paymentMap.GetValueOrDefault(customer.CustomerId);

                if (net > 0)
                {
                    model.Receivables += net;
                    model.DueCustomerCount++;
                }
            }

            // -----------------------------------------------------
            // Cash / Bank / Wallet (Fund Book logic to date)
            // -----------------------------------------------------

            var openings = await _context.FundOpeningBalances
                .ToListAsync();

            model.OpeningDate = openings.Count > 0
                ? openings.Min(x => x.OpeningDate)
                : new DateTime(2026, 6, 1);

            var cash = openings
                .Where(x => x.Account == "Cash").Sum(x => x.Amount);

            var bank = openings
                .Where(x => x.Account == "Bank").Sum(x => x.Amount);

            var wallet = openings
                .Where(x => x.Account == "Wallet").Sum(x => x.Amount);

            void AddToAccount(string account, decimal amount)
            {
                switch (account)
                {
                    case "Cash":
                        cash += amount;
                        break;

                    case "Bank":
                        bank += amount;
                        break;

                    case "Wallet":
                        wallet += amount;
                        break;
                }
            }

            if (asOf >= model.OpeningDate)
            {
                var fundStart = model.OpeningDate;

                var saleReceipts = await _context.SalesHeaders
                    .Where(x => x.SalesDate >= fundStart && x.SalesDate < end
                        && (x.ReceiveAmount ?? 0) > 0)
                    .Select(x => new
                    {
                        x.ReceiveMethod,
                        Amount = x.ReceiveAmount ?? 0
                    })
                    .ToListAsync();

                foreach (var receipt in saleReceipts)
                    AddToAccount(
                        AccountForMethod(receipt.ReceiveMethod),
                        receipt.Amount);

                var dueCollections = await _context.CustomerPayments
                    .Where(x => x.PaymentDate >= fundStart && x.PaymentDate < end)
                    .Select(x => new { x.PaymentMethod, x.PaymentAmount })
                    .ToListAsync();

                foreach (var collection in dueCollections)
                    AddToAccount(
                        AccountForMethod(collection.PaymentMethod),
                        collection.PaymentAmount);

                var manualEntries = await _context.FundEntries
                    .Where(x => x.EntryDate >= fundStart && x.EntryDate < end)
                    .ToListAsync();

                foreach (var entry in manualEntries)
                {
                    if (entry.Head == "Other Income")
                    {
                        AddToAccount(entry.Account, entry.Amount);
                    }
                    else if (entry.Head == "Bank Deposit")
                    {
                        cash -= entry.Amount;
                        bank += entry.Amount;
                    }
                    else
                    {
                        AddToAccount(entry.Account, -entry.Amount);
                    }
                }
            }

            model.Cash = cash;
            model.Bank = bank;
            model.Wallet = wallet;

            // -----------------------------------------------------
            // VAT / AIT payable
            // -----------------------------------------------------

            model.VatAitPayable =
                invoices.Sum(x => (x.Vat ?? 0) + (x.Ait ?? 0));

            // -----------------------------------------------------
            // Retained earnings (cumulative income statement)
            // -----------------------------------------------------

            var invoiceIds = invoices
                .Select(x => x.SalesInvoiceId)
                .ToList();

            var soldLines = await (
                from sd in _context.SalesDetails

                join ppd in _context.ProductPurchaseDetails
                    on sd.PurchaseDetailId equals ppd.PurchaseDetailId

                where invoiceIds.Contains(sd.SalesInvoiceId)

                select new
                {
                    Quantity = sd.Quantity ?? 0,
                    WeightKg = sd.WeightKg ?? 0,
                    LineTotal = sd.LineTotal ?? 0,
                    CostPerPiece = ppd.CostPricePerPieceTk,
                    CostPerKg = ppd.CostPriceKgTk
                })
                .ToListAsync();

            var sales = soldLines.Sum(x => x.LineTotal);

            var cogs = soldLines.Sum(x =>
            {
                var lineCogs = x.Quantity * (x.CostPerPiece ?? 0);

                return lineCogs != 0
                    ? lineCogs
                    : x.WeightKg * (x.CostPerKg ?? 0);
            });

            var discount = invoices.Sum(x => x.Discount ?? 0);

            var cuttingDelivery =
                invoices.Sum(x => x.CuttingCharge + x.DeliveryCharge);

            var profitEntries = await _context.FundEntries
                .Where(x => x.EntryDate < end
                    && (x.Head == "Other Income"
                        || x.Head == "Operating Expense"
                        || x.Head == "Other Expense"))
                .ToListAsync();

            var otherIncome = profitEntries
                .Where(x => x.Head == "Other Income").Sum(x => x.Amount);

            var operatingExpenses = profitEntries
                .Where(x => x.Head == "Operating Expense").Sum(x => x.Amount);

            var otherExpenses = profitEntries
                .Where(x => x.Head == "Other Expense").Sum(x => x.Amount);

            model.RetainedEarnings =
                sales - discount - cogs + cuttingDelivery
                + otherIncome - operatingExpenses - otherExpenses;

            return model;
        }

        // =========================================================
        // BALANCE SHEET WORKBOOK
        // =========================================================

        private static byte[] BuildBalanceSheetWorkbook(BalanceSheetDto model)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Balance Sheet");

            ws.Cell(1, 1).Value = model.Title;
            ws.Range(1, 1, 1, 2).Merge();
            ws.Cell(1, 1)
                .Style.Font.SetBold()
                .Font.SetFontSize(14)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            int rowIndex = 3;

            void Section(string label)
            {
                ws.Cell(rowIndex, 1).Value = label;
                ws.Cell(rowIndex, 1).Style.Font.SetBold();
                ws.Cell(rowIndex, 1)
                    .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
                ws.Cell(rowIndex, 2)
                    .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
                rowIndex++;
            }

            void Row(string label, decimal amount, bool bold = false)
            {
                ws.Cell(rowIndex, 1).Value = label;
                ws.Cell(rowIndex, 2).Value = amount;

                if (bold)
                {
                    ws.Range(rowIndex, 1, rowIndex, 2).Style.Font.SetBold();
                    ws.Range(rowIndex, 1, rowIndex, 2)
                        .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F2F2F2"));
                    ws.Range(rowIndex, 1, rowIndex, 2)
                        .Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                }

                rowIndex++;
            }

            Section("ASSETS");
            Row("Stock in Trade (at landed cost)", model.StockValue);
            Row("Accounts Receivable", model.Receivables);
            Row("Cash in Hand", model.Cash);
            Row("Bank Balance", model.Bank);
            Row("Wallet Balance (Bkash/Nagad/Tap/Roket)", model.Wallet);
            Row("TOTAL ASSETS", model.TotalAssets, true);

            rowIndex++;

            Section("LIABILITIES");
            Row("VAT & AIT Payable", model.VatAitPayable);
            Row("Accounts Payable (not tracked)", 0);
            Row("TOTAL LIABILITIES", model.TotalLiabilities, true);

            rowIndex++;

            Section("EQUITY");
            Row("Retained Earnings (cumulative)", model.RetainedEarnings);
            Row("Opening Capital & Unrecorded Financing (balancing figure)",
                model.BalancingFigure);
            Row("TOTAL EQUITY", model.TotalEquity, true);

            rowIndex++;

            Row("TOTAL LIABILITIES & EQUITY",
                model.TotalLiabilitiesAndEquity, true);

            rowIndex++;

            ws.Cell(rowIndex, 1).Value =
                $"Stock: {model.StockQuantity:N0} pcs at landed cost. " +
                $"Receivables: {model.DueCustomerCount} customer(s) with due.";
            rowIndex++;

            ws.Cell(rowIndex, 1).Value =
                "Balancing figure absorbs opening capital and unrecorded purchase financing.";

            ws.Column(1).Width = 52;
            ws.Column(2).Width = 20;
            ws.Column(2).Style.NumberFormat.Format = "#,##0.00;(#,##0.00)";

            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private DateTime ViewerToday()
        {
            var offsetMinutes = 0;

            if (Request.Cookies.TryGetValue("tz_offset", out var tzValue) &&
                int.TryParse(tzValue, out var parsedOffset))
            {
                offsetMinutes = Math.Clamp(parsedOffset, -840, 840);
            }

            return DateTime.UtcNow.AddMinutes(-offsetMinutes).Date;
        }

        private static string AccountForMethod(string? method) =>
            method switch
            {
                "Cash" => "Cash",
                "Bank" => "Bank",
                "Cheque" => "Bank",
                "Bkash" => "Wallet",
                "Nagad" => "Wallet",
                "Tap" => "Wallet",
                "Roket" => "Wallet",
                _ => "Exchange"
            };
    }
}