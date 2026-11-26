using Microsoft.AspNetCore.Http;
using SunwinInventory.DTOs.ProductCategory;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services.Implementations
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductCategoryService(
            IProductCategoryRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CurrentUserName =>
            _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";

        public async Task<List<ProductCategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories.Select(x => new ProductCategoryDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            }).ToList();
        }

        public async Task<ProductCategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
                return null;

            return new ProductCategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedBy = category.CreatedBy,
                CreatedDate = category.CreatedDate,
                UpdatedBy = category.UpdatedBy,
                UpdatedDate = category.UpdatedDate
            };
        }

        public async Task CreateAsync(ProductCategoryDto dto)
        {
            var entity = new ProductCategory
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = CurrentUserName,
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(ProductCategoryDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.CategoryId);

            if (entity == null)
                return;

            entity.CategoryName = dto.CategoryName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = CurrentUserName;
            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string categoryName, int? categoryId = null)
        {
            return await _repository.ExistsAsync(categoryName, categoryId);
        }
    }
}