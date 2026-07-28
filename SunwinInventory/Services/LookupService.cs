using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class LookupService : ILookupService
    {
        private readonly SunwinInventoryContext _context;

        public LookupService(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetColorListAsync()
        {
            return await _context.Colors
                .Where(x => x.IsActive)
                .OrderBy(x => x.ColorName)
                .Select(x => new SelectListItem
                {
                    Value = x.ColorId.ToString(),
                    Text = x.ColorName
                })
                .ToListAsync();
        }

        public async Task<int?> GetProductIdAsync(string productName)
        {
            return await _context.ProductNames
                .Where(x => x.Name.Trim().ToLower() == productName.Trim().ToLower())
                .Select(x => (int?)x.ProductNameId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetColorIdAsync(string colorName)
        {
            return await _context.Colors
                .Where(x => x.ColorName.Trim().ToLower() == colorName.Trim().ToLower())
                .Select(x => (int?)x.ColorId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetSupplierIdAsync(string supplierName)
        {
            return await _context.Suppliers
                .Where(x => x.SupplierName.Trim().ToLower() == supplierName.Trim().ToLower())
                .Select(x => (int?)x.SupplierId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetWarehouseIdAsync(string warehouseName)
        {
            return await _context.Warehouses
                .Where(x => x.WarehouseName.Trim().ToLower() == warehouseName.Trim().ToLower())
                .Select(x => (int?)x.WarehouseId)
                .FirstOrDefaultAsync();
        }
    }
}