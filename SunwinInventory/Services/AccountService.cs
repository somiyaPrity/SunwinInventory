using SunwinInventory.DTOs.Account;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IRoleRepository _roleRepository;

        public AccountService(
            IAccountRepository repository,
            IRoleRepository roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        public async Task<User?> LoginAsync(LoginDto dto)
        {
            var user = await _repository.LoginAsync(dto.UserName);

            if (user == null)
                return null;

            if (!user.IsActive)
                return null;

            if (user.IsLocked)
                return null;

            var role = await _roleRepository.GetByIdAsync(user.RoleId);

            if (role == null || !role.IsActive)
                return null;

            if (user.Password != dto.Password)
                return null;

            user.LastLogin = DateTime.Now;
            user.FailedLoginCount = 0;

            await _repository.UpdateAsync(user);

            return user;
        }

      
        public async Task<bool> ChangePasswordAsync(
    int userId,
    ChangePasswordDto dto)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (user.Password != dto.CurrentPassword)
                return false;

            user.Password = dto.NewPassword;
            user.MustChangePassword = false;
            user.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(user);

            return true;
        }
    }
}