using SunwinInventory.DTOs.CustomerPayment;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class CustomerPaymentService : ICustomerPaymentService
    {
        private readonly ICustomerPaymentRepository _repository;

        public CustomerPaymentService(
            ICustomerPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CustomerPaymentDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CustomerPaymentDto?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(CustomerPaymentDto model)
        {
            await _repository.CreateAsync(model);
        }

        public async Task<int?> AddPaymentAsync(
            CustomerPaymentDto model)
        {
            return await _repository.AddPaymentAsync(model);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<CustomerPaymentDto>> GetByInvoiceAsync(
            int salesInvoiceId)
        {
            return await _repository.GetByInvoiceAsync(
                salesInvoiceId);
        }

        public async Task<List<CustomerPaymentDto>> GetByCustomerAsync(
            int customerId)
        {
            return await _repository.GetByCustomerAsync(
                customerId);
        }

        public async Task<List<CustomerPaymentDto>>
            GetCustomerInvoicesAsync(int customerId)
        {
            return await _repository.GetCustomerInvoicesAsync(
                customerId);
        }

        public async Task<CustomerPaymentDto?> GetInvoiceInfoAsync(
            int salesInvoiceId)
        {
            return await _repository.GetInvoiceInfoAsync(
                salesInvoiceId);
        }

        public async Task<List<CustomerDueDto>>
            GetCustomerDueReportAsync()
        {
            return await _repository.GetCustomerDueReportAsync();
        }
    }
}