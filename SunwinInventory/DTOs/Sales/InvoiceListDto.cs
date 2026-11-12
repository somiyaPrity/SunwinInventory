namespace SunwinInventory.DTOs.Sales
{
    public class InvoiceListDto
    {
        public int SalesInvoiceId { get; set; }

        public string InvoiceNo { get; set; } = "";

        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; } = "";

        public string? WarehouseName { get; set; }

        public int ItemCount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public string? ReceiveMethod { get; set; }
    }
}
