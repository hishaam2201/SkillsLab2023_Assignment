using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment_ClassLibrary.DTO;
using SkillsLab2023_Assignment_ClassLibrary.Services.TrainingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    public class TrainingController : Controller
    {
        private readonly ITrainingService _trainingService;
        public TrainingController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpGet]
        public ActionResult ViewTrainings()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAllTrainings()
        {
            List<TrainingDTO> listOfTrainings = _trainingService.GetAllTrainings().ToList();
            return Json(new { success = true, trainings = listOfTrainings }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTrainingById(int id)
        {
            TrainingDTO trainingDTO = _trainingService.GetTrainingById(id);
            return Json(new { success = true, training = trainingDTO }, JsonRequestBehavior.AllowGet);
        }
    }
}