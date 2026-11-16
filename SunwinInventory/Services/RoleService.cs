using SunwinInventory.DTOs.Role;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            var roles = await _repository.GetAllAsync();

            return roles.Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Description = r.Description,
                IsActive = r.IsActive,
                CreatedBy = r.CreatedBy,
                CreatedDate = r.CreatedDate,
                UpdatedBy = r.UpdatedBy,
                UpdatedDate = r.UpdatedDate
            }).ToList();
        }

        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            var role = await _repository.GetByIdAsync(id);

            if (role == null)
                return null;

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedBy = role.CreatedBy,
                CreatedDate = role.CreatedDate,
                UpdatedBy = role.UpdatedBy,
                UpdatedDate = role.UpdatedDate
            };
        }

        public async Task<int> CreateAsync(RoleDto dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(role);

            return role.RoleId;
        }

        public async Task UpdateAsync(RoleDto dto)
        {
            var role = await _repository.GetByIdAsync(dto.RoleId);

            if (role == null)
                return;

            role.RoleName = dto.RoleName;
            role.Description = dto.Description;
            role.IsActive = dto.IsActive;
            role.UpdatedBy = "Admin";
            role.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(role);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string roleName, int? roleId = null)
        {
            return await _repository.ExistsAsync(roleName, roleId);
        }
    }
}