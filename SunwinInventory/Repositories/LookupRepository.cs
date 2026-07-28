using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly SunwinInventoryContext _context;

        public LookupRepository(SunwinInventoryContext context)
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
            if (string.IsNullOrWhiteSpace(productName))
                return null;

            productName = productName.Trim()
                                     .Replace(" ", "")
                                     .ToLower();

            return await _context.ProductNames
                .Where(x => x.Name != null &&
                            x.Name.Trim()
                                  .Replace(" ", "")
                                  .ToLower() == productName)
                .Select(x => (int?)x.ProductNameId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetColorIdAsync(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
                return null;

            colorName = colorName.Trim()
                                 .Replace(" ", "")
                                 .ToLower();

            return await _context.Colors
                .Where(x => x.ColorName != null &&
                            x.ColorName.Trim()
                                       .Replace(" ", "")
                                       .ToLower() == colorName)
                .Select(x => (int?)x.ColorId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetSupplierIdAsync(string supplierName)
        {
            if (string.IsNullOrWhiteSpace(supplierName))
                return null;

            supplierName = supplierName.Trim()
                                       .Replace(" ", "")
                                       .ToLower();

            return await _context.Suppliers
                .Where(x => x.SupplierName != null &&
                            x.SupplierName.Trim()
                                          .Replace(" ", "")
                                          .ToLower() == supplierName)
                .Select(x => (int?)x.SupplierId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetWarehouseIdAsync(string warehouseName)
        {
            if (string.IsNullOrWhiteSpace(warehouseName))
                return null;

            warehouseName = warehouseName.Trim()
                                         .Replace(" ", "")
                                         .ToLower();

            return await _context.Warehouses
                .Where(x => x.WarehouseName != null &&
                            x.WarehouseName.Trim()
                                           .Replace(" ", "")
                                           .ToLower() == warehouseName)
                .Select(x => (int?)x.WarehouseId)
                .FirstOrDefaultAsync();
        }
    }
}