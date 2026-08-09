using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.Sales;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly SunwinInventoryContext _context;

        public SalesRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<StockDto>> GetAvailableStockAsync(
            int warehouseId,
            int productNameId,
            int? colorId,
            decimal? thickness,
            string? productSize)
        {
            var query =
                from stock in _context.StockLedgers

                join purchase in _context.ProductPurchaseDetails
                    on stock.PurchaseDetailId
                    equals purchase.PurchaseDetailId

                join shipment in _context.Shipments
                    on purchase.ShipmentId
                    equals shipment.ShipmentId

                where
                    stock.WarehouseId == warehouseId &&
                    stock.ProductNameId == productNameId &&
                    stock.AvailableQty > 0

                select new
                {
                    Stock = stock,
                    Purchase = purchase,
                    Shipment = shipment
                };


            // ============================================
            // COLOR
            // ============================================

            if (colorId.HasValue)
            {
                query = query.Where(x =>
                    x.Purchase.ColorId == colorId.Value);
            }
            else
            {
                query = query.Where(x =>
                    x.Purchase.ColorId == null);
            }


            // ============================================
            // THICKNESS
            // ============================================

            if (thickness.HasValue)
            {
                query = query.Where(x =>
                    x.Purchase.Thickness == thickness.Value);
            }
            else
            {
                query = query.Where(x =>
                    x.Purchase.Thickness == null);
            }


            // ============================================
            // PRODUCT SIZE
            // ============================================

            if (!string.IsNullOrWhiteSpace(productSize))
            {
                query = query.Where(x =>
                    x.Purchase.ProductSize == productSize);
            }
            else
            {
                query = query.Where(x =>
                    x.Purchase.ProductSize == null ||
                    x.Purchase.ProductSize == "");
            }


            // ============================================
            // FIFO ORDER
            // ============================================

            return await query
                .OrderBy(x => x.Stock.PurchaseDate)
                .ThenBy(x => x.Stock.StockLedgerId)

                .Select(x => new StockDto
                {
                    StockLedgerId =
                        x.Stock.StockLedgerId,

                    PurchaseDetailId =
                        x.Stock.PurchaseDetailId,

                    ShipmentId =
                        x.Shipment.ShipmentId,

                    ShipmentNo =
                        x.Shipment.ShipmentNo,

                    AvailableQty =
                        x.Stock.AvailableQty,

                    AvailableWeight =
                        x.Stock.AvailableWeight,

                    PurchaseDate =
                        x.Stock.PurchaseDate
                })

                .ToListAsync();
        }
    }
}