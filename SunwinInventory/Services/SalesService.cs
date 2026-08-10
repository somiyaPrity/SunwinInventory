using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.Sales;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _repository;
        private readonly SunwinInventoryContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesService(
            ISalesRepository repository,
            SunwinInventoryContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int?> CreateSaleAsync(SalesEntryDto model)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                if (model.Items == null || !model.Items.Any())
                    throw new Exception("No sale items found.");

                var userName =
                    _httpContextAccessor.HttpContext?
                        .Session.GetString("UserName") ?? "System";

                // =====================================================
                // 1. VALIDATE STOCK FIRST
                // =====================================================

                foreach (var item in model.Items)
                {
                    if (item.Quantity <= 0)
                        throw new Exception(
                            "Quantity must be greater than zero.");

                    var stockList =
                        await _repository.GetAvailableStockAsync(
                            model.WarehouseId,
                            item.ProductNameId,
                            item.ColorId,
                            item.Thickness,
                            item.ProductSize);

                    int availableQty =
                        stockList.Sum(x => x.AvailableQty);

                    if (availableQty < item.Quantity)
                    {
                        throw new Exception(
                            $"Insufficient stock. " +
                            $"Required: {item.Quantity}, " +
                            $"Available: {availableQty}.");
                    }
                }


                // =====================================================
                // 2. CALCULATE LINE TOTALS ON SERVER
                // =====================================================

                decimal subtotal = 0;

                foreach (var item in model.Items)
                {
                    subtotal += item.Quantity * item.UnitPrice;
                }

                // =====================================================
                // 3. CALCULATE FINAL AMOUNT
                // =====================================================

                decimal totalAmount =
                    subtotal
                    - model.Discount
                    + model.Vat
                    + model.Ait;

                if (totalAmount < 0)
                    totalAmount = 0;

                decimal dueAmount =
                    totalAmount - model.ReceiveAmount;

                if (dueAmount < 0)
                    dueAmount = 0;


                // =====================================================
                // 4. GENERATE INVOICE NUMBER
                // =====================================================

                var lastInvoice =
                    await _context.SalesInvoices
                        .OrderByDescending(x => x.SalesInvoiceId)
                        .FirstOrDefaultAsync();

                int nextNumber = 1;

                if (lastInvoice != null &&
                    !string.IsNullOrWhiteSpace(lastInvoice.InvoiceNo))
                {
                    var numberPart =
                        new string(
                            lastInvoice.InvoiceNo
                                .Where(char.IsDigit)
                                .ToArray());

                    if (int.TryParse(
                            numberPart,
                            out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                string invoiceNo =
                    $"INV-{nextNumber:D5}";


                // =====================================================
                // 5. CREATE SALES INVOICE
                // =====================================================

                var invoice = new SalesInvoice
                {
                    InvoiceNo = invoiceNo,
                    InvoiceDate = model.SalesDate,

                    CustomerId = model.CustomerId,
                    WarehouseId = model.WarehouseId,

                    TotalAmount = totalAmount,
                    Discount = model.Discount,
                    Vat = model.Vat,
                    Ait = model.Ait,

                    PaidAmount = model.ReceiveAmount,
                    DueAmount = dueAmount,

                    ReceiveMethod = model.ReceiveMethod,
                    Remarks = model.Note,

                    CreatedBy = userName,
                    CreatedDate = DateTime.Now
                };

                _context.SalesInvoices.Add(invoice);

                await _context.SaveChangesAsync();


                // =====================================================
                // 6. CREATE SALES HEADER
                // =====================================================

                var salesHeader = new SalesHeader
                {
                    InvoiceNo = invoiceNo,
                    SalesDate = model.SalesDate,

                    CustomerId = model.CustomerId,
                    WarehouseId = model.WarehouseId,

                    SalesType = model.SalesType,
                    ReceiveMethod = model.ReceiveMethod,

                    ReceiveAmount = model.ReceiveAmount,
                    Discount = model.Discount,
                    Vat = model.Vat,
                    Ait = model.Ait,

                    Note = model.Note,

                    TotalAmount = totalAmount,
                    DueAmount = dueAmount,

                    CreatedBy = userName,
                    CreatedDate = DateTime.Now
                };

                _context.SalesHeaders.Add(salesHeader);

                await _context.SaveChangesAsync();


                // =====================================================
                // 7. FIFO STOCK DEDUCTION
                // =====================================================

                foreach (var item in model.Items)
                {
                    int remainingQty = item.Quantity;

                    var stockList =
                        await _repository.GetAvailableStockAsync(
                            model.WarehouseId,
                            item.ProductNameId,
                            item.ColorId,
                            item.Thickness,
                            item.ProductSize);

                    foreach (var stock in stockList)
                    {
                        if (remainingQty <= 0)
                            break;

                        int issueQty =
                            Math.Min(
                                remainingQty,
                                stock.AvailableQty);


                        // =============================================
                        // Find actual StockLedger entity
                        // =============================================

                        var stockEntity =
                            await _context.StockLedgers
                                .FirstOrDefaultAsync(x =>
                                    x.StockLedgerId ==
                                    stock.StockLedgerId);

                        if (stockEntity == null)
                            throw new Exception(
                                "Stock record not found.");


                        // =============================================
                        // Calculate weight proportionally
                        // =============================================

                        decimal issueWeight = 0;

                        if (stockEntity.AvailableQty > 0)
                        {
                            decimal weightPerUnit =
                                stockEntity.AvailableWeight /
                                stockEntity.AvailableQty;

                            issueWeight =
                                weightPerUnit * issueQty;
                        }


                        // =============================================
                        // Deduct stock
                        // =============================================

                        stockEntity.AvailableQty -= issueQty;

                        stockEntity.AvailableWeight -=
                            issueWeight;


                        // Prevent negative values due to rounding
                        if (stockEntity.AvailableQty < 0)
                            stockEntity.AvailableQty = 0;

                        if (stockEntity.AvailableWeight < 0)
                            stockEntity.AvailableWeight = 0;


                        // =============================================
                        // CREATE SALES DETAIL
                        // =============================================

                        var salesDetail = new SalesDetail
                        {
                            SalesInvoiceId =
                                invoice.SalesInvoiceId,

                            PurchaseDetailId =
                                stock.PurchaseDetailId,

                            ShipmentId =
                                stock.ShipmentId,

                            ProductNameId =
                                item.ProductNameId,

                            ColorId =
                                item.ColorId,

                            Thickness =
                                item.Thickness,

                            ProductSize =
                                item.ProductSize,

                            Quantity =
                                issueQty,
                            Unit = item.Unit,
                            WeightKg =
                                issueWeight,

                            UnitPrice =
                                item.UnitPrice,

                            LineTotal =
                                issueQty *
                                item.UnitPrice
                        };

                        _context.SalesDetails.Add(
                            salesDetail);


                        remainingQty -= issueQty;
                    }

                    if (remainingQty > 0)
                    {
                        throw new Exception(
                            "FIFO stock deduction failed.");
                    }
                }


                // =====================================================
                // 8. SAVE FIFO + SALES DETAILS
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 9. COMMIT TRANSACTION
                // =====================================================

                await transaction.CommitAsync();

                return invoice.SalesInvoiceId;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }


        // =============================================
        // Generate Invoice Number
        // =============================================

        private async Task<string> GenerateInvoiceNoAsync()
        {
            var lastInvoice =
                await _context.SalesInvoices
                    .OrderByDescending(x => x.SalesInvoiceId)
                    .FirstOrDefaultAsync();

            if (lastInvoice == null)
                return "INV-000001";

            int nextNumber =
                lastInvoice.SalesInvoiceId + 1;

            return $"INV-{nextNumber:D6}";
        }
    }
}