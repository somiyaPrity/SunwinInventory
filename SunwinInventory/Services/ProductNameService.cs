using Microsoft.AspNetCore.Http;
using SunwinInventory.DTOs.ProductCategory;
using SunwinInventory.DTOs.ProductName;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Services
{
    public class ProductNameService : IProductNameService
    {
        private readonly IProductNameRepository _repository;
        private readonly IProductCategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductNameService(
            IProductNameRepository repository,
            IProductCategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ProductNameDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var result = products.Select(x => new ProductNameDto
            {
                ProductNameId = x.ProductNameId,
                ProductCode = x.ProductCode,
                Name = x.Name,
                CategoryId = x.CategoryId,
                CategoryName = categories
                                .FirstOrDefault(c => c.CategoryId == x.CategoryId)
                                ?.CategoryName,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate
            }).ToList();

            return result;
        }

        public async Task<ProductNameDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return null;

            return new ProductNameDto
            {
                ProductNameId = product.ProductNameId,
                ProductCode = product.ProductCode,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Description = product.Description,
                IsActive = product.IsActive
            };
        }

        public async Task CreateAsync(ProductNameDto dto)
        {
            var entity = new ProductName
            {
                ProductCode = dto.ProductCode,
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = _httpContextAccessor.HttpContext?
                    .Session.GetString("UserName") ?? "System",
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(ProductNameDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.ProductNameId);

            if (entity == null)
                return;

            entity.ProductCode = dto.ProductCode;
            entity.Name = dto.Name;
            entity.CategoryId = dto.CategoryId;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = _httpContextAccessor.HttpContext?
                .Session.GetString("UserName") ?? "System";
            entity.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string name, int? productNameId = null)
        {
            return await _repository.ExistsAsync(name, productNameId);
        }
        //public async Task<bool> CodeExistsAsync(string? productCode, int? productNameId = null)
        //{
        //    if (string.IsNullOrWhiteSpace(productCode))
        //        return false;

        //    return await _repository.CodeExistsAsync(productCode, productNameId);
        //}
        //public async Task<bool> CodeExistsAsync(string productCode, int? productNameId = null)
        //{
        //    return await _repository.CodeExistsAsync(productCode, productNameId);
        //}
    }
}