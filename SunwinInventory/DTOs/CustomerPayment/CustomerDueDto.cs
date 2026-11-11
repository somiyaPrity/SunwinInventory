namespace SunwinInventory.DTOs.CustomerPayment
{
    public class CustomerDueDto
    {
        public int CustomerId { get; set; }

        public string? CustomerCode { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Mobile { get; set; }

        public string? Address { get; set; }

        public decimal PreviousDue { get; set; }

        public decimal TotalInvoiceAmount { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public decimal TotalDueAmount { get; set; }
    }
}