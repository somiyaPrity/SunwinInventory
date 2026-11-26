using SunwinInventory.Models;

namespace SunwinInventory.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<RoleMenuPermission>> GetByRoleIdAsync(int roleId);

        Task ReplaceForRoleAsync(
            int roleId,
            List<RoleMenuPermission> permissions);
    }
}
