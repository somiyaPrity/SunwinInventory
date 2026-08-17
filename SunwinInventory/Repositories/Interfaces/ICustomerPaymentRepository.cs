using SunwinInventory.DTOs.CustomerPayment;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface ICustomerPaymentRepository
    {
        Task<List<CustomerPaymentDto>> GetAllAsync();

        Task<CustomerPaymentDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(CustomerPaymentDto model);

        Task DeleteAsync(int id);

        Task<List<CustomerPaymentDto>> GetByInvoiceAsync(
            int salesInvoiceId);

        Task<List<CustomerPaymentDto>> GetByCustomerAsync(
            int customerId);

        Task<List<CustomerPaymentDto>> GetCustomerInvoicesAsync(
            int customerId);

        Task<CustomerPaymentDto?> GetInvoiceInfoAsync(
            int salesInvoiceId);
        Task<List<CustomerDueDto>> GetCustomerDueReportAsync();
    }
}