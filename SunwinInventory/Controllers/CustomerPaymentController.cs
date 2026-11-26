using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.CustomerPayment;
using SunwinInventory.Services;
using SunwinInventory.Services.Interfaces;
using SunwinInventory.Services.Pdf;

namespace SunwinInventory.Controllers
{
    public class CustomerPaymentController : Controller
    {
        private readonly ICustomerPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerStatementPdfService _customerStatementPdfService;
        private readonly IPaymentReceiptPdfService _paymentReceiptPdfService;
        private readonly SunwinInventoryContext _context;
        public CustomerPaymentController(
            ICustomerPaymentService paymentService,
            ICustomerService customerService, ICustomerStatementPdfService customerStatementPdfService
            , IPaymentReceiptPdfService paymentReceiptPdfService,
            SunwinInventoryContext context)
        {
            _paymentService = paymentService;
            _customerService = customerService;
            _customerStatementPdfService = customerStatementPdfService;
            _paymentReceiptPdfService = paymentReceiptPdfService;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> DownloadCustomerStatement(
    int customerId)
        {
            var customer =
                await _customerService.GetByIdAsync(customerId);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";

                return RedirectToAction(nameof(CustomerDue));
            }

            var pdf =
                await _customerStatementPdfService
                    .GenerateStatementAsync(customerId);

            return File(
                pdf,
                "application/pdf",
                $"CustomerStatement_{customerId}_{DateTime.Now:yyyyMMdd}.pdf");
        }
        [HttpGet]
        public async Task<IActionResult> DownloadPaymentReceipt(int id)
        {
            var pdf = await _paymentReceiptPdfService
                .GeneratePaymentReceiptAsync(id);

            return File(
                pdf,
                "application/pdf",
                $"PaymentReceipt_{id}.pdf");
        }

        //=====================================================
        // INDEX
        //=====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var payments =
                await _paymentService.GetAllAsync();

            return View(payments);
        }

        //=====================================================
        // CREATE - GET
        //=====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCustomers();

            var model = new CustomerPaymentDto
            {
                PaymentDate = DateTime.Now
            };

