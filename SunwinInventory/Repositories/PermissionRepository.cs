using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SunwinInventoryContext _context;

        public PermissionRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<RoleMenuPermission>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
        }

        public async Task ReplaceForRoleAsync(
            int roleId,
            List<RoleMenuPermission> permissions)
        {
            var existing = await _context.RoleMenuPermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();

            _context.RoleMenuPermissions.RemoveRange(existing);
            _context.RoleMenuPermissions.AddRange(permissions);

            await _context.SaveChangesAsync();
        }
    }
}
