using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SkillsLab2023_Assignment.Custom
{
    public class CustomAuthorizationAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; }
        public string[] AuthorizedRoles { get; set; }
        public CustomAuthorizationAttribute(string roles)
        {
            Roles = roles;
            AuthorizedRoles = Roles.Split(',');
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dfController = (Controller) filterContext.Controller;
            if (dfController != null && dfController.Session["UserRole"] != null)
            {
                var userRole = (string) dfController.Session["UserRole"];
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