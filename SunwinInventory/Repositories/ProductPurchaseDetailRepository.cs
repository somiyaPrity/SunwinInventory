using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
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