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
            var lastCustomer = await _repository.GetLastCustomerAsync();

            int nextNumber = 1;

            if (lastCustomer != null &&
                !string.IsNullOrWhiteSpace(lastCustomer.CustomerCode))
            {
                var code = lastCustomer.CustomerCode;

                if (code.StartsWith("CUS-"))
                {
                    var numberPart = code.Substring(4);

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
            }

            var customerCode = $"CUS-{nextNumber:D4}";

            var entity = new Customer
            {
                CustomerCode = customerCode,

                CustomerName = dto.CustomerName,
                ContactPerson = dto.ContactPerson,
                Mobile = dto.Mobile,
                Email = dto.Email,
                Address = dto.Address,

                IsActive = dto.IsActive,

                CreatedBy =
                    _httpContextAccessor.HttpContext?
                    .Session.GetString("UserName") ?? "System",

                CreatedDate = DateTime.Now
            };

            await _repository.CreateAsync(entity);
        }
        public async Task UpdateAsync(CustomerDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.CustomerId);

            if (entity == null)
                return;

            // CustomerCode intentionally NOT changed

            entity.CustomerName = dto.CustomerName;
            entity.ContactPerson = dto.ContactPerson;
            entity.Mobile = dto.Mobile;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.IsActive = dto.IsActive;

            entity.UpdatedBy =
                _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";

            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}