
using BusinessLayer.Services.AdministratorService;
using BusinessLayer.Services.ManagerService;
using SkillsLab2023_Assignment.Custom;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    public class HomeController : Controller
    {
        [CustomAuthorization("Employee")]
        public ActionResult EmployeeDashboard()
        {
            return View();
        }

        [CustomAuthorization("Manager")]
        public ActionResult ManagerDashboard()
        {
            return View();
        }

        [CustomAuthorization("Administrator")]
        public ActionResult AdministratorDashboard()
        {
            return View();
        }
    }
}