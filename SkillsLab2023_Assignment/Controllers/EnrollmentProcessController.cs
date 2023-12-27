using BusinessLayer.Services.EnrollmentProcessService;
using DAL.DTO;
using SkillsLab2023_Assignment.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SkillsLab2023_Assignment.Controllers
{
    [UserSession]
    [CustomAuthorization("Manager")]
    public class EnrollmentProcessController : Controller
    {
        private readonly IEnrollmentProcessService _enrollmentProcessService;
        public EnrollmentProcessController(IEnrollmentProcessService enrollmentProcessService)
        {
            _enrollmentProcessService = enrollmentProcessService;
        }

        [HttpGet]
        public async Task<JsonResult> ViewApplications()
        {
            short managerId = SessionManager.CurrentUser.Id;
            List<ApplicationDTO> pendingApplications = (await _enrollmentProcessService.GetApplicationsAsync(managerId)).ToList();
            return Json(new
            {
                success = pendingApplications != null && pendingApplications.Any(),
                PendingApplications = pendingApplications
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ViewDocuments(int applicationId)
        {
            List<ApplicationDocumentDTO> documents = (await _enrollmentProcessService.GetApplicationDocumentAsync(applicationId)).ToList();
            SessionManager.Attachments = documents;

            List<AttachmentInfoDTO> attachmentInfoList = documents.Select(doc => new AttachmentInfoDTO
            {
                AttachmentId = doc.AttachmentId,
                PreRequisiteDescription = doc.PreRequisiteDescription
            }).ToList();

            return Json(new
            {
                success = documents != null && documents.Any(),
                Attachments = attachmentInfoList
            });
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAttachment(int attachmentId)
        {
            ApplicationDocumentDTO document = SessionManager.Attachments.FirstOrDefault(doc => doc.AttachmentId == attachmentId);
            byte[] binaryFile = document.File;
            string contentType = "application/octet-stream"; // TODO: Perform server side validations to display allowed files based on extensions
            string fileName = Uri.UnescapeDataString(document.FileName); // Decode encoded file name
            return await Task.Run(() => File(binaryFile, contentType, fileName));
        }

        [HttpPost]
        public async Task<JsonResult> ApproveApplication(int applicationId)
        {
            string managerName = $"{SessionManager.CurrentUser.FirstName} {SessionManager.CurrentUser.LastName}";
            bool isApproved = await _enrollmentProcessService.ApproveApplicationAsync(applicationId, managerName);
            return Json(new
            {
                success = isApproved
            });
        }

        [HttpPost]
        public async Task<JsonResult> DeclineApplication(int applicationId, string message)
        {
            string managerName = $"{SessionManager.CurrentUser.FirstName} {SessionManager.CurrentUser.LastName}";
            bool isDeclined = await _enrollmentProcessService.DeclineApplicationAsync(applicationId, managerName, message);
            return Json(new
            {
                success = isDeclined
            });
        }
    }
}