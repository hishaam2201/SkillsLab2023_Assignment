using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Hangfire.Storage.Monitoring;
using SkillsLab2023_Assignment.Custom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;

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
        [CustomAuthorization(RoleEnum.Employee)]
        public ActionResult ViewTrainings()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthorization(RoleEnum.Employee, RoleEnum.Administrator)]
        public async Task<JsonResult> GetAllUnappliedTrainings()
        {
            byte userDepartmentId = (byte) SessionManager.CurrentUser.DepartmentId;
            List<TrainingDTO> listOfTrainings = (await _trainingService.GetUnappliedTrainingsAsync(userDepartmentId)).ToList();
            return Json(new { 
                success = listOfTrainings != null && listOfTrainings.Any(),
                trainings = listOfTrainings 
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Employee)]
        public async Task<ActionResult> Details(int id)
        {
            TrainingDTO trainingDTO = await _trainingService.GetTrainingByIdAsync(id);
            return View(trainingDTO);
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<JsonResult> Delete(int trainingId)
        {
            OperationResult result = await _trainingService.DeleteTrainingAsync(trainingId);
            return Json(new
            {
                success = result?.Success ?? false,
                message = result?.GetFormattedMessages() ?? "An error occurred"
            });
        }
    }
}