            return View(model);
        }

        //=====================================================
        // CREATE - POST
        //=====================================================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(
        //    CustomerPaymentDto model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await LoadCustomers();

        //        return View(model);
        //    }

        //    try
        //    {
        //        await _paymentService.CreateAsync(model);

        //        TempData["Success"] =
        //            "Payment saved successfully.";

        //        return RedirectToAction(nameof(Create));
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Message;

        //        await LoadCustomers();

        //        return View(model);
        //    }
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerPaymentDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCustomers();
                return View(model);
            }

            try
            {
                var paymentId = await _paymentService.AddPaymentAsync(model);

                if (!paymentId.HasValue)
                {
                    TempData["Error"] = "Payment could not be saved.";
                    await LoadCustomers();
                    return View(model);
                }

                TempData["Success"] = "Payment saved successfully.";

                return RedirectToAction(
                    nameof(PaymentReceipt),
                    new { id = paymentId.Value });
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;

                await LoadCustomers();
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentReceipt(int id)
        {
            var pdf = await _paymentReceiptPdfService
                .GeneratePaymentReceiptAsync(id);

            return File(
                pdf,
                "application/pdf");
        }
        //=====================================================
        // GET CUSTOMER INVOICES
        //=====================================================

        [HttpGet]
        public async Task<IActionResult>
            GetCustomerInvoices(int customerId)
        {
            if (customerId <= 0)
                return Json(new List<object>());

            var invoices =
                await _paymentService
                    .GetCustomerInvoicesAsync(
                        customerId);

            return Json(
                invoices.Select(x => new
                {
                    salesInvoiceId =
                        x.SalesInvoiceId,

                    invoiceNo =
                        x.InvoiceNo,

                    invoiceTotal =
                        x.InvoiceTotal,

                    paidAmount =
                        x.InvoicePaidAmount,

                    dueAmount =
                        x.InvoiceDueAmount
                })
            );
        }

        //=====================================================
        // GET INVOICE INFO
        //=====================================================

        [HttpGet]
        public async Task<IActionResult>
            GetInvoiceInfo(int salesInvoiceId)
        {
            if (salesInvoiceId <= 0)
                return BadRequest();

            var invoice =
                await _paymentService
                    .GetInvoiceInfoAsync(
                        salesInvoiceId);

            if (invoice == null)
                return NotFound();

            return Json(new
            {
                salesInvoiceId =
                    invoice.SalesInvoiceId,

                invoiceNo =
                    invoice.InvoiceNo,

                customerId =
                    invoice.CustomerId,

                customerName =
                    invoice.CustomerName,

                invoiceTotal =
                    invoice.InvoiceTotal,

                paidAmount =
                    invoice.InvoicePaidAmount,

                dueAmount =
                    invoice.InvoiceDueAmount
            });
        }

        //=====================================================
        // INVOICE PAYMENT HISTORY
        //=====================================================

        [HttpGet]
        public async Task<IActionResult>
            InvoicePayments(int salesInvoiceId)
        {
            var payments =
                await _paymentService
                    .GetByInvoiceAsync(
                        salesInvoiceId);

            return View(payments);
        }

        //=====================================================
        // CUSTOMER PAYMENT HISTORY
        //=====================================================

        [HttpGet]
        public async Task<IActionResult>
            CustomerPayments(int customerId)
        {
            var payments =
                await _paymentService
                    .GetByCustomerAsync(
                        customerId);

            return View(payments);
        }

        //=====================================================
        // DELETE
        //=====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                await _paymentService.DeleteAsync(id);

                TempData["Success"] =
                    "Payment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        //=====================================================
        // LOAD CUSTOMERS
        //=====================================================

        private async Task LoadCustomers()
        {
            var customers =
                await _customerService.GetAllAsync();

            ViewBag.Customers =
                new SelectList(
                    customers,
                    "CustomerId",
                    "CustomerName");
        }
        [HttpGet]
        public async Task<IActionResult> CustomerDue()
        {
            var list = await _paymentService.GetCustomerDueReportAsync();

            return View(list);
        }

        // =========================================================
        // RECEIVABLES LEDGER (per-client workbook format)
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> CustomerLedger(int customerId)
        {
            var ledger = await BuildLedgerAsync(customerId);

            if (ledger == null)
            {
                TempData["Error"] = "Customer not found.";

                return RedirectToAction(nameof(CustomerDue));
            }

            return View(ledger);
        }

        [HttpGet]
        public async Task<IActionResult> ExportReceivables()
        {
            var summary = await _paymentService.GetCustomerDueReportAsync();

            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Receivables Summary");

            ws.Cell(1, 1).Value = "Total Receivables";
            ws.Cell(1, 1).Style.Font.SetBold();
            ws.Cell(1, 3).Value = summary.Sum(x => x.TotalDueAmount);
            ws.Cell(1, 3).Style.Font.SetBold();
            ws.Cell(1, 3).Style.NumberFormat.Format = "#,##0.00";

            ws.Cell(2, 1).Value = "SL NO";
            ws.Cell(2, 2).Value = "CLIENT NAME";
            ws.Cell(2, 3).Value = "AMOUNT";
            ws.Range(2, 1, 2, 3).Style.Font.SetBold();
            ws.Range(2, 1, 2, 3)
                .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));

            int rowIndex = 3;
            int sl = 1;

            foreach (var item in summary)
            {
                ws.Cell(rowIndex, 1).Value = sl++;
                ws.Cell(rowIndex, 2).Value = item.CustomerName;
                ws.Cell(rowIndex, 3).Value = item.TotalDueAmount;
                rowIndex++;
            }

            ws.Column(3).Style.NumberFormat.Format = "#,##0.00";
            ws.Column(1).Width = 8;
            ws.Column(2).Width = 36;
            ws.Column(3).Width = 18;
            ws.SheetView.FreezeRows(2);

            var usedNames = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "Receivables Summary"
            };

            foreach (var item in summary)
            {
                var ledger = await BuildLedgerAsync(item.CustomerId);

                if (ledger == null)
                    continue;

                var sheetName = SanitizeSheetName(item.CustomerName);

                var baseName = sheetName;
                int suffix = 2;

                while (!usedNames.Add(sheetName))
                {
                    var tail = $" ({suffix++})";
                    sheetName = baseName.Substring(
                        0, Math.Min(baseName.Length, 31 - tail.Length)) + tail;
                }

                WriteLedgerSheet(workbook.Worksheets.Add(sheetName), ledger);
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Receivables-{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private async Task<CustomerLedgerDto?> BuildLedgerAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            if (customer == null)
                return null;

            var invoices = await _context.SalesInvoices
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.InvoiceDate)
                .ThenBy(x => x.SalesInvoiceId)
                .Select(x => new
                {
                    x.SalesInvoiceId,
                    x.InvoiceNo,
                    x.InvoiceDate,
                    x.TotalAmount,
                    x.Discount,
                    x.ReceiveMethod
                })
                .ToListAsync();

            var ids = invoices.Select(x => x.SalesInvoiceId).ToList();

            var receipts = await
                (
                    from h in _context.SalesHeaders

                    join i in _context.SalesInvoices
                        on h.InvoiceNo equals i.InvoiceNo

                    where i.CustomerId == customerId

                    select new { h.InvoiceNo, h.ReceiveAmount }
                )
                .ToListAsync();

            var receiptMap = receipts
                .GroupBy(x => x.InvoiceNo)
                .ToDictionary(g => g.Key, g => g.First().ReceiveAmount ?? 0);

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
                    g => string.Join(", ", g.Select(x => x.Name).Distinct()));

            var payments = await
                (
                    from p in _context.CustomerPayments

                    join i in _context.SalesInvoices
                        on p.SalesInvoiceId equals i.SalesInvoiceId

                    where i.CustomerId == customerId

                    select new
                    {
                        p.CustomerPaymentId,
                        p.PaymentDate,
                        p.PaymentAmount,
                        p.PaymentMethod,
                        i.InvoiceNo,
                        i.SalesInvoiceId
                    }
                )
                .ToListAsync();

            var ledger = new CustomerLedgerDto
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customer.CustomerCode ?? "",
                CustomerName = customer.CustomerName,
                Mobile = customer.Mobile,
                Address = customer.Address,
                PreviousDue = customer.PreviousDue
            };

            var ordered = new List<(DateTime SortDate, int Order, int Id,
                CustomerLedgerRowDto Row)>();

            if (customer.PreviousDue != 0)
            {
                ledger.Rows.Add(new CustomerLedgerRowDto
                {
                    Date = null,
                    Item = "Previous Due",
                    ReceivableAmount = customer.PreviousDue,
                    Narration = "Opening",
                    IsOpening = true
                });
            }

            foreach (var inv in invoices)
            {
                var salesAmount = inv.TotalAmount ?? 0;
                var received = receiptMap.TryGetValue(
                    inv.InvoiceNo, out var r) ? r : 0;

                var row = new CustomerLedgerRowDto
                {
                    Date = inv.InvoiceDate,
                    MemoNo = inv.InvoiceNo,
                    SalesInvoiceId = inv.SalesInvoiceId,
                    Item = itemMap.TryGetValue(
                        inv.SalesInvoiceId, out var names) ? names : "",
                    SalesAmount = salesAmount,
                    Payment = received > 0 ? received : null,
                    Discount = inv.Discount != 0 ? inv.Discount : null,
                    ReceivableAmount = salesAmount - received,
                    Narration = received > 0
                        ? inv.ReceiveMethod ?? "" : ""
                };

                ordered.Add((inv.InvoiceDate, 1, inv.SalesInvoiceId, row));

                ledger.TotalSales += salesAmount;
                ledger.TotalReceived += received;
            }

            foreach (var pay in payments)
            {
                var row = new CustomerLedgerRowDto
                {
                    Date = pay.PaymentDate,
                    MemoNo = pay.InvoiceNo,
                    SalesInvoiceId = pay.SalesInvoiceId,
                    Payment = pay.PaymentAmount,
                    ReceivableAmount = -pay.PaymentAmount,
                    Narration = pay.PaymentMethod ?? "",
                    IsCollection = true
                };

                ordered.Add((pay.PaymentDate, 2, pay.CustomerPaymentId, row));

                ledger.TotalCollections += pay.PaymentAmount;
            }

            foreach (var entry in ordered
                .OrderBy(x => x.SortDate)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.Id))
            {
                ledger.Rows.Add(entry.Row);
            }

            ledger.NetReceivable =
                ledger.PreviousDue
                + ledger.TotalSales
                - ledger.TotalReceived
                - ledger.TotalCollections;

            return ledger;
        }

        private static void WriteLedgerSheet(
            IXLWorksheet ws,
            CustomerLedgerDto ledger)
        {
            ws.Cell(1, 4).Value = ledger.TotalSales;
            ws.Cell(1, 5).Value = ledger.TotalReceived + ledger.TotalCollections;
            ws.Cell(1, 7).Value = ledger.NetReceivable;
            ws.Range(1, 4, 1, 7).Style.Font.SetBold();

            var headers = new[]
            {
                "Memo No", "Date", "Item", "Sales Amount",
                "Payment/TK", "Discounts", "Total Receivables", "Narration"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(2, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D9D9D9"));
            }

            int rowIndex = 3;

            foreach (var row in ledger.Rows)
            {
                ws.Cell(rowIndex, 1).Value = row.MemoNo;

                if (row.Date.HasValue)
                    ws.Cell(rowIndex, 2).Value = row.Date.Value;

                ws.Cell(rowIndex, 3).Value = row.Item;

                if (row.SalesAmount.HasValue)
                    ws.Cell(rowIndex, 4).Value = row.SalesAmount.Value;

                if (row.Payment.HasValue)
                    ws.Cell(rowIndex, 5).Value = row.Payment.Value;

                if (row.Discount.HasValue)
                    ws.Cell(rowIndex, 6).Value = row.Discount.Value;

                ws.Cell(rowIndex, 7).Value = row.ReceivableAmount;
                ws.Cell(rowIndex, 8).Value = row.Narration;

                rowIndex++;
            }

            ws.Column(2).Style.DateFormat.Format = "dd-MM-yyyy";
            ws.Range(1, 4, rowIndex, 7)
                .Style.NumberFormat.Format = "#,##0.00;(#,##0.00)";

            ws.SheetView.FreezeRows(2);

            ws.Column(1).Width = 12;
            ws.Column(2).Width = 12;
            ws.Column(3).Width = 32;
            ws.Column(4).Width = 16;
            ws.Column(5).Width = 16;
            ws.Column(6).Width = 12;
            ws.Column(7).Width = 18;
            ws.Column(8).Width = 16;
        }

        private static string SanitizeSheetName(string name)
        {
            var invalid = new[] { '\\', '/', '?', '*', '[', ']', ':' };

            var clean = new string(
                name.Select(c => invalid.Contains(c) ? ' ' : c).ToArray())
                .Trim();

            if (clean.Length == 0)
                clean = "Customer";

            return clean.Length > 31 ? clean.Substring(0, 31) : clean;
        }
    }
}