using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using SkillsLab2023_Assignment.Custom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    [CustomAuthorization("Employee")]
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

        [HttpGet]
        public async Task<JsonResult> GetAllTrainings()
        {
            byte userDepartmentId = (byte) SessionManager.CurrentUser.DepartmentId;
            List<TrainingDTO> listOfTrainings = (await _trainingService.GetAllTrainingsAsync(userDepartmentId)).ToList();
            return Json(new { 
                success = listOfTrainings != null && listOfTrainings.Any(),
                trainings = listOfTrainings 
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> Details(int id)
        {
            TrainingDTO trainingDTO = await _trainingService.GetTrainingByIdAsync(id);
            return View(trainingDTO);
        }
    }
}