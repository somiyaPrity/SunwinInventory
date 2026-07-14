using SunwinInventory.DTOs.Supplier;

namespace SunwinInventory.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync();

        Task<SupplierDto?> GetByIdAsync(int id);

        Task CreateAsync(SupplierDto dto);

        Task UpdateAsync(SupplierDto dto);

        Task DeleteAsync(int id);
        Task<string> GenerateSupplierCodeAsync();
        Task<bool> ExistsAsync(string supplierName, int? supplierId = null);

        Task<bool> CodeExistsAsync(string supplierCode, int? supplierId = null);
    }
}