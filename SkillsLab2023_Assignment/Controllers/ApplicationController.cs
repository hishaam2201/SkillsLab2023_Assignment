using BusinessLayer.Services.ApplicationService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Framework.StaticClass;
using SkillsLab2023_Assignment.Custom;
using SkillsLab2023_Assignment.Mapper;
using SkillsLab2023_Assignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [ValidationFilter]
    [UserSession]
    [CustomAuthorization(RoleEnum.Manager)]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost, CustomAuthorization(RoleEnum.Employee)]
        public async Task<JsonResult> Enroll(short trainingId, string trainingName, List<DocumentUploadViewModel> files)
        {
            UserDTO userInformation = SessionManager.CurrentUser; 

            List<DocumentUploadDTO> enrollmentDataList =
                files != null && files.Any()
                    ? files.ToDocumentUploadWithPreRequisites(userInformation.Id, trainingId, trainingName)
                    : new List<DocumentUploadDTO> { new DocumentUploadDTO { UsertId = userInformation.Id, TrainingId = trainingId, TrainingName = trainingName } };

            bool isSuccessful = await _applicationService.ProcessEmployeeApplicationAsync(userInformation, enrollmentDataList);
            return Json(new { 
                success = isSuccessful, 
                message = isSuccessful ? "Manager notified of your application." : "Could not perform application...", 
                redirectUrl = isSuccessful ? Url.Action("EmployeeDashboard", "Home") : null 
            });
        }

        [HttpPost]
        public async Task<JsonResult> ViewDocuments(int applicationId)
        {
            OperationResult result = await _applicationService.GetApplicationDocumentAsync(applicationId);

            if (result.Success)
            {
                var listOfData = (result.ListOfData).ToList();
                var documents = (List<ApplicationDocumentDTO>)listOfData[0];
                SessionManager.Attachments = documents;

                return Json(new
                {
                    success = result?.Success ?? false,
                    Attachments = (List<AttachmentInfoDTO>)listOfData[1]
                });
            }
            else
            {
                return Json(new
                {
                    success = result?.Success ?? false,
                    message = result?.Message
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ApproveApplication(int applicationId)
        {
            string managerName = $"{SessionManager.CurrentUser.FirstName} {SessionManager.CurrentUser.LastName}";
            bool isApproved = await _applicationService.ApproveApplicationAsync(applicationId, managerName);
            return Json(new
            {
                success = isApproved,
                message = isApproved ? "Application successfully approved" : "Could not approve application"
            });
        }

        [HttpPost]
        public async Task<JsonResult> DeclineApplication(int applicationId, string message)
        {
            string managerName = $"{SessionManager.CurrentUser.FirstName} {SessionManager.CurrentUser.LastName}";
            bool isDeclined = await _applicationService.DeclineApplicationAsync(applicationId, managerName, message);
            return Json(new
            {
                success = isDeclined,
                message = isDeclined ? "Application successfully declined" : "Could not decline application"
            });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAttachment(int attachmentId)
        {
            ApplicationDocumentDTO document = SessionManager.Attachments.FirstOrDefault(doc => doc.AttachmentId == attachmentId);

            if (document == null || document.File == null || document.File.Length == 0)
                return Json(new { success = false, message = "No file found to download" });

            string fileName = Uri.UnescapeDataString(document.FileName); // Decode encoded file name
            string contentType = Extensions.GetContentTypeFromFileName(fileName);

            return await Task.Run(() => File(document.File, contentType, fileName));
        }

    }
}