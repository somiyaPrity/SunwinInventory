using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IProductCategoryRepository
    {
        Task<List<ProductCategory>> GetAllAsync();

        Task<ProductCategory?> GetByIdAsync(int id);

        Task AddAsync(ProductCategory category);

        Task UpdateAsync(ProductCategory category);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string categoryName, int? categoryId = null);
    }
}