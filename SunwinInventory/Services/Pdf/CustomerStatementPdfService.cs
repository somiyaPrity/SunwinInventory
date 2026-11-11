using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SunwinInventory.Data;
using SunwinInventory.Services.Pdf;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Services
{
    public class CustomerStatementPdfService
        : ICustomerStatementPdfService
    {
        private readonly SunwinInventoryContext _context;

        public CustomerStatementPdfService(
            SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateStatementAsync(
            int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId);

            if (customer == null)
                throw new Exception("Customer not found.");

            var invoices = await _context.SalesInvoices
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.InvoiceDate)
                .ToListAsync();

            var payments = await _context.CustomerPayments
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.PaymentDate)
                .ThenBy(x => x.CustomerPaymentId)
                .ToListAsync();

            var paymentLookup = payments
                .GroupBy(x => x.SalesInvoiceId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

            var rows = new List<StatementRow>();

            foreach (var invoice in invoices)
            {
                if (paymentLookup.TryGetValue(
                    invoice.SalesInvoiceId,
                    out var invoicePayments))
                {
                    foreach (var payment in invoicePayments)
                    {
                        rows.Add(new StatementRow
                        {
                            Date = payment.PaymentDate,
                            InvoiceNo = invoice.InvoiceNo,
                            Description = "Payment",
                            InvoiceAmount = 0,
                            PaymentAmount = payment.PaymentAmount,
                            PaymentMethod = payment.PaymentMethod
                        });
                    }
                }
                else
                {
                    rows.Add(new StatementRow
                    {
                        Date = invoice.InvoiceDate,
                        InvoiceNo = invoice.InvoiceNo,
                        Description = "Invoice",
                        InvoiceAmount = invoice.TotalAmount ?? 0,
                        PaymentAmount = 0
                    });
                }
            }

            // Add invoice rows when payments exist
            foreach (var invoice in invoices)
            {
                var hasPayment = paymentLookup.ContainsKey(
                    invoice.SalesInvoiceId);

                if (hasPayment)
                {
                    rows.Add(new StatementRow
                    {
                        Date = invoice.InvoiceDate,
                        InvoiceNo = invoice.InvoiceNo,
                        Description = "Invoice",
                        InvoiceAmount = invoice.TotalAmount ?? 0,
                        PaymentAmount = 0
                    });
                }
            }

            rows = rows
                .OrderBy(x => x.Date)
                .ThenBy(x =>
                    x.Description == "Invoice" ? 0 : 1)
                .ToList();

            decimal runningBalance =
                customer.PreviousDue;

            foreach (var row in rows)
            {
                runningBalance += row.InvoiceAmount;
                runningBalance -= row.PaymentAmount;

                row.Balance = runningBalance;
            }

            decimal totalInvoice =
                invoices.Sum(x => x.TotalAmount ?? 0);

            decimal totalPayment =
                payments.Sum(x => x.PaymentAmount);

            decimal finalDue =
                customer.PreviousDue
                + totalInvoice
                - totalPayment;

            if (finalDue < 0)
                finalDue = 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.DefaultTextStyle(
                        x => x.FontSize(9));

                    page.Header()
                        .Column(header =>
                        {
                            header.Item()
                                .AlignCenter()
                                .Text("CUSTOMER STATEMENT")
                                .Bold()
                                .FontSize(18);

                            header.Item()
                                .AlignCenter()
                                .Text("Sunwin")
                                .FontSize(12);

                            header.Item()
                                .PaddingTop(10)
                                .LineHorizontal(1);
                        });

                    page.Content()
                        .Column(content =>
                        {
                            content.Item()
                                .PaddingTop(15)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .Text(
                                                    $"Customer: {customer.CustomerName}")
                                                .Bold();

                                            col.Item()
                                                .Text(
                                                    $"Customer Code: {customer.CustomerCode}");

                                            col.Item()
                                                .Text(
                                                    $"Contact: {customer.ContactPerson}");

                                            col.Item()
                                                .Text(
                                                    $"Mobile: {customer.Mobile}");
                                        });

                                    row.RelativeItem()
                                        .AlignRight()
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .Text(
                                                    $"Statement Date: {DateTime.Now:dd-MMM-yyyy}");

                                            col.Item()
                                                .Text(
                                                    $"Previous Due: {customer.PreviousDue:N2}");
                                        });
                                });

                            content.Item()
                                .PaddingTop(15)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(25);
                                        columns.ConstantColumn(70);
                                        columns.ConstantColumn(85);
                                        columns.RelativeColumn();
                                        columns.ConstantColumn(80);
                                        columns.ConstantColumn(80);
                                        columns.ConstantColumn(80);
                                    });

                                    table.Header(header =>
                                    {
                                        HeaderCell(header.Cell(), "SL");
                                        HeaderCell(header.Cell(), "Date");
                                        HeaderCell(header.Cell(), "Invoice No");
                                        HeaderCell(header.Cell(), "Description");
                                        HeaderCell(header.Cell(), "Invoice");
                                        HeaderCell(header.Cell(), "Due Payment");
                                        HeaderCell(header.Cell(), "Balance");
                                    });

                                    int sl = 1;

                                    foreach (var item in rows)
                                    {
                                        table.Cell()
                                            .Element(CellStyle)
                                            .Text(sl++.ToString());

                                        table.Cell()
                                            .Element(CellStyle)
                                            .Text(
                                                item.Date.ToString("dd-MMM-yyyy"));

                                        table.Cell()
                                            .Element(CellStyle)
                                            .Text(item.InvoiceNo);

                                        table.Cell()
                                            .Element(CellStyle)
                                            .Text(
                                                item.Description +
                                                (string.IsNullOrWhiteSpace(
                                                    item.PaymentMethod)
                                                    ? ""
                                                    : $" ({item.PaymentMethod})"));

                                        table.Cell()
                                            .Element(CellStyle)
                                            .AlignRight()
                                            .Text(
                                                item.InvoiceAmount
                                                    .ToString("N2"));

                                        table.Cell()
                                            .Element(CellStyle)
                                            .AlignRight()
                                            .Text(
                                                item.PaymentAmount
                                                    .ToString("N2"));

                                        table.Cell()
                                            .Element(CellStyle)
                                            .AlignRight()
                                            .Text(
                                                item.Balance
                                                    .ToString("N2"));
                                    }
                                });

                            content.Item()
                                .PaddingTop(15)
                                .AlignRight()
                                .Column(col =>
                                {
                                    col.Item()
                                        .Text(
                                            $"Total Invoice: {totalInvoice:N2}")
                                        .Bold();

                                    col.Item()
                                        .Text(
                                            $"Total Payment: {totalPayment:N2}")
                                        .Bold();

                                    col.Item()
                                        .Text(
                                            $"Outstanding Due: {finalDue:N2}")
                                        .Bold()
                                        .FontSize(12);
                                });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(
                            $"Generated on {DateTime.Now:dd-MMM-yyyy HH:mm}");
                });
            });

            return document.GeneratePdf();
        }

        private static void HeaderCell(
            IContainer container,
            string text)
        {
            container
                .Background(Colors.Grey.Darken2)
                .Padding(5)
                .Border(1)
                .BorderColor(Colors.Grey.Darken1)
                .AlignCenter()
                .Text(text)
                .Bold()
                .FontColor(Colors.White);
        }

        private static IContainer CellStyle(
            IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(4);
        }

        private class StatementRow
        {
            public DateTime Date { get; set; }

            public string InvoiceNo { get; set; } = "";

            public string Description { get; set; } = "";

            public decimal InvoiceAmount { get; set; }

            public decimal PaymentAmount { get; set; }

            public string? PaymentMethod { get; set; }

            public decimal Balance { get; set; }
        }
    }
}