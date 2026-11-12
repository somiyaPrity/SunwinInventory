namespace SunwinInventory.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalWarehouses { get; set; }

        public decimal TodaySales { get; set; }

        public int TodayInvoiceCount { get; set; }

        public decimal MonthSales { get; set; }

        public int MonthInvoiceCount { get; set; }

        public decimal TotalDue { get; set; }

        public int DueInvoiceCount { get; set; }

        public int LowStockCount { get; set; }

        public int LowStockThreshold { get; set; }

        public List<DailySalesDto> SalesTrend { get; set; } = new();

        public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();

        public List<LowStockItemDto> LowStockItems { get; set; } = new();
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }

        public decimal Amount { get; set; }
    }

    public class RecentInvoiceDto
    {
        public int SalesInvoiceId { get; set; }

        public string InvoiceNo { get; set; } = "";

        public string CustomerName { get; set; } = "";

        public DateTime InvoiceDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DueAmount { get; set; }
    }

    public class LowStockItemDto
    {
        public string ProductName { get; set; } = "";

        public string? ColorName { get; set; }

        public int AvailableQty { get; set; }
    }
}
