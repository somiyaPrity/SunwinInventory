using Microsoft.AspNetCore.Http;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class StockLedgerService : IStockLedgerService
    {
        private readonly IStockLedgerRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StockLedgerService(
            IStockLedgerRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateAsync(StockLedger entity)
        {
            entity.CreatedDate = DateTime.Now;

            await _repository.CreateAsync(entity);
        }

        public async Task<StockLedger?> GetByPurchaseDetailIdAsync(int purchaseDetailId)
        {
            return await _repository.GetByPurchaseDetailIdAsync(purchaseDetailId);
        }

        public async Task UpdateAsync(StockLedger entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}