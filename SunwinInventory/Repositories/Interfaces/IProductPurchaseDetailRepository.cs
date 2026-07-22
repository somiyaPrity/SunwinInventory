using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IProductPurchaseDetailRepository
    {
        Task<List<ProductPurchaseDetail>> GetAllAsync();

        Task<ProductPurchaseDetail?> GetByIdAsync(int id);

        Task AddAsync(ProductPurchaseDetail entity);

        Task UpdateAsync(ProductPurchaseDetail entity);

        Task DeleteAsync(int id);
        Task AddRangeAsync(List<ProductPurchaseDetail> entities);
    }
}