using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.ProductCategory;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        //==========================
        // Index
        //==========================
        public async Task<IActionResult> Index()
        {
            var categories = await _productCategoryService.GetAllAsync();
            return View(categories);
        }

        //==========================
        // Create GET
        //==========================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductCategoryDto
            {
                IsActive = true
            });
        }

        //==========================
        // Create POST
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool exists = await _productCategoryService.ExistsAsync(dto.CategoryName);

            if (exists)
            {
                ModelState.AddModelError("CategoryName", "Category name already exists.");
                return View(dto);
            }

            await _productCategoryService.CreateAsync(dto);

            TempData["Success"] = "Product Category created successfully.";

            return RedirectToAction(nameof(Index));
        }

        //==========================
        // Edit GET
        //==========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _productCategoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        //==========================
        // Edit POST
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool exists = await _productCategoryService.ExistsAsync(dto.CategoryName, dto.CategoryId);

            if (exists)
            {
                ModelState.AddModelError("CategoryName", "Category name already exists.");
                return View(dto);
            }

            await _productCategoryService.UpdateAsync(dto);

            TempData["Success"] = "Product Category updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        //==========================
        // Delete
        //==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productCategoryService.DeleteAsync(id);

            TempData["Success"] = "Product Category deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}