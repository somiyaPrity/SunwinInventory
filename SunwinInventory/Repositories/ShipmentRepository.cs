using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly SunwinInventoryContext _context;

        public ShipmentRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<Shipment>> GetAllAsync()
        {
            return await _context.Shipments
                .OrderByDescending(x => x.ShipmentId)
                .ToListAsync();
        }

        public async Task<Shipment?> GetByIdAsync(int id)
        {
            return await _context.Shipments
                .FirstOrDefaultAsync(x => x.ShipmentId == id);
        }

        public async Task AddAsync(Shipment shipment)
        {
            await _context.Shipments.AddAsync(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var shipment = await GetByIdAsync(id);

            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string shipmentNo, int? shipmentId = null)
        {
            return await _context.Shipments.AnyAsync(x =>
                x.ShipmentNo == shipmentNo &&
                (!shipmentId.HasValue || x.ShipmentId != shipmentId.Value));
        }

        public async Task<string> GenerateShipmentNoAsync()
        {
            var lastShipment = await _context.Shipments
                .OrderByDescending(x => x.ShipmentId)
                .FirstOrDefaultAsync();

            if (lastShipment == null)
                return "Shipment-1";

            int nextNo = 1;

            if (!string.IsNullOrWhiteSpace(lastShipment.ShipmentNo))
            {
                var numberPart = lastShipment.ShipmentNo.Replace("Shipment-", "");

                if (int.TryParse(numberPart, out int lastNo))
                {
                    nextNo = lastNo + 1;
                }
            }

            return $"Shipment-{nextNo}";
        }
    }
}