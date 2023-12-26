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
    public class EnrollmentProcessController : Controller
    {
        private readonly IEnrollmentProcessService _enrollmentProcessService;
        public EnrollmentProcessController(IEnrollmentProcessService enrollmentProcessService)
        {
            _enrollmentProcessService = enrollmentProcessService;
        }

        [HttpGet]
        [CustomAuthorization("Manager")]
        public async Task<JsonResult> ViewApplications()
        {
            short managerId = SessionManager.CurrentUser.Id;
            List<PendingApplicationDTO> pendingApplications = (await _enrollmentProcessService.GetApplicationsAsync(managerId)).ToList();
            return Json(new
            {
                success = pendingApplications != null && pendingApplications.Any(),
                PendingApplications = pendingApplications
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CustomAuthorization("Manager")]
        public async Task<JsonResult> ViewDocuments(int applicationId)
        {
            List<PendingApplicationDocumentDTO> documents = (await _enrollmentProcessService.GetApplicationDocumentAsync(applicationId)).ToList();
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
        [CustomAuthorization("Manager")]
        public async Task<ActionResult> DownloadAttachment(int attachmentId)
        {
            PendingApplicationDocumentDTO document = SessionManager.Attachments.FirstOrDefault(doc => doc.AttachmentId == attachmentId);
            byte[] binaryFile = document.File;
            string contentType = "application/octet-stream"; // TODO: Perform server side validations to display allowed files based on extensions
            string fileName = Uri.UnescapeDataString(document.FileName); // Decode encoded file name
            return await Task.Run(() => File(binaryFile, contentType, fileName));
        }
    }
}