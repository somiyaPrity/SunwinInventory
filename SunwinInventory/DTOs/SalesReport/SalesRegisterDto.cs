using System;
using System.Collections.Generic;

namespace SunwinInventory.DTOs.SalesReport
{
    public class SalesRegisterRowDto
    {
        public int SalesInvoiceId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string InvoiceNo { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string Items { get; set; } = string.Empty;

        public decimal SalesAmount { get; set; }

        public decimal Cash { get; set; }

        public decimal Bank { get; set; }

        public decimal Bkash { get; set; }

        public decimal Nagad { get; set; }

        public decimal TapRoket { get; set; }

        public decimal ProductExchange { get; set; }

        public decimal PaymentAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal DueAmount { get; set; }

        public decimal DueCollection { get; set; }

        public decimal NetDue { get; set; }
    }

    public class SalesRegisterTotalsDto
    {
        public decimal SalesAmount { get; set; }

        public decimal Cash { get; set; }

        public decimal Bank { get; set; }

        public decimal Bkash { get; set; }

        public decimal Nagad { get; set; }

        public decimal TapRoket { get; set; }

        public decimal ProductExchange { get; set; }

        public decimal PaymentAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal DueAmount { get; set; }

        public decimal DueCollection { get; set; }

        public decimal NetDue { get; set; }
    }

    public class SalesRegisterDto
    {
        public string Title { get; set; } = "Sales Register";

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<SalesRegisterRowDto> Rows { get; set; } = new();

        public SalesRegisterTotalsDto Totals { get; set; } = new();
    }
}