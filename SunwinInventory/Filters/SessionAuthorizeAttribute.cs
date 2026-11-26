using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SunwinInventory.Authorization;
using SunwinInventory.Services.Interfaces;

namespace SunwinInventory.Filters
{
    public class SessionAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IPermissionService _permissionService;

        public SessionAuthorizeAttribute(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var controller =
                context.RouteData.Values["controller"]?.ToString();

            var action =
                context.RouteData.Values["action"]?.ToString();

            if (controller == "Account")
            {
                await next();
                return;
            }

            var userId =
                context.HttpContext.Session.GetInt32("UserId");

            var roleId =
                context.HttpContext.Session.GetInt32("RoleId");

            if (userId == null || roleId == null)
            {
                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Account",
                        null);

                return;
            }

            var request = context.HttpContext.Request;

            bool allowed = await _permissionService.HasPermissionAsync(
                roleId.Value,
                controller,
                action,
                request.Method);

            if (!allowed)
            {
                string menuTitle = string.Empty;

                if (MenuCatalog.TryResolve(
                        controller, action, request.Method,
                        out var menu, out _))
                {
                    menuTitle = menu.Title;
                }

                if (HttpMethods.IsGet(request.Method))
                {
                    context.Result = new RedirectToActionResult(
                        "AccessDenied",
                        "Home",
                        new { menu = menuTitle });
                }
                else
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Content = string.IsNullOrEmpty(menuTitle)
                            ? "You do not have permission for this action."
                            : $"You do not have permission to perform this action on {menuTitle}."
                    };
                }

                return;
            }

            await next();
        }
    }
}
