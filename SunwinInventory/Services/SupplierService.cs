using Microsoft.AspNetCore.Http;
using SunwinInventory.DTOs.Supplier;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierService(
            ISupplierRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _repository.GetAllAsync();

            return suppliers.Select(x => new SupplierDto
            {
                SupplierId = x.SupplierId,
                SupplierCode = x.SupplierCode,
                SupplierName = x.SupplierName,
                ContactPerson = x.ContactPerson,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                Country = x.Country,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            }).ToList();
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);

            if (supplier == null)
                return null;

            return new SupplierDto
            {
                SupplierId = supplier.SupplierId,
                SupplierCode = supplier.SupplierCode,
                SupplierName = supplier.SupplierName,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                Country = supplier.Country,
                IsActive = supplier.IsActive
            };
        }
        public async Task<string> GenerateSupplierCodeAsync()
        {
            return await _repository.GenerateSupplierCodeAsync();
        }
        public async Task CreateAsync(SupplierDto dto)
        {
            var entity = new Supplier
            {
                SupplierCode = dto.SupplierCode,
                SupplierName = dto.SupplierName,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                Country = dto.Country,
                IsActive = dto.IsActive,
                CreatedBy = _httpContextAccessor.HttpContext?
                    .Session.GetString("UserName") ?? "System",
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(SupplierDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.SupplierId);

            if (entity == null)
                return;

            entity.SupplierCode = dto.SupplierCode;
            entity.SupplierName = dto.SupplierName;
            entity.ContactPerson = dto.ContactPerson;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.Country = dto.Country;
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

        public async Task<bool> ExistsAsync(string supplierName, int? supplierId = null)
        {
            return await _repository.ExistsAsync(supplierName, supplierId);
        }

        public async Task<bool> CodeExistsAsync(string supplierCode, int? supplierId = null)
        {
            return await _repository.CodeExistsAsync(supplierCode, supplierId);
        }
    }
}