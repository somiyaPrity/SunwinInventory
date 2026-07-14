using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class ProductNameRepository : IProductNameRepository
    {
        private readonly SunwinInventoryContext _context;

        public ProductNameRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<ProductName>> GetAllAsync()
        {
            return await _context.ProductNames
                                 .OrderBy(x => x.Name)
                                 .ToListAsync();
        }

        public async Task<ProductName?> GetByIdAsync(int id)
        {
            return await _context.ProductNames
                                 .FirstOrDefaultAsync(x => x.ProductNameId == id);
        }

        public async Task AddAsync(ProductName productName)
        {
            await _context.ProductNames.AddAsync(productName);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductName productName)
        {
            _context.ProductNames.Update(productName);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productName = await GetByIdAsync(id);

            if (productName != null)
            {
                _context.ProductNames.Remove(productName);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string name, int? productNameId = null)
        {
            return await _context.ProductNames.AnyAsync(x =>
                x.Name == name &&
                (!productNameId.HasValue || x.ProductNameId != productNameId.Value));
        }

        public async Task<bool> CodeExistsAsync(string productCode, int? productNameId = null)
        {
            return await _context.ProductNames.AnyAsync(x =>
                x.ProductCode == productCode &&
                (!productNameId.HasValue || x.ProductNameId != productNameId.Value));
        }
    }
}