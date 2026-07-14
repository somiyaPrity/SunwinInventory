using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SunwinInventory.DTOs.ProductName;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class ProductNameController : Controller
    {
        private readonly IProductNameService _productNameService;
        private readonly IProductCategoryService _productCategoryService;

        public ProductNameController(
            IProductNameService productNameService,
            IProductCategoryService productCategoryService)
        {
            _productNameService = productNameService;
            _productCategoryService = productCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _productNameService.GetAllAsync();

            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategories();

            return View(new ProductNameDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductNameDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(dto);
            }

            //if (await _productNameService.CodeExistsAsync(dto.ProductCode))
            //{
            //    ModelState.AddModelError("ProductCode", "Product Code already exists.");

            //    await LoadCategories();

            //    return View(dto);
            //}

            if (await _productNameService.ExistsAsync(dto.Name))
            {
                ModelState.AddModelError("Name", "Product Name already exists.");

                await LoadCategories();

                return View(dto);
            }

            await _productNameService.CreateAsync(dto);

            TempData["Success"] = "Product Name created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _productNameService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            await LoadCategories();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductNameDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();

                return View(dto);
            }

            //if (await _productNameService.CodeExistsAsync(dto.ProductCode, dto.ProductNameId))
            //{
            //    ModelState.AddModelError("ProductCode", "Product Code already exists.");

            //    await LoadCategories();

            //    return View(dto);
            //}

            if (await _productNameService.ExistsAsync(dto.Name, dto.ProductNameId))
            {
                ModelState.AddModelError("Name", "Product Name already exists.");

                await LoadCategories();

                return View(dto);
            }

            await _productNameService.UpdateAsync(dto);

            TempData["Success"] = "Product Name updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productNameService.DeleteAsync(id);

            TempData["Success"] = "Product Name deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories()
        {
            var categories = await _productCategoryService.GetAllAsync();

            ViewBag.CategoryList = categories
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                })
                .ToList();
        }
    }
}