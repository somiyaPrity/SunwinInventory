using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class StockLedgerRepository : IStockLedgerRepository
    {
        private readonly SunwinInventoryContext _context;

        public StockLedgerRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(StockLedger entity)
        {
            _context.StockLedgers.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<StockLedger?> GetByPurchaseDetailIdAsync(int purchaseDetailId)
        {
            return await _context.StockLedgers
                .FirstOrDefaultAsync(x => x.PurchaseDetailId == purchaseDetailId);
        }

        public async Task UpdateAsync(StockLedger entity)
        {
            _context.StockLedgers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
