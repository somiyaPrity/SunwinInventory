using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.Customer;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();

            return View(list);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View(new CustomerDto
            {
                IsActive = true
            });
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _service.CreateAsync(model);

            TempData["Success"] = "Customer created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.GetByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Customer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _service.UpdateAsync(model);

            TempData["Success"] = "Customer updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Customer/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            TempData["Success"] = "Customer deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}