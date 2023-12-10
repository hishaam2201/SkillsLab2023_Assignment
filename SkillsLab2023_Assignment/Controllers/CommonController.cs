using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}