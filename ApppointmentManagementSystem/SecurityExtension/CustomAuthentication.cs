using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.SecurityExtension
{
    public class CustomAuthentication : AuthorizeAttribute
    {
        private readonly RequestDelegate next;
        public CustomAuthentication(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, IHttpContextAccessor httpContextAccessor, ApplicationDbContext db, UserManager<IdentityUser> _userManager)
        {
            var controllerName = context.GetRouteValue("controller").ToString();
            var actionName = context.GetRouteValue("action").ToString();
            var session = new SessionData(httpContextAccessor);
            var userEmail = session.LogCurrentUserEmail();
            //var userRole = session.LogCurrentUserRole();
            if (userEmail == null)
            {
                context.Response.Redirect("/Account/Login");
            }
            else
            {
                string roleId = "";
                var userData = _userManager.FindByNameAsync(userEmail).Result;
                var data = userData as IdentityModel;
                 roleId = data?.RoleId;
                var hasAccess = (from m in db.RolePermissions
                                 join s in db.SubMenus on m.SubMenuId equals s.Id
                                 where m.RoleId == roleId && s.ControllerName == controllerName && s.ActionName == actionName
                                 select s).FirstOrDefault();



                if (hasAccess != null)
                {
                    await this.next.Invoke(context);
                }
                else
                {
                    context.Response.Redirect("/Account/Login");
                }

            }

        }
    }
    public static class MyCustomAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthentication>();
        }
    }
}
