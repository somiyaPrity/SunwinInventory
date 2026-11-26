using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.Sales;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;
using SunwinInventory.Services.Pdf;

namespace SunwinInventory.Controllers
{
    public class SalesController : Controller
    {
        private readonly SunwinInventoryContext _context;
        private readonly ISalesRepository _salesRepository;
        private readonly ISalesService _salesService;
        private readonly IInvoicePdfService _invoicePdfService;
        public SalesController(
            SunwinInventoryContext context,
            ISalesRepository salesRepository,
            IInvoicePdfService invoicePdfService,
            ISalesService salesService)
        {
            _context = context;
            _salesRepository = salesRepository;
           
            _salesService = salesService;
            _invoicePdfService = invoicePdfService;
        }


        // =========================================================
        // SALES ENTRY PAGE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            var model = new SalesEntryDto
            {
                SalesDate = DateTime.Now
            };

            return View(model);
        }


        // =========================================================
        // INVOICE LIST / PRINT
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? invoiceNo,
            DateTime? fromDate,
            DateTime? toDate,
            int? customerId,
            string? status)
        {
            await LoadDropdowns();

            var query =
                from i in _context.SalesInvoices

                join c in _context.Customers
                    on i.CustomerId equals c.CustomerId

                join w in _context.Warehouses
                    on i.WarehouseId equals w.WarehouseId

                select new
                {
                    Invoice = i,
                    Customer = c,
                    Warehouse = w
                };

            if (!string.IsNullOrWhiteSpace(invoiceNo))
            {
                query = query.Where(x =>
                    x.Invoice.InvoiceNo.Contains(invoiceNo));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x =>
                    x.Invoice.InvoiceDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.Invoice.InvoiceDate < endDate);
            }

            if (customerId.HasValue)
            {
                query = query.Where(x =>
                    x.Invoice.CustomerId == customerId.Value);
            }

            switch (status)
            {
                case "paid":
                    query = query.Where(x =>
                        (x.Invoice.DueAmount ?? 0) <= 0);
                    break;

                case "partial":
                    query = query.Where(x =>
                        (x.Invoice.DueAmount ?? 0) > 0 &&
                        (x.Invoice.PaidAmount ?? 0) > 0);
                    break;

                case "due":
                    query = query.Where(x =>
                        (x.Invoice.PaidAmount ?? 0) <= 0 &&
                        (x.Invoice.DueAmount ?? 0) > 0);
                    break;
            }

            var invoices = await query
                .OrderByDescending(x => x.Invoice.InvoiceDate)
                .ThenByDescending(x => x.Invoice.SalesInvoiceId)
                .Select(x => new InvoiceListDto
                {
                    SalesInvoiceId = x.Invoice.SalesInvoiceId,

                    InvoiceNo = x.Invoice.InvoiceNo,

                    InvoiceDate = x.Invoice.InvoiceDate,

                    CustomerName = x.Customer.CustomerName,

                    WarehouseName = x.Warehouse.WarehouseName,

                    ItemCount = _context.SalesDetails.Count(d =>
                        d.SalesInvoiceId == x.Invoice.SalesInvoiceId),

                    TotalAmount = x.Invoice.TotalAmount ?? 0,

                    PaidAmount = x.Invoice.PaidAmount ?? 0,

                    DueAmount = x.Invoice.DueAmount ?? 0,

                    ReceiveMethod = x.Invoice.ReceiveMethod
                })
                .ToListAsync();

            ViewBag.InvoiceNo = invoiceNo;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.CustomerId = customerId;
            ViewBag.Status = status;

            return View(invoices);
        }


        // =========================================================
        // LOAD CUSTOMER / WAREHOUSE / PRODUCT / COLOR
        // =========================================================

        private async Task LoadDropdowns()
        {
            ViewBag.Customers = await _context.Customers
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.CustomerName)
                .Select(x => new SelectListItem
                {
                    Value = x.CustomerId.ToString(),
                    Text = x.CustomerName
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


            ViewBag.Products = await _context.ProductNames
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.ProductNameId.ToString(),
                    Text = x.Name
                })
                .ToListAsync();


            ViewBag.Colors = await _context.Colors
                .OrderBy(x => x.ColorName)
                .Select(x => new SelectListItem
                {
                    Value = x.ColorId.ToString(),
                    Text = x.ColorName
                })
                .ToListAsync();
        }


        // =========================================================
        // GET STOCK - FIFO / SHIPMENT WISE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetStock(
            int warehouseId,
            int productNameId,
            int? colorId,
            decimal? thickness,
            string? productSize)
        {
            if (warehouseId <= 0)
                return BadRequest("Warehouse is required.");

            if (productNameId <= 0)
                return BadRequest("Product is required.");


            var stock =
                await _salesRepository.GetAvailableStockAsync(
                    warehouseId,
                    productNameId,
                    colorId,
                    thickness,
                    productSize);


            return Json(stock);
        }


        // =========================================================
        // GET PRODUCT SIZES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetProductSizes(
            int warehouseId,
            int productNameId,
            int? colorId,
            decimal? thickness)
        {
            if (warehouseId <= 0 || productNameId <= 0)
                return Json(new List<string>());


            var query =
                from stock in _context.StockLedgers

                join purchase in _context.ProductPurchaseDetails
                    on stock.PurchaseDetailId
                    equals purchase.PurchaseDetailId

                where
                    stock.WarehouseId == warehouseId &&
                    stock.ProductNameId == productNameId &&
                    stock.AvailableQty > 0

                select purchase;


            if (colorId.HasValue)
            {
                query = query.Where(x =>
                    x.ColorId == colorId.Value);
            }


            if (thickness.HasValue)
            {
                query = query.Where(x =>
                    x.Thickness == thickness.Value);
            }


            var sizes = await query
                .Where(x => x.ProductSize != null)
                .Select(x => x.ProductSize!)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();


            return Json(sizes);
        }


        // =========================================================
        // GET STOCK OPTIONS - AVAILABLE COLORS + THICKNESSES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetStockOptions(
            int warehouseId,
            int productNameId,
            int? colorId)
        {
            if (warehouseId <= 0 || productNameId <= 0)
            {
                return Json(new
                {
                    colors = new List<object>(),
                    thicknesses = new List<decimal>()
                });
            }


            var query =
                from stock in _context.StockLedgers

                join purchase in _context.ProductPurchaseDetails
                    on stock.PurchaseDetailId
                    equals purchase.PurchaseDetailId

                where
                    stock.WarehouseId == warehouseId &&
                    stock.ProductNameId == productNameId &&
                    stock.AvailableQty > 0

                select purchase;


            var colorIds = await query
                .Select(x => x.ColorId)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();


            var colorNames = await _context.Colors
                .Where(x => colorIds.Contains(x.ColorId))
                .ToDictionaryAsync(x => x.ColorId, x => x.ColorName);


            var colors = colorIds
                .Select(id => new
                {
                    value = id,
                    text = id == 0
                        ? "No Color"
                        : colorNames.TryGetValue(id, out var name)
                            ? name
                            : $"Color {id}"
                })
                .ToList();


            var thicknessQuery = query;

            if (colorId.HasValue)
            {
                thicknessQuery = thicknessQuery.Where(x =>
                    x.ColorId == colorId.Value);
            }


            var thicknesses = await thicknessQuery
                .Where(x => x.Thickness != null)
                .Select(x => x.Thickness!.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();


            return Json(new { colors, thicknesses });
        }


        // =========================================================
        // SAVE SALE
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesEntryDto model)
        {
            UseFriendlySalesEntryMessages(model);

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                var salesInvoiceId =
                    await _salesService.CreateSaleAsync(model);

                if (!salesInvoiceId.HasValue)
                {
                    TempData["Error"] =
                        "Sale could not be completed.";

                    await LoadDropdowns();
                    return View(model);
                }

                var invoiceNo = await _context.SalesInvoices
                    .Where(x => x.SalesInvoiceId == salesInvoiceId.Value)
                    .Select(x => x.InvoiceNo)
                    .FirstOrDefaultAsync();

                TempData["Success"] =
                    $"Sale completed successfully. Invoice No: {invoiceNo}";

                TempData["SalesInvoiceId"] =
                    salesInvoiceId.Value;

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message ?? ex.Message;

                await LoadDropdowns();
                return View(model);
            }
        }
        private void UseFriendlySalesEntryMessages(SalesEntryDto model)
        {
            if (model.SalesDate == default)
            {
                ModelState.Remove(nameof(model.SalesDate));
                ModelState.AddModelError(nameof(model.SalesDate),
                    "Please select the sales date.");
            }

            if (model.CustomerId <= 0)
            {
                ModelState.Remove(nameof(model.CustomerId));
                ModelState.AddModelError(nameof(model.CustomerId),
                    "Please select a customer.");
            }

            if (model.WarehouseId <= 0)
            {
                ModelState.Remove(nameof(model.WarehouseId));
                ModelState.AddModelError(nameof(model.WarehouseId),
                    "Please select a warehouse.");
            }
        }

        //   [HttpPost]
        //   [ValidateAntiForgeryToken]
        //   public async Task<IActionResult> Create(SalesEntryDto model)
        //   {
        //       if (!ModelState.IsValid)
        //       {
        //           await LoadDropdowns();
        //           return View(model);
        //       }

        //       try
        //       {
        //           var salesInvoiceId =
        //               await _salesService.CreateSaleAsync(model);

        //           if (!salesInvoiceId.HasValue)
        //           {
        //               TempData["Error"] =
        //                   "Sale could not be completed.";

        //               await LoadDropdowns();
        //               return View(model);
        //           }

        //           TempData["Success"] =
        //$"Sale completed successfully. Invoice No: INV-{salesInvoiceId.Value:D5}";

        //           TempData["SalesInvoiceId"] = salesInvoiceId.Value;

        //           // Redirect to fresh Create page
        //           return RedirectToAction("Create");
        //       }
        //       catch (Exception ex)
        //       {
        //           TempData["Error"] = ex.Message;

        //           await LoadDropdowns();
        //           return View(model);
        //       }
        //   }


        // =========================================================
        // GET INVOICE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var invoice =
                await _context.SalesInvoices
                    .FirstOrDefaultAsync(x =>
                        x.SalesInvoiceId == id);


            if (invoice == null)
                return NotFound();


            var customer =
                await _context.Customers
                    .FirstOrDefaultAsync(x =>
                        x.CustomerId == invoice.CustomerId);


            var warehouse =
                await _context.Warehouses
                    .FirstOrDefaultAsync(x =>
                        x.WarehouseId == invoice.WarehouseId);


            var details =
                await (
                    from d in _context.SalesDetails

                    join p in _context.ProductNames
                        on d.ProductNameId
                        equals p.ProductNameId

                    join c in _context.Colors
                        on d.ColorId
                        equals c.ColorId
                        into colorGroup

                    from c in colorGroup.DefaultIfEmpty()

                    join shipment in _context.Shipments
                        on d.ShipmentId
                        equals shipment.ShipmentId

                    where d.SalesInvoiceId == invoice.SalesInvoiceId

                    select new
                    {
                        d.SalesDetailId,

                        ProductName = p.Name,

                        ColorName =
                            c != null
                                ? c.ColorName
                                : "",

                        d.Thickness,

                        d.ProductSize,

                        d.Quantity,

                        d.WeightKg,

                        d.UnitPrice,

                        d.LineTotal,

                        ShipmentNo =
                            shipment.ShipmentNo
                    })
                    .ToListAsync();


            ViewBag.Customer = customer;
            ViewBag.Warehouse = warehouse;
            ViewBag.Details = details;


            return View(invoice);
        }


        // =========================================================
        // PDF INVOICE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> DownloadInvoicePdf(int id)
        {
            var invoice =
                await _context.SalesInvoices
                    .FirstOrDefaultAsync(x =>
                        x.SalesInvoiceId == id);

            if (invoice == null)
                return NotFound();

            var pdf =
                await _invoicePdfService
                    .GenerateInvoiceAsync(id);

            return File(
                pdf,
                "application/pdf",
                $"Invoice-{invoice.InvoiceNo}.pdf");
        }
    }
}