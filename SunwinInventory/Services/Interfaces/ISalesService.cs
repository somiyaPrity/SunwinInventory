using SunwinInventory.DTOs.Sales;

namespace SunwinInventory.Services.Interfaces
{
    public interface ISalesService
    {
        Task<bool> CreateSaleAsync(SalesEntryDto model);
    }
}