using DAL.DTO;
using DAL.Repositories.EnrollmentProcessRepository;
using Framework.Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public class EnrollmentProcessService : IEnrollmentProcessService
    {
        private readonly IEnrollmentProcessRepository _enrollmentProcessRepository;
        public EnrollmentProcessService(IEnrollmentProcessRepository enrollmentProcessRepository)
        {
            _enrollmentProcessRepository = enrollmentProcessRepository;
        }

        public async Task<bool> ApproveApplicationAsync(int applicationId, string managerName)
        {
            var (isApproved, result) = await _enrollmentProcessRepository.ApproveApplicationAsync(applicationId);

            if (isApproved)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    IsApproved = isApproved,
                    EmployeeName = result.EmployeeName,
                    EmployeeEmail = result.EmployeeEmail,
                    ManagerName = managerName,
                    TrainingName = result.TrainingName
                };
                GenerateEmailBody(emailDTO, out string body, out string subject);
                try
                {
                    return await EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
                }
                catch(Exception) { throw; }
            }
            return false;
        }

        public async Task<bool> DeclineApplicationAsync(int applicationId, string managerName, string message)
        {
            var (isDeclined, result) = await _enrollmentProcessRepository.DeclineApplicationAsync(applicationId);

            if (isDeclined)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    IsApproved = isDeclined,
                    EmployeeName = result.EmployeeName,
                    EmployeeEmail = result.EmployeeEmail,
                    ManagerName = managerName,
                    TrainingName = result.TrainingName
                };
                GenerateEmailBody(emailDTO, out string body, out string subject, message);
                try
                {
                    return await EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
                }
                catch (Exception) { throw; }
            }
            return false;
        }

        public async Task<IEnumerable<ApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId)
        {
            return await _enrollmentProcessRepository.GetApplicationDocumentAsync(applicationId);
        }

        public async Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            return await _enrollmentProcessRepository.GetApplicationsAsync(managerId);
        }

        // PRIVATE HELPER METHODS
        private void GenerateEmailBody(SendEmailDTO emailDTO, out string htmlBody, out string subject, string message = "")
        {
            string result = emailDTO.IsApproved ? "Approved" : "Rejected";

            if (result == "Approved")
            {
                htmlBody = $@"
                    <html>
                        <head>
                            <title>Training Approval Notification</title>
                        </head>
                       <body style='font-family: Arial, sans-serif;'>
                            <p>Dear <strong>{emailDTO.EmployeeName},</strong></p>
                            <p>This is an automated notification from the training system.</p>
                            <p>Your training request for <strong>{emailDTO.TrainingName}</strong> has been <strong>{result}</strong> by your manager, 
                             <strong>{emailDTO.ManagerName}</strong>.</p>
                            <br/>
                            <p>You are now one step closer to being selected. Please patiently await the next steps in the selection process to determine if you have been officially enrolled in the training.</p>
                            <br/>
                            <p>Best regards</p>
                        </body>
                    </html>";
            }
            else
            {
                htmlBody = $@"
                    <html>
                        <head>
                            <title>Training Notification</title>
                        </head>
                        <body style='font-family: Arial, sans-serif;'>
                            <p>Dear <strong>{emailDTO.EmployeeName},</strong></p>
                            <p>This is an automated notification from the training system.</p>
                            <p>Unfortunately, your training request for <strong>{emailDTO.TrainingName}</strong> has been declined by your manager, 
                                <strong>{emailDTO.ManagerName}</strong>.</p>
                            <br/>
                            <p><em>{message}</em></p>
                            <br/>
                            <p>Feel free to reach out to your manager for further clarification or explore other training opportunities.</p>
                            <br/>
                            <p>Best regards</p>
                        </body>
                    </html>";
            }
            subject = $"Training Request for {emailDTO.TrainingName} - {result}";
        }
    }
}
