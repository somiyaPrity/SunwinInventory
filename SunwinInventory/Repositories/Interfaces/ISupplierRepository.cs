using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync();

        Task<Supplier?> GetByIdAsync(int id);

        Task AddAsync(Supplier supplier);

        Task UpdateAsync(Supplier supplier);

        Task DeleteAsync(int id);
        Task<string> GenerateSupplierCodeAsync();

        Task<bool> ExistsAsync(string supplierName, int? supplierId = null);

        Task<bool> CodeExistsAsync(string supplierCode, int? supplierId = null);
    }
}