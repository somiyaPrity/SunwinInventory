using System;
using System.Collections.Generic;

namespace SunwinInventory.DTOs.CustomerPayment
{
    public class CustomerLedgerRowDto
    {
        public DateTime? Date { get; set; }

        public string MemoNo { get; set; } = string.Empty;

        public int? SalesInvoiceId { get; set; }

        public string Item { get; set; } = string.Empty;

        public decimal? SalesAmount { get; set; }

        public decimal? Payment { get; set; }

        public decimal? Discount { get; set; }

        public decimal ReceivableAmount { get; set; }

        public string Narration { get; set; } = string.Empty;

        public bool IsCollection { get; set; }

        public bool IsOpening { get; set; }
    }

    public class CustomerLedgerDto
    {
        public int CustomerId { get; set; }

        public string CustomerCode { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string? Mobile { get; set; }

        public string? Address { get; set; }

        public decimal PreviousDue { get; set; }

        public List<CustomerLedgerRowDto> Rows { get; set; } = new();

        public decimal TotalSales { get; set; }

        public decimal TotalReceived { get; set; }

        public decimal TotalCollections { get; set; }

        public decimal NetReceivable { get; set; }
    }
}