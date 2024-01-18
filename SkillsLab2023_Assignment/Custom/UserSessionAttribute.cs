using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Custom
{
    public class UserSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["CurrentUser"] == null
                || filterContext.HttpContext.Session["UserRole"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
    }
}