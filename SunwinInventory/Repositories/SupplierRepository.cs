using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly SunwinInventoryContext _context;

        public SupplierRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                                 .OrderBy(x => x.SupplierName)
                                 .ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                                 .FirstOrDefaultAsync(x => x.SupplierId == id);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await GetByIdAsync(id);

            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<string> GenerateSupplierCodeAsync()
        {
            var lastSupplier = await _context.Suppliers
                .OrderByDescending(x => x.SupplierId)
                .FirstOrDefaultAsync();

            if (lastSupplier == null)
                return "SUP0001";

            int number = 1;

            if (!string.IsNullOrEmpty(lastSupplier.SupplierCode))
            {
                var numericPart = lastSupplier.SupplierCode.Replace("SUP", "");

                int.TryParse(numericPart, out number);

                number++;
            }

            return $"SUP{number:D4}";
        }
        public async Task<bool> ExistsAsync(string supplierName, int? supplierId = null)
        {
            return await _context.Suppliers.AnyAsync(x =>
                x.SupplierName == supplierName &&
                (!supplierId.HasValue || x.SupplierId != supplierId.Value));
        }

        public async Task<bool> CodeExistsAsync(string supplierCode, int? supplierId = null)
        {
            return await _context.Suppliers.AnyAsync(x =>
                x.SupplierCode == supplierCode &&
                (!supplierId.HasValue || x.SupplierId != supplierId.Value));
        }
    }
}