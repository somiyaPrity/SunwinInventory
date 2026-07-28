using Microsoft.AspNetCore.Mvc.Rendering;

namespace SunwinInventory.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<SelectListItem>> GetColorListAsync();

        Task<int?> GetProductIdAsync(string productName);

        Task<int?> GetColorIdAsync(string colorName);

        Task<int?> GetSupplierIdAsync(string supplierName);

        Task<int?> GetWarehouseIdAsync(string warehouseName);
    }
}