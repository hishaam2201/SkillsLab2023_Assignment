using Framework.AppLogger;
using System;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Custom
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private readonly ILogger _logger;
        public CustomHandleErrorAttribute(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            var errorGuid = Guid.NewGuid();
            _logger.LogError(filterContext.Exception, errorGuid);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.Result = new ViewResult()
            {
                ViewName = "InternalServerError",
                TempData = filterContext.Controller.TempData
            };
        }
    }
}