using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(int id);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string userName, int? userId = null);
    }
}