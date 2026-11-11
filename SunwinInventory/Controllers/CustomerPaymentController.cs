using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public CustomerPaymentController(
            ICustomerPaymentService paymentService,
            ICustomerService customerService, ICustomerStatementPdfService customerStatementPdfService
            , IPaymentReceiptPdfService paymentReceiptPdfService)
        {
            _paymentService = paymentService;
            _customerService = customerService;
            _customerStatementPdfService = customerStatementPdfService;
            _paymentReceiptPdfService = paymentReceiptPdfService;
            
        }
        [HttpGet]
        public async Task<IActionResult> DownloadCustomerStatement(
    int customerId)
        {
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
                TempData["Error"] = ex.Message;

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
                    ex.Message;
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
    }
}