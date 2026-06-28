using SunwinInventory.DTOs.ProductCategory;

namespace SunwinInventory.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetAllAsync();

        Task<ProductCategoryDto?> GetByIdAsync(int id);

        Task CreateAsync(ProductCategoryDto dto);

        Task UpdateAsync(ProductCategoryDto dto);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string categoryName, int? categoryId = null);
    }
}