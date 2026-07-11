using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();

        Task<Role?> GetByIdAsync(int id);

        Task AddAsync(Role role);

        Task UpdateAsync(Role role);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string roleName, int? roleId = null);
    }
}