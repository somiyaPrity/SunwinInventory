using SunwinInventory.DTOs.Account;
using SunwinInventory.Models;

namespace SunwinInventory.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> LoginAsync(LoginDto dto);

        Task<bool> ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto);
    }
}