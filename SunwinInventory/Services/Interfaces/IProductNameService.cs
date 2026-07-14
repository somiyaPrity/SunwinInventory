using SunwinInventory.DTOs.ProductName;

namespace SunwinInventory.Services.Interfaces
{
    public interface IProductNameService
    {
        Task<List<ProductNameDto>> GetAllAsync();

        Task<ProductNameDto?> GetByIdAsync(int id);

        Task CreateAsync(ProductNameDto dto);

        Task UpdateAsync(ProductNameDto dto);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string name, int? productNameId = null);

        //Task<bool> CodeExistsAsync(string productCode, int? productNameId = null);
    }
}