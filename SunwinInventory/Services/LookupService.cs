using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
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
    }
}