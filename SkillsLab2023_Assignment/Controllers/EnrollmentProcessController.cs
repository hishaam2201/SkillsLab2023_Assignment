using BusinessLayer.Services.EnrollmentProcessService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Framework.StaticClass;
using SkillsLab2023_Assignment.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    [CustomAuthorization(RoleEnum.Manager)]
    public class EnrollmentProcessController : Controller
    {
        private readonly IEnrollmentProcessService _enrollmentProcessService;
        public EnrollmentProcessController(IEnrollmentProcessService enrollmentProcessService)
        {
            _enrollmentProcessService = enrollmentProcessService;
        }

        [HttpPost]
        [CustomAuthorization(RoleEnum.Administrator)]
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
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<ActionResult> ViewSelectedUsers(short trainingId)
        {
            SelectedProcessUserDTO selectedUsers = await _enrollmentProcessService.GetSelectedUsersForTrainingAsync(trainingId);
            return View(selectedUsers);
        }

        [HttpPost, CustomAuthorization(RoleEnum.Administrator)]
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