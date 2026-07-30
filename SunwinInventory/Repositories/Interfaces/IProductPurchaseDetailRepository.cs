using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IProductPurchaseDetailRepository
    {
        Task<List<ProductPurchaseDetail>> GetAllAsync();

        Task<ProductPurchaseDetail?> GetByIdAsync(int id);

        Task CreateAsync(ProductPurchaseDetail entity);

        Task UpdateAsync(ProductPurchaseDetail entity);

        Task DeleteAsync(int id);
        Task<List<PurchaseReportDto>> GetPurchaseReportAsync();
     
    }
}