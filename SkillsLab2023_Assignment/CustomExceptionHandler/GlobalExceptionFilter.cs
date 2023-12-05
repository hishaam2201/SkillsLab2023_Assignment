using SkillsLab2023_Assignment.CustomExceptionHandler.AppLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.CustomExceptionHandler
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        public GlobalExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext filterContext)
        {
            _logger.LogError(filterContext.Exception);
        }
    }
}