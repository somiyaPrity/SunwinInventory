using Microsoft.AspNetCore.Http;
using SunwinInventory.DTOs.Shipment;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShipmentService(
            IShipmentRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ShipmentDto>> GetAllAsync()
        {
            var shipments = await _repository.GetAllAsync();

            return shipments.Select(x => new ShipmentDto
            {
                ShipmentId = x.ShipmentId,
                ShipmentNo = x.ShipmentNo,
                ShipmentDate = x.ShipmentDate.ToDateTime(TimeOnly.MinValue),
                ShipmentType = x.ShipmentType,
                VesselName = x.VesselName,
                ContainerNo = x.ContainerNo,
                BLNo = x.Blno,
                ArrivalDate = x.ArrivalDate?.ToDateTime(TimeOnly.MinValue),
                Remarks = x.Remarks,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            }).ToList();
        }

        public async Task<ShipmentDto?> GetByIdAsync(int id)
        {
            var shipment = await _repository.GetByIdAsync(id);

            if (shipment == null)
                return null;

            return new ShipmentDto
            {
                ShipmentId = shipment.ShipmentId,
                ShipmentNo = shipment.ShipmentNo,
                ShipmentDate = shipment.ShipmentDate.ToDateTime(TimeOnly.MinValue),
                ShipmentType = shipment.ShipmentType,
                VesselName = shipment.VesselName,
                ContainerNo = shipment.ContainerNo,
                BLNo = shipment.Blno,
                ArrivalDate = shipment.ArrivalDate?.ToDateTime(TimeOnly.MinValue),
                Remarks = shipment.Remarks,
                IsActive = shipment.IsActive
            };
        }

        public async Task CreateAsync(ShipmentDto dto)
        {
            var entity = new Shipment
            {
                ShipmentNo = await _repository.GenerateShipmentNoAsync(),
                ShipmentDate = DateOnly.FromDateTime(dto.ShipmentDate),
                ShipmentType = dto.ShipmentType,
                VesselName = dto.VesselName,
                ContainerNo = dto.ContainerNo,
                Blno = dto.BLNo,
                ArrivalDate = dto.ArrivalDate == null
                    ? null
                    : DateOnly.FromDateTime(dto.ArrivalDate.Value),
                Remarks = dto.Remarks,
                IsActive = dto.IsActive,
                CreatedBy = _httpContextAccessor.HttpContext?
                    .Session.GetString("UserName") ?? "System",
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(ShipmentDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.ShipmentId);

            if (entity == null)
                return;

            entity.ShipmentDate = DateOnly.FromDateTime(dto.ShipmentDate);
            entity.ShipmentType = dto.ShipmentType;
            entity.VesselName = dto.VesselName;
            entity.ContainerNo = dto.ContainerNo;
            entity.Blno = dto.BLNo;
            entity.ArrivalDate = dto.ArrivalDate == null
                ? null
                : DateOnly.FromDateTime(dto.ArrivalDate.Value);
            entity.Remarks = dto.Remarks;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";
            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string shipmentNo, int? shipmentId = null)
        {
            return await _repository.ExistsAsync(shipmentNo, shipmentId);
        }

        public async Task<string> GenerateShipmentNoAsync()
        {
            return await _repository.GenerateShipmentNoAsync();
        }
    }
}