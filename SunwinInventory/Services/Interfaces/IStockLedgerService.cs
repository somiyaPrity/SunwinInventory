using SunwinInventory.Models;

namespace SunwinInventory.Services.Interfaces
{
    public interface IStockLedgerService
    {
        Task CreateAsync(StockLedger entity);

        Task<StockLedger?> GetByPurchaseDetailIdAsync(int purchaseDetailId);

        Task UpdateAsync(StockLedger entity);
    }
}
