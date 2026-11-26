using SunwinInventory.Authorization;
using SunwinInventory.DTOs.Role;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleRepository _roleRepository;

        // Scoped service: one instance per request, so this is a per-request cache.
        private readonly Dictionary<int, Dictionary<string, MenuAccessDto>> _cache =
            new();

        private readonly Dictionary<int, bool> _administratorCache = new();

        public PermissionService(
            IPermissionRepository permissionRepository,
            IRoleRepository roleRepository)
        {
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> IsAdministratorAsync(int roleId)
        {
            if (_administratorCache.TryGetValue(roleId, out bool cached))
                return cached;

            var role = await _roleRepository.GetByIdAsync(roleId);

            bool isAdmin = role != null && string.Equals(
                role.RoleName,
                MenuCatalog.AdministratorRole,
                StringComparison.OrdinalIgnoreCase);

            _administratorCache[roleId] = isAdmin;

            return isAdmin;
        }

        public async Task<Dictionary<string, MenuAccessDto>> GetAccessAsync(
            int roleId)
        {
            if (_cache.TryGetValue(roleId, out var cached))
                return cached;

            var rows = await _permissionRepository.GetByRoleIdAsync(roleId);

            var access = rows.ToDictionary(
                x => x.MenuKey,
                x => new MenuAccessDto
                {
                    MenuKey = x.MenuKey,
                    CanView = x.CanView,
                    CanCreate = x.CanCreate,
                    CanEdit = x.CanEdit,
                    CanDelete = x.CanDelete
                },
                StringComparer.OrdinalIgnoreCase);

            _cache[roleId] = access;

            return access;
        }

        public async Task<bool> HasPermissionAsync(
            int roleId,
            string? controller,
            string? action,
            string httpMethod)
        {
            if (!MenuCatalog.TryResolve(
                    controller, action, httpMethod, out var menu, out var required))
            {
                return true;
            }

            if (await IsAdministratorAsync(roleId))
                return true;

            var access = await GetAccessAsync(roleId);

            if (!access.TryGetValue(menu.Key, out var permission))
                return false;

            return required switch
            {
                PermissionAction.Create => permission.CanCreate,
                PermissionAction.Edit => permission.CanEdit,
                PermissionAction.Delete => permission.CanDelete,
                _ => permission.CanView
            };
        }

        public async Task<List<MenuAccessDto>> GetRoleMenusAsync(int roleId)
        {
            var access = await GetAccessAsync(roleId);

            bool isAdministrator = await IsAdministratorAsync(roleId);

            return MenuCatalog.Menus
                .Select(menu =>
                {
                    access.TryGetValue(menu.Key, out var saved);

                    return new MenuAccessDto
                    {
                        MenuKey = menu.Key,
                        Title = menu.Title,
                        Group = menu.Group,
                        Icon = menu.Icon,
                        CanView = isAdministrator || (saved?.CanView ?? false),
                        CanCreate = isAdministrator || (saved?.CanCreate ?? false),
                        CanEdit = isAdministrator || (saved?.CanEdit ?? false),
                        CanDelete = isAdministrator || (saved?.CanDelete ?? false)
                    };
                })
                .ToList();
        }

        public async Task SaveRoleMenusAsync(
            int roleId,
            List<MenuAccessDto> menus)
        {
            if (await IsAdministratorAsync(roleId))
                return;

            var rows = menus
                .Where(x => MenuCatalog.Find(x.MenuKey) != null)
                .Where(x => x.CanView || x.CanCreate || x.CanEdit || x.CanDelete)
                .Select(x => new RoleMenuPermission
                {
                    RoleId = roleId,
                    MenuKey = x.MenuKey,
                    CanView = x.CanView || x.CanCreate || x.CanEdit || x.CanDelete,
                    CanCreate = x.CanCreate,
                    CanEdit = x.CanEdit,
                    CanDelete = x.CanDelete
                })
                .ToList();

            await _permissionRepository.ReplaceForRoleAsync(roleId, rows);

            _cache.Remove(roleId);
        }
    }
}
