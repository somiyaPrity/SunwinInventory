using Microsoft.AspNetCore.Mvc.Rendering;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        Task<List<SelectListItem>> GetColorListAsync();

        Task<int?> GetProductIdAsync(string productName);

        Task<int?> GetColorIdAsync(string colorName);

        Task<int?> GetSupplierIdAsync(string supplierName);

        Task<int?> GetWarehouseIdAsync(string warehouseName);
    }
}