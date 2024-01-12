using BusinessLayer.Services.TrainingService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment.Mapper;
using SkillsLab2023_Assignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    [ValidationFilter]
    public class TrainingController : Controller
    {
        private readonly ITrainingService _trainingService;
        public TrainingController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpGet, CustomAuthorization(RoleEnum.Employee)]
        public async Task<ActionResult> ViewTrainings()
        {
            byte userDepartmentId = (byte)SessionManager.CurrentUser.DepartmentId;
            short userId = SessionManager.CurrentUser.Id;
            List<TrainingDTO> listOfTrainings = (await _trainingService.GetUnappliedTrainingsAsync(userDepartmentId, userId)).ToList();
            return View(listOfTrainings);
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
            OperationResult result = await _trainingService.GetAllPreRequisitesAsync();
            return Json(new
            {
                success = result?.Success ?? false,
                preRequisites = result?.ListOfData.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<JsonResult> AddTraining(TrainingViewModel trainingViewModel)
        {
            Training training = trainingViewModel.ToTraining();
            OperationResult result = await _trainingService.SaveTrainingAsync(training, trainingViewModel.PreRequisiteIds, isUpdate: false);
            return Json(new
            {
                success = result?.Success ?? false,
                message = result?.Message ?? "An error occurred"
            });
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<JsonResult> UpdateTraining(EditTrainingViewModel editTrainingViewModel)
        {
            Training training = editTrainingViewModel.ToTraining();
            OperationResult result = await _trainingService.SaveTrainingAsync(training, editTrainingViewModel.PreRequisiteIds, isUpdate: true);
            return Json(new
            {
                success = result?.Success ?? false,
                message = result?.Message ?? "An error occurred"
            });
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<JsonResult> Delete(int trainingId)
        {
            OperationResult result = await _trainingService.DeleteTrainingAsync(trainingId);
            return Json(new
            {
                success = result?.Success ?? false,
                message = result?.Message ?? "An error occurred"
            });
        }
    }
}