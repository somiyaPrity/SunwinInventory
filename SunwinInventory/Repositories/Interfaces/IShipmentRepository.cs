using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IShipmentRepository
    {
        Task<List<Shipment>> GetAllAsync();

        Task<Shipment?> GetByIdAsync(int id);

        Task AddAsync(Shipment shipment);

        Task UpdateAsync(Shipment shipment);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string shipmentNo, int? shipmentId = null);

        Task<string> GenerateShipmentNoAsync();
    }
}