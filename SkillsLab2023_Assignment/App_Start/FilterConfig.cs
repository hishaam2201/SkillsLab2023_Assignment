using SkillsLab2023_Assignment.CustomExceptionHandler;
using SkillsLab2023_Assignment.CustomExceptionHandler.AppLogger;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            var logger = DependencyResolver.Current.GetService<ILogger>();
            filters.Add(new GlobalExceptionFilter(logger));
        }
    }
}
