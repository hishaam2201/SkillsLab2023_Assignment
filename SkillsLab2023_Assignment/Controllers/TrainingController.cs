using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    public class TrainingController : Controller
    {
        [HttpGet]
        public ActionResult Browse()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllTrainings()
        {
            // TODO: Need to Return a list of trainings in db to work with
            List<Training> list = new List<Training>();
            return Json(new { list }, JsonRequestBehavior.AllowGet);
        }
    }
}