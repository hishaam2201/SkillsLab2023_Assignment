using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    public class CommonController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}