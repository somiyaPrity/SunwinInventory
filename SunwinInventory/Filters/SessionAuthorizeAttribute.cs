using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SunwinInventory.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller =
                context.RouteData.Values["controller"]?.ToString();

            if (controller == "Account")
            {
                base.OnActionExecuting(context);
                return;
            }

            var userId =
                context.HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Account",
                        null);

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}