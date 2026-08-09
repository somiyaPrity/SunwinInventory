using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task CreateAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(int id);
        Task<Customer?> GetLastCustomerAsync();
    }
}