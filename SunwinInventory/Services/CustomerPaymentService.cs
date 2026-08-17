using SunwinInventory.DTOs.CustomerPayment;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class CustomerPaymentService
        : ICustomerPaymentService
    {
        private readonly ICustomerPaymentRepository _repository;

        public CustomerPaymentService(
            ICustomerPaymentRepository repository)
        {
            _repository = repository;
        }

        //=====================================================
        // GET ALL
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        //=====================================================
        // GET BY ID
        //=====================================================

        public async Task<CustomerPaymentDto?>
            GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        //=====================================================
        // CREATE
        //=====================================================

        public async Task CreateAsync(
            CustomerPaymentDto model)
        {
            var result =
                await _repository.CreateAsync(model);

            if (!result)
            {
                throw new Exception(
                    "Payment could not be completed.");
            }
        }

        //=====================================================
        // DELETE
        //=====================================================

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        //=====================================================
        // BY INVOICE
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetByInvoiceAsync(int salesInvoiceId)
        {
            return await _repository
                .GetByInvoiceAsync(salesInvoiceId);
        }

        //=====================================================
        // BY CUSTOMER
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetByCustomerAsync(int customerId)
        {
            return await _repository
                .GetByCustomerAsync(customerId);
        }

        //=====================================================
        // CUSTOMER INVOICES
        //=====================================================

        public async Task<List<CustomerPaymentDto>>
            GetCustomerInvoicesAsync(int customerId)
        {
            return await _repository
                .GetCustomerInvoicesAsync(customerId);
        }

        //=====================================================
        // INVOICE INFO
        //=====================================================

        public async Task<CustomerPaymentDto?>
            GetInvoiceInfoAsync(int salesInvoiceId)
        {
            return await _repository
                .GetInvoiceInfoAsync(salesInvoiceId);
        }
        public async Task<List<CustomerDueDto>> GetCustomerDueReportAsync()
        {
            return await _repository.GetCustomerDueReportAsync();
        }
    }
}