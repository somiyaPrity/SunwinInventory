using Microsoft.AspNetCore.Mvc.Rendering;

namespace SunwinInventory.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<SelectListItem>> GetColorListAsync();
    }
}