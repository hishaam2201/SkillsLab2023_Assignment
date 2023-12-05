using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Services.TrainingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    public class TrainingController : Controller
    {
        private readonly ITrainingService _trainingService;
        public TrainingController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpGet]
        public ActionResult Browse()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllTrainings()
        {
            List<Training> list = _trainingService.GetAllTrainings().ToList();
            return Json(new { trainings = list }, JsonRequestBehavior.AllowGet);
        }
    }
}