using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IProductPurchaseDetailRepository
    {
        Task<List<ProductPurchaseDetail>> GetAllAsync();

        Task<ProductPurchaseDetail?> GetByIdAsync(int id);

        Task AddAsync(ProductPurchaseDetail entity);
        Task CreateAsync(PurchaseEntryDto dto);
        Task UpdateAsync(ProductPurchaseDetail entity);

        Task DeleteAsync(int id);
        Task AddRangeAsync(List<ProductPurchaseDetail> entities);
    }
}