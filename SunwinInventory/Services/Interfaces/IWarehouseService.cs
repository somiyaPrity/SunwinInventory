using SunwinInventory.DTOs.Warehouse;

namespace SunwinInventory.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<List<WarehouseDto>> GetAllAsync();

        Task<WarehouseDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(CreateWarehouseDto dto);

        Task<bool> UpdateAsync(WarehouseDto dto);

        Task<bool> DeleteAsync(int id);
    }
}