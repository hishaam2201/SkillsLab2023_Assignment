
using SkillsLab2023_Assignment.Custom;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

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