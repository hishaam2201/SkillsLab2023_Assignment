using BusinessLayer.Services.ApplicationService;
using DAL.DTO;
using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    [CustomAuthorization("Employee")]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        public async Task<JsonResult> Enroll(List<DocumentUploadViewModel> files)
        {
            // TODO: Perform server side validations as well
            var enrollmentDataList = files.Select(file => new DocumentUploadDTO
            {
                UsertId = SessionManager.CurrentUser.Id,
                TrainingId = file.TrainingId,
                PreRequisiteId = file.PreRequisiteId,
                File = file.File,
                FileName = file.FileName
            }).ToList();

            // TODO: File restriction to only images, and look at max file size

            bool isUploaded = await _applicationService.ProcessApplication(enrollmentDataList);
            if (isUploaded)
            {
                return Json(new { success = true, message = "Application successful", redirectUrl = Url.Action("EmployeeDashboard", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Could not perform application..."});
            }
        }
    }
}