using SunwinInventory.Models;

namespace SunwinInventory.Services.Interfaces
{
    public interface IStockLedgerService
    {
        Task CreateAsync(StockLedger entity);
    }
}