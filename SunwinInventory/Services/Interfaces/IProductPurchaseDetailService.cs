using SunwinInventory.DTOs.ProductPurchaseDetail;
using SunwinInventory.Models;

namespace SunwinInventory.Services.Interfaces
{
    public interface IProductPurchaseDetailService
    {
        Task<List<ProductPurchaseDetailDto>> GetAllAsync();

        Task<ProductPurchaseDetailDto?> GetByIdAsync(int id);

        Task CreateAsync(ProductPurchaseDetail entity);

        Task UpdateAsync(ProductPurchaseDetail entity);

        Task DeleteAsync(int id);
        Task<List<PurchaseReportDto>> GetPurchaseReportAsync();
        Task<ProductPurchaseDetail?> GetEntityByIdAsync(int id);
        Task<List<PurchaseReportDto>> GetPurchaseReportAsync(PurchaseReportFilterDto filter);
    }
}