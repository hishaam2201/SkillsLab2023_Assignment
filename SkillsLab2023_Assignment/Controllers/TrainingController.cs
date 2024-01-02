using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using SkillsLab2023_Assignment.Custom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> TrainingDetails(int trainingId)
        {
            TrainingDTO trainingDTO = await _trainingService.GetTrainingByIdAsync(trainingId);
            return View(trainingDTO);
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<JsonResult> GetTrainingById(int trainingId)
        {
            TrainingDTO trainingDTO = await _trainingService.GetTrainingByIdAsync(trainingId);
            return trainingDTO != null
                    ? Json(new { success = true, training = trainingDTO })
                    : Json(new { success = false, message = "Training not found." });
        }

        [HttpGet]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<JsonResult> GetAllPreRequisites()
        {
            OperationResult result = await _trainingService.GetAllPreRequisites();
            return Json(new
            {
                success = result?.Success ?? false,
                preRequisites = result?.ListOfData.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[CustomAuthorization(RoleEnum.Administrator)]
        //public async Task<JsonResult> Update(int trainingId)
        //{
        //    OperationResult result = await _trainingService.
        //}

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