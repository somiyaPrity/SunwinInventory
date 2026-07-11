using SunwinInventory.DTOs.Designation;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _repository;

        public DesignationService(IDesignationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DesignationDto>> GetAllAsync()
        {
            var designations = await _repository.GetAllAsync();

            return designations.Select(d => new DesignationDto
            {
                DesignationId = d.DesignationId,
                DesignationName = d.DesignationName,
                ReportTo = d.ReportTo,
                ReportToName = designations
                    .FirstOrDefault(x => x.DesignationId == d.ReportTo)?.DesignationName,
                IsActive = d.IsActive,
                CreatedBy = d.CreatedBy,
                CreatedDate = d.CreatedDate,
                UpdatedBy = d.UpdatedBy,
                UpdatedDate = d.UpdatedDate
            }).ToList();
        }

        public async Task<DesignationDto?> GetByIdAsync(int id)
        {
            var d = await _repository.GetByIdAsync(id);

            if (d == null)
                return null;

            return new DesignationDto
            {
                DesignationId = d.DesignationId,
                DesignationName = d.DesignationName,
                ReportTo = d.ReportTo,
                IsActive = d.IsActive,
                CreatedBy = d.CreatedBy,
                CreatedDate = d.CreatedDate,
                UpdatedBy = d.UpdatedBy,
                UpdatedDate = d.UpdatedDate
            };
        }

        public async Task CreateAsync(DesignationDto dto)
        {
            var entity = new Designation
            {
                DesignationName = dto.DesignationName,
                ReportTo = dto.ReportTo,
                IsActive = dto.IsActive,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(DesignationDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.DesignationId);

            if (entity == null)
                return;

            entity.DesignationName = dto.DesignationName;
            entity.ReportTo = dto.ReportTo;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = "Admin";
            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string designationName, int? designationId = null)
        {
            return await _repository.ExistsAsync(designationName, designationId);
        }
    }
}