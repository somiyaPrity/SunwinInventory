using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SunwinInventory.Data;

namespace SunwinInventory.Services.Pdf
{
    public class InvoicePdfService : IInvoicePdfService
    {
        private readonly SunwinInventoryContext _context;
        private readonly IWebHostEnvironment _environment;

        public InvoicePdfService(
            SunwinInventoryContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<byte[]> GenerateInvoiceAsync(int salesInvoiceId)
        {
            // -----------------------------
            // Invoice
            // -----------------------------

            var invoice = await _context.SalesInvoices
                .FirstOrDefaultAsync(x =>
                    x.SalesInvoiceId == salesInvoiceId);

            if (invoice == null)
                throw new Exception("Invoice not found.");


            // -----------------------------
            // Customer
            // -----------------------------

            var customer = await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == invoice.CustomerId);

            if (customer == null)
                throw new Exception("Customer not found.");


            // -----------------------------
            // Warehouse
            // -----------------------------

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == invoice.WarehouseId);

            if (warehouse == null)
                throw new Exception("Warehouse not found.");


            // -----------------------------
            // Sales Details
            // -----------------------------

            var details =
                await (
                    from d in _context.SalesDetails

                    join p in _context.ProductNames
                        on d.ProductNameId equals p.ProductNameId

                    join c in _context.Colors
                        on d.ColorId equals c.ColorId
                        into colorGroup

                    from c in colorGroup.DefaultIfEmpty()

                    where d.SalesInvoiceId == salesInvoiceId

                    orderby d.SalesDetailId

                    select new
                    {
                        Detail = d,
                        ProductName = p.Name,
                        ColorName = c != null
                            ? c.ColorName
                            : null
                    }
                ).ToListAsync();


            // -----------------------------
            // Logo
            // -----------------------------

            string logoPath = Path.Combine(
                _environment.WebRootPath,
                "images",
                "sunwin-logo.png");


            // -----------------------------
            // Generate PDF
            // -----------------------------

            var document =
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);

                        page.Margin(30);

                        page.DefaultTextStyle(
                            x => x.FontSize(9));


                        // =============================
                        // HEADER
                        // =============================

