using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly SunwinInventoryContext _context;

        public DesignationRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<Designation>> GetAllAsync()
        {
            return await _context.Designations
                .OrderBy(x => x.DesignationName)
                .ToListAsync();
        }

        public async Task<Designation?> GetByIdAsync(int id)
        {
            return await _context.Designations
                .FirstOrDefaultAsync(x => x.DesignationId == id);
        }

        public async Task AddAsync(Designation designation)
        {
            await _context.Designations.AddAsync(designation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Designation designation)
        {
            _context.Designations.Update(designation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var designation = await _context.Designations.FindAsync(id);

            if (designation != null)
            {
                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string designationName, int? designationId = null)
        {
            var query = _context.Designations.AsQueryable();

            if (designationId.HasValue)
            {
                query = query.Where(x => x.DesignationId != designationId.Value);
            }

            return await query.AnyAsync(x => x.DesignationName == designationName);
        }
    }
}