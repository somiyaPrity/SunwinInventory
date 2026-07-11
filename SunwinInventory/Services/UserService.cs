using SunwinInventory.DTOs.User;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDesignationRepository _designationRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IDesignationRepository designationRepository,
            IWarehouseRepository warehouseRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _designationRepository = designationRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var roles = await _roleRepository.GetAllAsync();
            var designations = await _designationRepository.GetAllAsync();
            var warehouses = await _warehouseRepository.GetAllAsync();

            return users.Select(x => new UserDto
            {
                UserId = x.UserId,
                FullName = x.FullName,
                UserName = x.UserName,
                Password = x.Password,
                Email = x.Email,
                Phone = x.Phone,
                RoleId = x.RoleId,
                DesignationId = x.DesignationId,
                WarehouseId = x.WarehouseId,

                RoleName = roles.FirstOrDefault(r => r.RoleId == x.RoleId)?.RoleName,
                DesignationName = designations.FirstOrDefault(d => d.DesignationId == x.DesignationId)?.DesignationName,
                WarehouseName = warehouses.FirstOrDefault(w => w.WarehouseId == x.WarehouseId)?.WarehouseName,

                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate

            }).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var x = await _userRepository.GetByIdAsync(id);

            if (x == null)
                return null;

            return new UserDto
            {
                UserId = x.UserId,
                FullName = x.FullName,
                UserName = x.UserName,
                Password = x.Password,
                Email = x.Email,
                Phone = x.Phone,
                RoleId = x.RoleId,
                DesignationId = x.DesignationId,
                WarehouseId = x.WarehouseId,
                IsActive = x.IsActive
            };
        }

        public async Task CreateAsync(UserDto dto)
        {
            User user = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Password = dto.Password,
                Email = dto.Email,
                Phone = dto.Phone,
                RoleId = dto.RoleId,
                DesignationId = dto.DesignationId,
                WarehouseId = dto.WarehouseId,
                IsActive = dto.IsActive,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateAsync(UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
                return;

            user.FullName = dto.FullName;
            user.UserName = dto.UserName;
            user.Password = dto.Password;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.RoleId = dto.RoleId;
            user.DesignationId = dto.DesignationId;
            user.WarehouseId = dto.WarehouseId;
            user.IsActive = dto.IsActive;
            user.UpdatedBy = "Admin";
            user.UpdatedDate = DateTime.Now;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string userName, int? userId = null)
        {
            return await _userRepository.ExistsAsync(userName, userId);
        }
    }
}