using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment.AppLogger;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            var logger = DependencyResolver.Current.GetService<ILogger>();
            filters.Add(new CustomHandleErrorAttribute(logger));
        }
    }
}
