using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class ProductPurchaseDetailRepository : IProductPurchaseDetailRepository
    {
        private readonly SunwinInventoryContext _context;

        public ProductPurchaseDetailRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<ProductPurchaseDetail>> GetAllAsync()
        {
            return await _context.ProductPurchaseDetails
                .OrderByDescending(x => x.PurchaseDetailId)
                .ToListAsync();
        }

        public async Task<ProductPurchaseDetail?> GetByIdAsync(int id)
        {
            return await _context.ProductPurchaseDetails
                .FirstOrDefaultAsync(x => x.PurchaseDetailId == id);
        }

        public async Task CreateAsync(ProductPurchaseDetail entity)
        {
            _context.ProductPurchaseDetails.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductPurchaseDetail entity)
        {
            _context.ProductPurchaseDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ProductPurchaseDetails.FindAsync(id);

            if (entity != null)
            {
                _context.ProductPurchaseDetails.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<PurchaseReportDto>> GetPurchaseReportAsync()
        {
            var result = await
                (from p in _context.ProductPurchaseDetails

                 join s in _context.Shipments
                     on p.ShipmentId equals s.ShipmentId

                 join sup in _context.Suppliers
                     on p.SupplierId equals sup.SupplierId

                 join w in _context.Warehouses
                     on p.WarehouseId equals w.WarehouseId

                 join pn in _context.ProductNames
                     on p.ProductNameId equals pn.ProductNameId into ProductNamesGroup
                 from pn in ProductNamesGroup.DefaultIfEmpty()

                join c in _context.Colors
                     on p.ColorId equals c.ColorId into colorGroup
                 from c in colorGroup.DefaultIfEmpty()

                 orderby p.PurchaseDetailId descending

                 select new PurchaseReportDto
                 {
                     ProductPurchaseDetailId = p.PurchaseDetailId,

                     CreatedDate = p.CreatedDate,

                     ShipmentNo = s.ShipmentNo,

                     SupplierName = sup.SupplierName,

                     WarehouseName = w.WarehouseName,

                     ProductName = pn.Name,

                     ColorName = c.ColorName,

                     Thickness = p.Thickness ?? 0,

                     ProductSize = p.ProductSize,

                     InitialQuantity = p.InitialQuantity,

                     TotalWeightKg = p.TotalWeightKg ?? 0,

                     UnitPriceTk = p.UnitPriceTk ?? 0,

                     TotalAmountIncludingTaxTk = p.TotalAmountIncludingTaxTk ?? 0
                 }).ToListAsync();

            return result;
        }
    }
}