using SunwinInventory.DTOs.ProductPurchaseDetail;

namespace SunwinInventory.Services.Interfaces
{
    public interface IProductPurchaseDetailService
    {
        Task<List<ProductPurchaseDetailDto>> GetAllAsync();

        Task<ProductPurchaseDetailDto?> GetByIdAsync(int id);

        Task CreateAsync(PurchaseEntryDto dto);

        Task UpdateAsync(ProductPurchaseDetailDto dto);

        Task DeleteAsync(int id);
    }
}