                        page.Header()
                            .Column(header =>
                            {
                                header.Spacing(5);

                                // Logo
                                if (File.Exists(logoPath))
                                {
                                    header.Item()
                                        .AlignCenter()
                                        .Height(60)
                                        .Image(logoPath);
                                }

                                header.Item()
                                    .AlignCenter()
                                    .Text("SALES INVOICE")
                                    .Bold()
                                    .FontSize(16);


                                // Invoice information
                                header.Item()
                                    .PaddingTop(10)
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Column(left =>
                                            {
                                                left.Item()
                                                    .Text(
                                                        $"Invoice No: {invoice.InvoiceNo}");

                                                left.Item()
                                                    .Text(
                                                        $"Invoice Date: {invoice.InvoiceDate:dd-MMM-yyyy}");

                                                left.Item()
                                                    .Text(
                                                        $"Warehouse: {warehouse.WarehouseName}");
                                            });


                                        row.RelativeItem()
                                            .Column(right =>
                                            {
                                                right.Item()
                                                    .Text(
                                                        $"Customer: {customer.CustomerName}");

                                                if (!string.IsNullOrWhiteSpace(
                                                        customer.CustomerCode))
                                                {
                                                    right.Item()
                                                        .Text(
                                                            $"Customer Code: {customer.CustomerCode}");
                                                }

                                                if (!string.IsNullOrWhiteSpace(
                                                        customer.Mobile))
                                                {
                                                    right.Item()
                                                        .Text(
                                                            $"Mobile: {customer.Mobile}");
                                                }
                                            });
                                    });
                            });


                        // =============================
                        // CONTENT
                        // =============================

                        page.Content()
                            .PaddingTop(15)
                            .Column(content =>
                            {
                                // ==========================================
                                // ITEM TABLE
                                // ==========================================

                                content.Item()
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(35);
                                            columns.RelativeColumn(4);
                                            columns.ConstantColumn(65);
                                            columns.ConstantColumn(45);
                                            columns.ConstantColumn(65);
                                            columns.ConstantColumn(80);
                                        });


                                        // ==========================================
                                        // TABLE HEADER
                                        // ==========================================

                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignCenter()
                                                .Text("Sl No.");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Description of goods");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignCenter()
                                                .Text("Quantity");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignCenter()
                                                .Text("Unit");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignRight()
                                                .Text("Rate");

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .AlignRight()
                                                .Text("Amount");
                                        });


                                        // ==========================================
                                        // TABLE DATA
                                        // ==========================================

                                        int sl = 1;

                                        foreach (var item in details)
                                        {
                                            // Sl No.
                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignCenter()
                                                .Text(sl.ToString());


                                            // Description
                                            table.Cell()
                                                .Element(BodyCell)
                                                .Column(description =>
                                                {
                                                    description.Item()
                                                        .Text(item.ProductName)
                                                        .Bold();

                                                    if (!string.IsNullOrWhiteSpace(
                                                            item.ColorName))
                                                    {
                                                        description.Item()
                                                            .Text(
                                                                $"Color: {item.ColorName}");
                                                    }

                                                    if (!string.IsNullOrWhiteSpace(
                                                            item.Detail.ProductSize))
                                                    {
                                                        description.Item()
                                                            .Text(
                                                                $"Size: {item.Detail.ProductSize}");
                                                    }

                                                    if (item.Detail.Thickness.HasValue)
                                                    {
                                                        description.Item()
                                                            .Text(
                                                                $"Thickness: {item.Detail.Thickness.Value:0.##}");
                                                    }
                                                });


                                            // Quantity
                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignCenter()
                                                .Text(
                                                    (item.Detail.Quantity ?? 0)
                                                    .ToString("0.##"));


                                            // Unit
                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignCenter()
                                                .Text(
                                                    item.Detail.Unit ?? "");


                                            // Rate
                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignRight()
                                                .Text(
                                                    (item.Detail.UnitPrice ?? 0)
                                                    .ToString("N2"));


                                            // Amount
                                            table.Cell()
                                                .Element(BodyCell)
                                                .AlignRight()
                                                .Text(
                                                    (item.Detail.LineTotal ?? 0)
                                                    .ToString("N2"));


                                            sl++;
                                        }
                                    });


                                // ==========================================
                                // TOTALS
                                // ==========================================

                                content.Item()
                                    .PaddingTop(15)
                                    .AlignRight()
                                    .Width(280)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.ConstantColumn(110);
                                        });


                                        // Total Amount
                                        table.Cell()
                                            .Element(TotalLabelCell)
                                            .Text("Total Amount");

                                        table.Cell()
                                            .Element(TotalValueCell)
                                            .Text(
                                                $"{(invoice.TotalAmount ?? 0):N2}");


                                        // Discount
                                        table.Cell()
                                            .Element(TotalLabelCell)
                                            .Text("Discount");

                                        table.Cell()
                                            .Element(TotalValueCell)
                                            .Text(
                                                $"{(invoice.Discount ?? 0):N2}");


                                        // VAT
                                        table.Cell()
                                            .Element(TotalLabelCell)
                                            .Text("VAT");

                                        table.Cell()
                                            .Element(TotalValueCell)
                                            .Text(
                                                $"{(invoice.Vat ?? 0):N2}");


                                        // AIT
                                        table.Cell()
                                            .Element(TotalLabelCell)
                                            .Text("AIT");

                                        table.Cell()
                                            .Element(TotalValueCell)
                                            .Text(
                                                $"{(invoice.Ait ?? 0):N2}");


                                        // Grand Total
                                        table.Cell()
                                            .Element(GrandTotalLabelCell)
                                            .Text("Grand Total");

                                        table.Cell()
                                            .Element(GrandTotalValueCell)
                                            .Text(
                                                $"{(invoice.TotalAmount ?? 0):N2}");


                                        // Paid Amount
                                        table.Cell()
                                            .Element(TotalLabelCell)
                                            .Text("Paid Amount");

                                        table.Cell()
                                            .Element(TotalValueCell)
                                            .Text(
                                                $"{(invoice.PaidAmount ?? 0):N2}");


                                        // Due Amount
                                        table.Cell()
                                            .Element(GrandTotalLabelCell)
                                            .Text("Due Amount");

                                        table.Cell()
                                            .Element(GrandTotalValueCell)
                                            .Text(
                                                $"{(invoice.DueAmount ?? 0):N2}");
                                    });


                                // ==========================================
                                // PAYMENT INFORMATION
                                // ==========================================

                                content.Item()
                                    .PaddingTop(15)
                                    .Column(payment =>
                                    {
                                        if (!string.IsNullOrWhiteSpace(
                                                invoice.ReceiveMethod))
                                        {
                                            payment.Item()
                                                .Text(
                                                    $"Receive Method: {invoice.ReceiveMethod}");
                                        }

                                        if (!string.IsNullOrWhiteSpace(
                                                invoice.Remarks))
                                        {
                                            payment.Item()
                                                .PaddingTop(5)
                                                .Text(
                                                    $"Remarks: {invoice.Remarks}");
                                        }
                                    });


                                // ==========================================
                                // SIGNATURES
                                // ==========================================

                                content.Item()
                                    .PaddingTop(45)
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .AlignCenter()
                                            .Column(col =>
                                            {
                                                col.Item()
                                                    .Width(150)
                                                    .AlignCenter()
                                                    .LineHorizontal(1);

                                                col.Item()
                                                    .PaddingTop(5)
                                                    .AlignCenter()
                                                    .Text(
                                                        "Customer Signature");
                                            });


                                        row.ConstantItem(100);


                                        row.RelativeItem()
                                            .AlignCenter()
                                            .Column(col =>
                                            {
                                                col.Item()
                                                    .Width(150)
                                                    .AlignCenter()
                                                    .LineHorizontal(1);

                                                col.Item()
                                                    .PaddingTop(5)
                                                    .AlignCenter()
                                                    .Text(
                                                        "Authorized Signature");
                                            });
                                    });
                            });


                        // =============================
                        // FOOTER
                        // =============================

                        page.Footer()
                            .AlignCenter()
                            .Text(
                                "This is a computer generated invoice.");
                    });
                });


            return document.GeneratePdf();
        }


        // ==========================================
        // TABLE STYLES
        // ==========================================

        static IContainer HeaderCell(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(Colors.Grey.Darken1)
                .Padding(5)
                .DefaultTextStyle(
                    x => x.Bold());
        }


        static IContainer BodyCell(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Padding(5);
        }


        // ==========================================
        // TOTAL STYLES
        // ==========================================

        static IContainer TotalLabelCell(IContainer container)
        {
            return container
                .Padding(4)
                .AlignRight();
        }


        static IContainer TotalValueCell(IContainer container)
        {
            return container
                .Padding(4)
                .AlignRight();
        }


        static IContainer GrandTotalLabelCell(IContainer container)
        {
            return container
                .BorderTop(1)
                .BorderColor(Colors.Grey.Darken1)
                .Padding(5)
                .AlignRight()
                .DefaultTextStyle(
                    x => x.Bold());
        }


        static IContainer GrandTotalValueCell(IContainer container)
        {
            return container
                .BorderTop(1)
                .BorderColor(Colors.Grey.Darken1)
                .Padding(5)
                .AlignRight()
                .DefaultTextStyle(
                    x => x.Bold());
        }
    }
}