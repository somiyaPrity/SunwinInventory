using SunwinInventory.DTOs.User;

namespace SunwinInventory.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int id);

        Task CreateAsync(UserDto dto);

        Task UpdateAsync(UserDto dto);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string userName, int? userId = null);
    }
}