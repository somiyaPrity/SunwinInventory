using SunwinInventory.DTOs.Shipment;

namespace SunwinInventory.Services.Interfaces
{
    public interface IShipmentService
    {
        Task<List<ShipmentDto>> GetAllAsync();

        Task<ShipmentDto?> GetByIdAsync(int id);

        Task CreateAsync(ShipmentDto dto);

        Task UpdateAsync(ShipmentDto dto);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string shipmentNo, int? shipmentId = null);

        Task<string> GenerateShipmentNoAsync();
    }
}