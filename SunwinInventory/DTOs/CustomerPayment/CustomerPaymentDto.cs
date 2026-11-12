using System.ComponentModel.DataAnnotations;

namespace SunwinInventory.DTOs.CustomerPayment
{
    public class CustomerPaymentDto
    {
        public int CustomerPaymentId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int SalesInvoiceId { get; set; }

        public string? InvoiceNo { get; set; }

        public string? CustomerName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal PaymentAmount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(30)]
        public string? PaymentMethod { get; set; }

        [StringLength(30)]
        public string? BankName { get; set; }

        [StringLength(30)]
        public string? TransactionCode { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public decimal InvoiceTotal { get; set; }

        public decimal InvoicePaidAmount { get; set; }

        public decimal InvoiceDueAmount { get; set; }
    }
}