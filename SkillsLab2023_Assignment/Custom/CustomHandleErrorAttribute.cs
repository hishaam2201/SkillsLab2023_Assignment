using SkillsLab2023_Assignment.AppLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            _logger.LogError(filterContext.Exception);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error",
                TempData = filterContext.Controller.TempData
            };
        }
    }
}