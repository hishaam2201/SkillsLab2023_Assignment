using DAL.DTO;
using DAL.Models;
using DAL.Repositories.ApplicationRepository;
using Framework.Enums;
using Framework.Notification;
using Framework.StaticClass;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserId(short userId)
        {
            return await _applicationRepository.GetApplicationByUserIdAsync(userId);
        }

        public async Task<bool> ApproveApplicationAsync(int applicationId, string managerName)
        {
            var (isApproved, result) = await _applicationRepository.ApproveApplicationAsync(applicationId);
            if (isApproved)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = result.EmployeeName,
                    Email = result.Email,
                    ManagerName = managerName,
                    TrainingName = result.TrainingName
                };
                EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Approved, out string body, out string subject);
#pragma warning disable CS4014
                EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
                return true;
            }
            return false;
        }

        public async Task<bool> DeclineApplicationAsync(int applicationId, string managerName, string message)
        {
            var (isDeclined, result) = await _applicationRepository.DeclineApplicationAsync(applicationId, message);
            if (isDeclined)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = result.EmployeeName,
                    Email = result.Email,
                    ManagerName = managerName,
                    TrainingName = result.TrainingName
                };
                EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Declined, 
                    out string body, out string subject, message: message);
#pragma warning disable CS4014
                EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
                return true;
            }
            return false;
        }

        public async Task<OperationResult> GetApplicationDocumentAsync(int applicationId)
        {
            var documents = (await _applicationRepository.GetApplicationDocumentAsync(applicationId)).ToList();
            if (documents != null && documents.Any())
            {
                List<AttachmentInfoDTO> attachmentInfoList = documents.Select(doc => new AttachmentInfoDTO
                {
                    AttachmentId = doc.AttachmentId,
                    PreRequisiteName = doc.PreRequisiteName,
                    PreRequisiteDescription = doc.PreRequisiteDescription
                }).ToList();

                return new OperationResult
                {
                    Success = true,
                    ListOfData = new List<object> { documents, attachmentInfoList }
                };
            }
            else
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "No documents found."
                };
            }
        }

        public async Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            return await _applicationRepository.GetApplicationsAsync(managerId);
        }

        public async Task<bool> ProcessApplicationAsync(UserDTO userInformation, List<DocumentUploadDTO> documentUploads)
        {
            if (documentUploads == null || !documentUploads.Any())
            {
                return false;
            }
            var firstDocument = documentUploads.First();
            var applicationEntity = new Application
            {
                UserId = firstDocument.UsertId,
                TrainingId = firstDocument.TrainingId,
            };

            ApplicationDTO application = await _applicationRepository.InsertApplicationAndGetIdAsync(applicationEntity);
            if (application.ApplicationId <= 0)
            {
                return false;
            }

            foreach (var documentUpload in documentUploads)
            {
                if (documentUpload.File != null && documentUpload.File.ContentLength > 0)
                {
                    // Read file content into a byte array
                    byte[] fileData;
                    using (var stream = new MemoryStream())
                    {
                        await documentUpload.File.InputStream.CopyToAsync(stream);
                        fileData = stream.ToArray();
                    }

                    var documentUploadEntity = new DocumentUpload
                    {
                        ApplicationId = application.ApplicationId,
                        File = fileData,
                        PreRequisiteId = documentUpload.PreRequisiteId,
                        FileName = documentUpload.FileName
                    };

                    if (!await _applicationRepository.InsertDocumentUploadAsync(documentUploadEntity))
                    {
                        // TODO: Delete record if application has failed
                        return false;
                    }
                }
            }
            SendEmailDTO emailDTO = new SendEmailDTO
            {
                EmployeeName = $"{userInformation.FirstName} {userInformation.LastName}",
                Email = userInformation.ManagerEmail,
                ManagerName = userInformation.ManagerName,
                TrainingName = application.TrainingName
            };
            EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Pending, out string body, out string subject);
#pragma warning disable CS4014
            EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
            return true;
        }
    }
}
