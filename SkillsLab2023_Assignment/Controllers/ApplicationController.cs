using BusinessLayer.Services.ApplicationService;
using DAL.DTO;
using Framework.Enums;
using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [ValidationFilter]
    [UserSession]
    [CustomAuthorization(RoleEnum.Employee)]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        public async Task<JsonResult> Enroll(int trainingId, List<DocumentUploadViewModel> files)
        {
            List<DocumentUploadDTO> enrollmentDataList = new List<DocumentUploadDTO>();
            if (files != null && files.Any())
            {
                enrollmentDataList = files.Select(file => new DocumentUploadDTO
                {
                    UsertId = SessionManager.CurrentUser.Id,
                    TrainingId = trainingId,
                    PreRequisiteId = file.PreRequisiteId,
                    File = file.File,
                    FileName = file.FileName
                }).ToList();
            }
            else
            {
                enrollmentDataList.Add(new DocumentUploadDTO
                {
                    UsertId = SessionManager.CurrentUser.Id,
                    TrainingId = trainingId,
                });
            }

            bool isSuccessful = await _applicationService.ProcessApplication(enrollmentDataList);
            if (isSuccessful)
            {
                return Json(new { success = true, message = "Application successful", redirectUrl = Url.Action("EmployeeDashboard", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Could not perform application..." });
            }
        }
    }
}