using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<User?> LoginAsync(string userName);

        Task UpdateAsync(User user);
        Task<User?> GetByIdAsync(int userId);

        
    }
}