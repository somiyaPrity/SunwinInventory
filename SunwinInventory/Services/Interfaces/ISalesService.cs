using SunwinInventory.DTOs.Sales;

namespace SunwinInventory.Services.Interfaces
{
    public interface ISalesService
    {
        Task<int?> CreateSaleAsync(SalesEntryDto model);
    }
}