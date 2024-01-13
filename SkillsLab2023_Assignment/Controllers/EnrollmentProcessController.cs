using BusinessLayer.Services.EnrollmentProcessService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using SkillsLab2023_Assignment.Custom;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    [CustomAuthorization(RoleEnum.Administrator)]
    public class EnrollmentProcessController : Controller
    {
        private readonly IEnrollmentProcessService _enrollmentProcessService;
        public EnrollmentProcessController(IEnrollmentProcessService enrollmentProcessService)
        {
            _enrollmentProcessService = enrollmentProcessService;
        }

        [HttpPost]
        public async Task<ActionResult> ViewSelectedUsers(short trainingId)
        {
            SelectionProcessDTO selectionProcessUsers = await _enrollmentProcessService.GetSelectedUsersForTrainingAsync(trainingId);
            return View(selectionProcessUsers);
        }

        [HttpPost]
        public async Task<JsonResult> PerformManualSelectionProcess(short trainingId)
        {
            OperationResult result = await _enrollmentProcessService.PerformManualSelectionProcessAsync(trainingId);
            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpPost]
        public async Task<ActionResult> DownloadSelectedUsers(short trainingId)
        {
            byte[] excelFileBytes = await _enrollmentProcessService.ExportToExcel(trainingId);
            string fileName = $"ExportedSelectedEmployees_{DateTime.Now:f}.xlsx";

            Response.Clear();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", $"attachment; filename={fileName}");
            return File(excelFileBytes, contentType);
        }
    }
}