using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using SkillsLab2023_Assignment.Custom;
using System.Collections.Generic;
using System.Linq;
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


        [HttpPost]
        public ActionResult Details(int id)
        {
            TrainingDTO trainingDTO = _trainingService.GetTrainingById(id);
            return View(trainingDTO);
        }
    }
}