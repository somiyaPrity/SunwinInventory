using SunwinInventory.DTOs.Role;

namespace SunwinInventory.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> IsAdministratorAsync(int roleId);

        Task<Dictionary<string, MenuAccessDto>> GetAccessAsync(int roleId);

        Task<bool> HasPermissionAsync(
            int roleId,
            string? controller,
            string? action,
            string httpMethod);

        Task<List<MenuAccessDto>> GetRoleMenusAsync(int roleId);

        Task SaveRoleMenusAsync(int roleId, List<MenuAccessDto> menus);
    }
}
