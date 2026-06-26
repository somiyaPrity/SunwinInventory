using SunwinInventory.DTOs.Warehouse;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repository;

        public WarehouseService(IWarehouseRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _repository.GetAllAsync();

            return warehouses.Select(w => new WarehouseDto
            {
                WarehouseId = w.WarehouseId,
                WarehouseName = w.WarehouseName,
                Address = w.Address,
                Phone = w.Phone,
                Email = w.Email,
                ContactPerson = w.ContactPerson,
                IsActive = w.IsActive ?? false
            }).ToList();
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var warehouse = await _repository.GetByIdAsync(id);

            if (warehouse == null)
                return null;

            return new WarehouseDto
            {
                WarehouseId = warehouse.WarehouseId,
                WarehouseName = warehouse.WarehouseName,
                Address = warehouse.Address,
                Phone = warehouse.Phone,
                Email = warehouse.Email,
                ContactPerson = warehouse.ContactPerson,
                IsActive = warehouse.IsActive ?? false
            };
        }

        public async Task<bool> CreateAsync(CreateWarehouseDto dto)
        {
            // Duplicate warehouse name check
            var warehouses = await _repository.GetAllAsync();

            if (warehouses.Any(x => x.WarehouseName.ToLower() == dto.WarehouseName.ToLower()))
            {
                return false;
            }

            var warehouse = new Warehouse
            {
                WarehouseName = dto.WarehouseName,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                ContactPerson = dto.ContactPerson,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(warehouse);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(WarehouseDto dto)
        {
            var warehouse = await _repository.GetByIdAsync(dto.WarehouseId);

            if (warehouse == null)
                return false;

            warehouse.WarehouseName = dto.WarehouseName;
            warehouse.Address = dto.Address;
            warehouse.Phone = dto.Phone;
            warehouse.Email = dto.Email;
            warehouse.ContactPerson = dto.ContactPerson;
            warehouse.IsActive = dto.IsActive;
            warehouse.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(warehouse);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveAsync();

            return true;
        }
    }
}