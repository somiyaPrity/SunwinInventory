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

        public async Task AddAsync(ProductPurchaseDetail entity)
        {
            await _context.ProductPurchaseDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task CreateAsync(PurchaseEntryDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in dto.Items)
                {
                    ProductPurchaseDetail purchase = new ProductPurchaseDetail
                    {
                        ShipmentId = item.ShipmentId,

                        SupplierId = item.SupplierId,

                        WarehouseId = item.WarehouseId,

                        ProductNameId = item.ProductNameId,

                        ColorId = item.ColorId,

                        Thickness = item.Thickness,

                        ProductSize = item.ProductSize,
                        CostPricePerPieceTk = item.CostPricePerPieceTk,
                        InitialQuantity = item.InitialQuantity,

                        UnitWeightKg = item.UnitWeightKg,

                        TotalWeightKg = item.TotalWeightKg,

                        ExchangeRate = item.ExchangeRate,

                        UnitPriceRmb = item.UnitPriceRmb,

                        UnitPriceTk = item.UnitPriceTk,

                        ImportTaxPerKgTk = item.ImportTaxPerKgTk,

                        ImportTaxPerKgRmb = item.ImportTaxPerKgRmb,

                        CostPriceKgTk = item.CostPriceKgTk,

                        CostPriceLbsTk = item.CostPriceLbsTk,

                        CostPriceKgRmb = item.CostPriceKgRmb,

                        CostPriceLbsRmb = item.CostPriceLbsRmb,

                        TotalAmountWithoutTaxTk = item.TotalAmountWithoutTaxTk,

                        TotalAmountIncludingTaxTk = item.TotalAmountIncludingTaxTk,

                        TotalAmountWithoutTaxRmb = item.TotalAmountWithoutTaxRmb,

                        TotalAmountIncludingTaxRmb = item.TotalAmountIncludingTaxRmb,

                        Remarks = item.Remarks,

                        CreatedBy = "Admin",

                        CreatedDate = DateTime.Now
                    };

                    _context.ProductPurchaseDetails.Add(purchase);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UpdateAsync(ProductPurchaseDetail entity)
        {
            _context.ProductPurchaseDetails.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                _context.ProductPurchaseDetails.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddRangeAsync(List<ProductPurchaseDetail> entities)
        {
            await _context.ProductPurchaseDetails.AddRangeAsync(entities);

            await _context.SaveChangesAsync();
        }
    }
}