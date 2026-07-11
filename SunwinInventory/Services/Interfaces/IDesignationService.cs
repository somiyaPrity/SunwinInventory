using SunwinInventory.DTOs.Designation;

namespace SunwinInventory.Services.Interfaces
{
    public interface IDesignationService
    {
        Task<List<DesignationDto>> GetAllAsync();

        Task<DesignationDto?> GetByIdAsync(int id);

        Task CreateAsync(DesignationDto dto);

        Task UpdateAsync(DesignationDto dto);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string designationName, int? designationId = null);
    }
}