using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories.Implementations
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly SunwinInventoryContext _context;

        public ProductCategoryRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories
                                 .OrderBy(x => x.CategoryName)
                                 .ToListAsync();
        }

        public async Task<ProductCategory?> GetByIdAsync(int id)
        {
            return await _context.ProductCategories
                                 .FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task AddAsync(ProductCategory category)
        {
            await _context.ProductCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductCategory category)
        {
            _context.ProductCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);

            if (category != null)
            {
                _context.ProductCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string categoryName, int? categoryId = null)
        {
            var query = _context.ProductCategories.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId != categoryId.Value);
            }

            return await query.AnyAsync(x => x.CategoryName == categoryName);
        }
    }
}