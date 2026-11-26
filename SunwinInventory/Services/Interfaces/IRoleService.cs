using SunwinInventory.DTOs.Role;

namespace SunwinInventory.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllAsync();

        Task<RoleDto?> GetByIdAsync(int id);

        Task<int> CreateAsync(RoleDto dto);

        Task UpdateAsync(RoleDto dto);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string roleName, int? roleId = null);
    }
}