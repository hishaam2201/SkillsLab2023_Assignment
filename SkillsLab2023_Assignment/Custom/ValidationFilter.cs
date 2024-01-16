using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Custom
{
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                var model = filterContext.Controller.ViewData.ModelState;
                var error = GetErrors(model);
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Errors = error
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                };
            }
        }

        private static object GetErrors(ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var entry in modelState)
            {
                var key = entry.Key;
                var value = entry.Value;

                var errorMessages = value.Errors.Select(error => error.ErrorMessage).ToList();

                if (errorMessages.Any())
                {
                    errors.Add(key, errorMessages);
                }
            }
            return errors;
        }
    }
}