using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IStockLedgerRepository
    {
        Task CreateAsync(StockLedger entity);

        Task<StockLedger?> GetByPurchaseDetailIdAsync(int purchaseDetailId);

        Task UpdateAsync(StockLedger entity);
    }
}
