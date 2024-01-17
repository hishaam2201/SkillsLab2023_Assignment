using BusinessLayer.Services.ApplicationService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
using Framework.HelperClasses;
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
        public async Task<JsonResult> Enroll(EnrollViewModel enrollViewModel)
        {
            UserDTO userInformation = SessionManager.CurrentUser;

            List<DocumentUploadDTO> enrollmentDataList = enrollViewModel.Files != null && enrollViewModel.Files.Any()
                    ? enrollViewModel.Files.ToDocumentUploadWithPreRequisites(enrollViewModel.TrainingId, enrollViewModel.TrainingName)
                    : new List<DocumentUploadDTO> { new DocumentUploadDTO { TrainingId = enrollViewModel.TrainingId, TrainingName = enrollViewModel.TrainingName } };

            string dashboardAction = Extensions.GetDashboardAction(SessionManager.CurrentUser.RoleId);
            OperationResult result = await _applicationService.ProcessEmployeeApplicationAsync(userInformation, enrollmentDataList);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                redirectUrl = result.Success ? Url.Action(dashboardAction, "Home") : null
            });
        }

        [HttpPost]
        public async Task<JsonResult> ViewDocuments(int applicationId)
        {
            OperationResult result = await _applicationService.GetApplicationDocumentAsync(applicationId);
            if (result.Success)
            {
                var documents = result.ListOfData as List<ApplicationDocumentDTO>;
                SessionManager.Attachments = documents;
                return Json(new
                {
                    success = result.Success,
                    Attachments = documents.Select(d => d.AttachmentInfoDTO).ToList()
                });
            }
            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpPost]
        public async Task<JsonResult> ApproveApplication(int applicationId)
        {
            string managerName = $"{SessionManager.CurrentUser.FirstName} {SessionManager.CurrentUser.LastName}";
            OperationResult result = await _applicationService.ApproveApplicationAsync(applicationId, managerName);
            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpPost]
        public async Task<JsonResult> DeclineApplication(int applicationId, string message)
        {
            string managerName = $"{SessionManager.CurrentUser.FirstName} {SessionManager.CurrentUser.LastName}";
            OperationResult result = await _applicationService.DeclineApplicationAsync(applicationId, managerName, message);
            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAttachment(int attachmentId)
        {
            ApplicationDocumentDTO document = SessionManager.Attachments.FirstOrDefault(doc => doc.AttachmentInfoDTO.AttachmentId == attachmentId);

            if (document == null || document.File == null || document.File.Length == 0)
                return Json(new { success = false, message = "No file found to download" });

            string fileName = Uri.UnescapeDataString(document.FileName); // Decode encoded file name
            string contentType = Extensions.GetContentTypeFromFileName(fileName);

            return await Task.Run(() => File(document.File, contentType, fileName));
        }

    }
}