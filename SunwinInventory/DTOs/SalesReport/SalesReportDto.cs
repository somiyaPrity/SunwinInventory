namespace SunwinInventory.DTOs.SalesReport
{
    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int InvoiceCount { get; set; }

        public List<SalesReportDayDto> Days { get; set; } = new();

        public SalesReportTotalsDto Totals { get; set; } = new();

        public string Title => FromDate.Year == ToDate.Year &&
                               FromDate.Month == ToDate.Month
            ? $"SALES REPORT OF {FromDate:MMM-yyyy}".ToUpper()
            : $"SALES REPORT ({FromDate:dd-MMM-yyyy} TO {ToDate:dd-MMM-yyyy})".ToUpper();
    }

    public class SalesReportDayDto
    {
        public DateTime Date { get; set; }

        public List<SalesReportRowDto> Rows { get; set; } = new();

        public SalesReportTotalsDto Totals { get; set; } = new();
    }

    public class SalesReportRowDto
    {
        public int SalesInvoiceId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string InvoiceNo { get; set; } = "";

        public string CustomerName { get; set; } = "";

        public string? CustomerMobile { get; set; }

        public string? SalesMan { get; set; }

        public string ProductName { get; set; } = "";

        public string? ColorName { get; set; }

        public decimal? Thickness { get; set; }

        public string? ProductSize { get; set; }

        public int Quantity { get; set; }

        public decimal WeightKg { get; set; }

        public decimal WeightLbs => WeightKg * 2.20462m;

        public decimal? LbsRate => WeightKg > 0
            ? LineTotal / WeightLbs
            : null;

        public decimal? KgRate => WeightKg > 0
            ? LineTotal / WeightKg
            : null;

        public decimal? Discount { get; set; }

        public decimal LineTotal { get; set; }

        public decimal? OtherCharges { get; set; }

        public decimal? Paid { get; set; }

        public decimal? Due { get; set; }
    }

    public class SalesReportTotalsDto
    {
        public int Quantity { get; set; }

        public decimal WeightKg { get; set; }

        public decimal WeightLbs => WeightKg * 2.20462m;

        public decimal Taka { get; set; }

        public decimal OtherCharges { get; set; }

        public decimal Discount { get; set; }

        public decimal Paid { get; set; }

        public decimal Due { get; set; }
    }
}
