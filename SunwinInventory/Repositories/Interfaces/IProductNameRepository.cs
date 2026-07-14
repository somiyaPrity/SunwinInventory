using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IProductNameRepository
    {
        Task<List<ProductName>> GetAllAsync();

        Task<ProductName?> GetByIdAsync(int id);

        Task AddAsync(ProductName productName);

        Task UpdateAsync(ProductName productName);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string name, int? productNameId = null);

        Task<bool> CodeExistsAsync(string productCode, int? productNameId = null);
    }
}