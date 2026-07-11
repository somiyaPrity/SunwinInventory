using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IDesignationRepository
    {
        Task<List<Designation>> GetAllAsync();

        Task<Designation?> GetByIdAsync(int id);

        Task AddAsync(Designation designation);

        Task UpdateAsync(Designation designation);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string designationName, int? designationId = null);
    }
}