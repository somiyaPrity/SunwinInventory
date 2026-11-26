using Microsoft.AspNetCore.Mvc;
using SunwinInventory.DTOs.Account;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        //------------------------------------------------

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //------------------------------------------------

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return View(dto);

        //    var user = await _service.LoginAsync(dto);

        //    if (user == null)
        //    {
        //        ViewBag.Error = "Invalid Username or Password.";

        //        return View(dto);
        //    }

        //    HttpContext.Session.SetInt32("UserId", user.UserId);

        //    HttpContext.Session.SetString("UserName", user.UserName);

        //    HttpContext.Session.SetString("FullName", user.FullName);

        //    HttpContext.Session.SetInt32("RoleId", user.RoleId);

        //    return RedirectToAction(
        //        "Index",
        //        "Home");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _service.LoginAsync(dto);

            if (user == null)
            {
                TempData["Error"] = "Invalid Username or Password.";

                return View(dto);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);

            HttpContext.Session.SetString("UserName", user.UserName);

            HttpContext.Session.SetString("FullName", user.FullName);

            HttpContext.Session.SetInt32("RoleId", user.RoleId);

            if (user.MustChangePassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            TempData["Success"] = "Welcome " + user.FullName;

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction(nameof(Login));

            bool result = await _service.ChangePasswordAsync(userId.Value, dto);

            if (!result)
            {
                TempData["Error"] = "Current password is incorrect.";

                return View(dto);
            }

            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction(nameof(Login));
        }
        //------------------------------------------------

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Login));
        }
    }
}