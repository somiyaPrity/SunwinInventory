using SunwinInventory.DTOs.Sales;
using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface ISalesRepository
    {
        Task<List<StockDto>> GetAvailableStockAsync(
    int warehouseId,
    int productNameId,
    int? colorId,
    decimal? thickness,
    string? productSize);
    }
}