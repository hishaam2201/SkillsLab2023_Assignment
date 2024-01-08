using BusinessLayer.Services.EnrollmentProcessService;
using DAL.DTO;
using DAL.Models;
using Framework.Enums;
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
            OperationResult result = await _enrollmentProcessService.GetApplicationDocumentAsync(applicationId);

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

        [HttpGet]
        public async Task<ActionResult> DownloadAttachment(int attachmentId)
        {
            ApplicationDocumentDTO document = SessionManager.Attachments.FirstOrDefault(doc => doc.AttachmentId == attachmentId);
            byte[] binaryFile = document.File;
            string contentType = "application/octet-stream";
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

        [HttpGet]
        [CustomAuthorization(RoleEnum.Administrator)]
        public async Task<ActionResult> ViewSelectedUsers(short trainingId)
        {
            SelectedProcessUserDTO selectedUsers = await _enrollmentProcessService.GetSelectedUsersForTrainingAsync(trainingId);
            return View(selectedUsers);
        }
    }
}