using Framework.Enums;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SkillsLab2023_Assignment.Custom
{
    public class CustomAuthorizationAttribute : ActionFilterAttribute
    {
        public RoleEnum[] AuthorizedRoles { get; set; }
        public CustomAuthorizationAttribute(params RoleEnum[] roles)
        {
            AuthorizedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dfController = filterContext.Controller as Controller;
            if (dfController != null)
            {
                object userRoleObject = dfController.Session["UserRole"];

                if (userRoleObject != null && Enum.TryParse(userRoleObject.ToString(), out RoleEnum userRole))
                {
                    if (!AuthorizedRoles.Contains(userRole))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Common", action = "AccessDenied" }
                            ));
                    }
                }
            }
        }
    }
}