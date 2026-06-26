using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly SunwinInventoryContext _context;

        public WarehouseRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<Warehouse>> GetAllAsync()
        {
            return await _context.Warehouses
                                 .OrderBy(w => w.WarehouseName)
                                 .ToListAsync();
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            return await _context.Warehouses
                                 .FirstOrDefaultAsync(w => w.WarehouseId == id);
        }

        public async Task<bool> ExistsByNameAsync(string warehouseName)
        {
            return await _context.Warehouses
                                 .AnyAsync(w => w.WarehouseName == warehouseName);
        }

        public async Task AddAsync(Warehouse warehouse)
        {
            await _context.Warehouses.AddAsync(warehouse);
        }

        public Task UpdateAsync(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var warehouse = await GetByIdAsync(id);

            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}