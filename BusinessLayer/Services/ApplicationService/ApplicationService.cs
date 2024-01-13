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

        public async Task<OperationResult> ProcessEmployeeApplicationAsync(UserDTO userInformation, List<DocumentUploadDTO> enrollmentDataList)
        {
            if (enrollmentDataList == null || !enrollmentDataList.Any())
                return new OperationResult
                {
                    Success = false,
                    Message = "No enrollment data found to enroll employee."
                };

            bool isApplicationSuccessful;
            var firstEnrollmentDocument = enrollmentDataList.First();
            if (firstEnrollmentDocument.File == null)
            {
                isApplicationSuccessful = await _applicationRepository.InsertIntoApplicationAsync(userInformation.Id, firstEnrollmentDocument.TrainingId);
            }
            else
            {
                List<DocumentUpload> documentUploads = new List<DocumentUpload>();
                foreach (var enrollmentData in enrollmentDataList)
                {
                    if (enrollmentData.File != null && enrollmentData.File.ContentLength > 0)
                    {
                        // Read file content into a byte array
                        byte[] fileData;
                        using (var stream = new MemoryStream())
                        {
                            await enrollmentData.File.InputStream.CopyToAsync(stream);
                            fileData = stream.ToArray();
                        }
                        documentUploads.Add(new DocumentUpload
                        {
                            File = fileData,
                            PreRequisiteId = enrollmentData.PreRequisiteId,
                            FileName = enrollmentData.FileName
                        });
                    }
                }
                isApplicationSuccessful = await _applicationRepository.ProcessEmployeeApplicationAndDocumentsAsync
                    (userInformation.Id, firstEnrollmentDocument.TrainingId, documentUploads);
            }

            if (isApplicationSuccessful)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = $"{userInformation.FirstName} {userInformation.LastName}",
                    Email = userInformation.ManagerEmail,
                    ManagerName = userInformation.ManagerName,
                    TrainingName = firstEnrollmentDocument.TrainingName
                };
                EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Pending, out string body, out string subject);
#pragma warning disable CS4014
                EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
                return new OperationResult
                {
                    Success = true,
                    Message = "Manager notified of your application."
                };
            }
            else
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Could not perform application..."
                };
            }
        }
    }
}
