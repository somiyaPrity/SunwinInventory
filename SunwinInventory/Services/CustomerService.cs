using Microsoft.AspNetCore.Http;
using SunwinInventory.DTOs.Customer;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(
            ICustomerRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();

            return list.Select(x => new CustomerDto
            {
                CustomerId = x.CustomerId,
                CustomerCode = x.CustomerCode,
                CustomerName = x.CustomerName,
                ContactPerson = x.ContactPerson,
                Mobile = x.Mobile,
                Email = x.Email,
                Address = x.Address,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var x = await _repository.GetByIdAsync(id);

            if (x == null)
                return null;

            return new CustomerDto
            {
                CustomerId = x.CustomerId,
                CustomerCode = x.CustomerCode,
                CustomerName = x.CustomerName,
                ContactPerson = x.ContactPerson,
                Mobile = x.Mobile,
                Email = x.Email,
                Address = x.Address,
                IsActive = x.IsActive
            };
        }

        public async Task CreateAsync(CustomerDto dto)
        {
            var entity = new Customer
            {
                CustomerCode = dto.CustomerCode,
                CustomerName = dto.CustomerName,
                ContactPerson = dto.ContactPerson,
                Mobile = dto.Mobile,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = dto.IsActive,

                CreatedBy = _httpContextAccessor.HttpContext?.Session.GetString("UserName") ?? "System",
                CreatedDate = DateTime.Now
            };

            await _repository.CreateAsync(entity);
        }

        public async Task UpdateAsync(CustomerDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.CustomerId);

            if (entity == null)
                return;

            entity.CustomerCode = dto.CustomerCode;
            entity.CustomerName = dto.CustomerName;
            entity.ContactPerson = dto.ContactPerson;
            entity.Mobile = dto.Mobile;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.IsActive = dto.IsActive;

            entity.UpdatedBy = _httpContextAccessor.HttpContext?.Session.GetString("UserName") ?? "System";
            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}