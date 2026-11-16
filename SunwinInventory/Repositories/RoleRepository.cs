using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SunwinInventoryContext _context;

        public RoleRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .OrderBy(x => x.RoleName)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == id);
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role != null)
            {
                var permissions = await _context.RoleMenuPermissions
                    .Where(x => x.RoleId == id)
                    .ToListAsync();

                _context.RoleMenuPermissions.RemoveRange(permissions);
                _context.Roles.Remove(role);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string roleName, int? roleId = null)
        {
            var query = _context.Roles.AsQueryable();

            if (roleId.HasValue)
            {
                query = query.Where(x => x.RoleId != roleId.Value);
            }

            return await query.AnyAsync(x => x.RoleName == roleName);
        }
    }
}