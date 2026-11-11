using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SunwinInventory.Data;
using SunwinInventory.Services.Pdf;
using Microsoft.EntityFrameworkCore;

namespace SunwinInventory.Services.Pdf
{
    public class PaymentReceiptPdfService : IPaymentReceiptPdfService
    {
        private readonly SunwinInventoryContext _context;

        public PaymentReceiptPdfService(
            SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GeneratePaymentReceiptAsync(
            int customerPaymentId)
        {
            var payment = await
                (
                    from p in _context.CustomerPayments

                    join c in _context.Customers
                        on p.CustomerId equals c.CustomerId

                    join i in _context.SalesInvoices
                        on p.SalesInvoiceId equals i.SalesInvoiceId

                    where p.CustomerPaymentId == customerPaymentId

                    select new
                    {
                        Payment = p,
                        Customer = c,
                        Invoice = i
                    }
                ).FirstOrDefaultAsync();

            if (payment == null)
                throw new Exception("Payment record not found.");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(30);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .AlignCenter()
                                .Text("SUNWIN INVENTORY")
                                .Bold()
                                .FontSize(18);

                            column.Item()
                                .AlignCenter()
                                .Text("PAYMENT RECEIPT")
                                .Bold()
                                .FontSize(13);

                            column.Item()
                                .PaddingTop(5)
                                .LineHorizontal(1);
                        });

                    page.Content()
                        .PaddingTop(15)
                        .Column(column =>
                        {
                            column.Spacing(8);

                            column.Item().Row(row =>
                            {
                                row.RelativeItem()
                                    .Text($"Receipt Date: {payment.Payment.PaymentDate:dd-MMM-yyyy}");

                                row.RelativeItem()
                                    .AlignRight()
                                    .Text($"Receipt No: {payment.Payment.CustomerPaymentId}");
                            });

                            column.Item()
                                .PaddingTop(10)
                                .Text($"Customer: {payment.Customer.CustomerName}")
                                .Bold();

                            if (!string.IsNullOrWhiteSpace(
                                payment.Customer.Mobile))
                            {
                                column.Item()
                                    .Text($"Mobile: {payment.Customer.Mobile}");
                            }

                            if (!string.IsNullOrWhiteSpace(
                                payment.Customer.Address))
                            {
                                column.Item()
                                    .Text($"Address: {payment.Customer.Address}");
                            }

                            column.Item()
                                .PaddingTop(10)
                                .Text($"Invoice No: {payment.Invoice.InvoiceNo}")
                                .Bold();

                            column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.ConstantColumn(100);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell()
                                            .Background(Colors.Grey.Lighten2)
                                            .Padding(5)
                                            .Text("Description")
                                            .Bold();

                                        header.Cell()
                                            .Background(Colors.Grey.Lighten2)
                                            .Padding(5)
                                            .AlignRight()
                                            .Text("Amount")
                                            .Bold();
                                    });

                                    table.Cell()
                                        .Padding(5)
                                        .Text("Invoice Total");

                                    table.Cell()
                                        .Padding(5)
                                        .AlignRight()
                                        .Text(
                                            $"৳ {payment.Invoice.TotalAmount ?? 0:N2}");

                                    table.Cell()
                                        .Padding(5)
                                        .Text("Payment Received");

                                    table.Cell()
                                        .Padding(5)
                                        .AlignRight()
                                        .Text(
                                            $"৳ {payment.Payment.PaymentAmount:N2}");

                                    table.Cell()
                                        .Padding(5)
                                        .Text("Total Paid");

                                    table.Cell()
                                        .Padding(5)
                                        .AlignRight()
                                        .Text(
                                            $"৳ {payment.Invoice.PaidAmount ?? 0:N2}");

                                    table.Cell()
                                        .Padding(5)
                                        .Text("Remaining Due");

                                    table.Cell()
                                        .Padding(5)
                                        .AlignRight()
                                        .Text(
                                            $"৳ {payment.Invoice.DueAmount ?? 0:N2}");
                                });

                            column.Item()
                                .PaddingTop(10)
                                .Text(
                                    $"Payment Method: {payment.Payment.PaymentMethod ?? "N/A"}");

                            if (!string.IsNullOrWhiteSpace(
                                payment.Payment.Remarks))
                            {
                                column.Item()
                                    .Text(
                                        $"Remarks: {payment.Payment.Remarks}");
                            }

                            column.Item()
                                .PaddingTop(25)
                                .AlignRight()
                                .Text("Authorized Signature")
                                .Bold();

                            column.Item()
                                .PaddingTop(35)
                                .LineHorizontal(1);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for your payment.");
                });
            });

            return document.GeneratePdf();
        }
    }
}