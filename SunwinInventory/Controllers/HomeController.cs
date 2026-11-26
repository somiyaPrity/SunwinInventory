using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.Dashboard;

namespace SunwinInventory.Controllers
{
    public class HomeController : Controller
    {
        private const int LowStockThreshold = 5;

        private readonly SunwinInventoryContext _context;

        public HomeController(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var offsetMinutes = 0;

            if (Request.Cookies.TryGetValue("tz_offset", out var tzValue) &&
                int.TryParse(tzValue, out var parsedOffset))
            {
                offsetMinutes = Math.Clamp(parsedOffset, -840, 840);
            }

            var today = DateTime.UtcNow.AddMinutes(-offsetMinutes).Date;
            var tomorrow = today.AddDays(1);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var trendStart = today.AddDays(-6);

            var model = new DashboardDto
            {
                TotalProducts = await _context.ProductNames
                    .CountAsync(x => x.IsActive),

                TotalCustomers = await _context.Customers
                    .CountAsync(x => x.IsActive),

                TotalWarehouses = await _context.Warehouses
                    .CountAsync(x => x.IsActive == true),

                TodaySales = await _context.SalesInvoices
                    .Where(x => x.InvoiceDate >= today && x.InvoiceDate < tomorrow)
                    .SumAsync(x => x.TotalAmount ?? 0),

                TodayInvoiceCount = await _context.SalesInvoices
                    .CountAsync(x => x.InvoiceDate >= today && x.InvoiceDate < tomorrow),

                MonthSales = await _context.SalesInvoices
                    .Where(x => x.InvoiceDate >= monthStart && x.InvoiceDate < tomorrow)
                    .SumAsync(x => x.TotalAmount ?? 0),

                MonthInvoiceCount = await _context.SalesInvoices
                    .CountAsync(x => x.InvoiceDate >= monthStart && x.InvoiceDate < tomorrow),

                TotalDue = await _context.SalesInvoices
                    .Where(x => (x.DueAmount ?? 0) > 0)
                    .SumAsync(x => x.DueAmount ?? 0)
                    + await _context.Customers.SumAsync(x => x.PreviousDue),

                DueInvoiceCount = await _context.SalesInvoices
                    .CountAsync(x => (x.DueAmount ?? 0) > 0),

                LowStockThreshold = LowStockThreshold
            };

            model.RecentInvoices = await (
                from i in _context.SalesInvoices

                join c in _context.Customers
                    on i.CustomerId equals c.CustomerId

                orderby i.InvoiceDate descending,
                        i.SalesInvoiceId descending

                select new RecentInvoiceDto
                {
                    SalesInvoiceId = i.SalesInvoiceId,
                    InvoiceNo = i.InvoiceNo,
                    CustomerName = c.CustomerName,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount ?? 0,
                    DueAmount = i.DueAmount ?? 0
                })
                .Take(8)
                .ToListAsync();

            model.SalesTrend = await BuildSalesTrendAsync(trendStart, tomorrow);

            await LoadLowStockAsync(model);

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied(string? menu)
        {
            ViewData["Title"] = "Access Denied";

            ViewBag.Menu = menu;

            return View();
        }

        private async Task<List<DailySalesDto>> BuildSalesTrendAsync(
            DateTime start,
            DateTime end)
        {
            var invoices = await _context.SalesInvoices
                .Where(x => x.InvoiceDate >= start && x.InvoiceDate < end)
                .Select(x => new { x.InvoiceDate, x.TotalAmount })
                .ToListAsync();

            return Enumerable
                .Range(0, 7)
                .Select(i =>
                {
                    var date = start.AddDays(i);

                    return new DailySalesDto
                    {
                        Date = date,
                        Amount = invoices
                            .Where(x => x.InvoiceDate.Date == date)
                            .Sum(x => x.TotalAmount ?? 0)
                    };
                })
                .ToList();
        }

        private async Task LoadLowStockAsync(DashboardDto model)
        {
            var stock = await (
                from s in _context.StockLedgers

                join p in _context.ProductNames
                    on s.ProductNameId equals p.ProductNameId

                join c in _context.Colors
                    on s.ColorId equals (int?)c.ColorId
                    into colorGroup

                from c in colorGroup.DefaultIfEmpty()

                select new
                {
                    s.ProductNameId,
                    ProductName = p.Name,
                    s.ColorId,
                    ColorName = c != null ? c.ColorName : null,
                    s.AvailableQty
                })
                .ToListAsync();

            var lowStock = stock
                .GroupBy(x => new { x.ProductNameId, x.ColorId })
                .Select(g => new LowStockItemDto
                {
                    ProductName = g.First().ProductName,
                    ColorName = g.First().ColorName,
                    AvailableQty = g.Sum(x => x.AvailableQty)
                })
                .Where(x => x.AvailableQty <= LowStockThreshold)
                .OrderBy(x => x.AvailableQty)
                .ToList();

            model.LowStockCount = lowStock.Count;
            model.LowStockItems = lowStock.Take(10).ToList();
        }
    }
}
