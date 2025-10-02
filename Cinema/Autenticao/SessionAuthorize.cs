using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Cinema.Autenticao;

namespace Cinema.Autenticao
{
    public class SessionAuthorize : ActionFilterAttribute
    {
        public string? RoleAnyOf { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var role = http.Session.GetString(SessionKey.UserRole);
            var userId = http.Session.GetInt32(SessionKey.UserId);

            if(userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if(!string.IsNullOrWhiteSpace(RoleAnyOf))
            {
                var allowed = RoleAnyOf.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if(!allowed.Contains(role))
                {
                    context.Result = new RedirectToActionResult("AcessoNegado", "Auth", null);
                    return;
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
