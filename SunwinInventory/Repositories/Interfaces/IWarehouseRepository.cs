using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<List<Warehouse>> GetAllAsync();

        Task<Warehouse?> GetByIdAsync(int id);

        Task<bool> ExistsByNameAsync(string warehouseName);

        Task AddAsync(Warehouse warehouse);

        Task UpdateAsync(Warehouse warehouse);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}