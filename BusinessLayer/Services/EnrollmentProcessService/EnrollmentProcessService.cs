using DAL.DTO;
using DAL.Models;
using DAL.Repositories.EnrollmentProcessRepository;
using Framework.BackgroundEnrollmentProcessLogger;
using Framework.Enums;
using Framework.Notification;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Repositories.TrainingRepository;
using OfficeOpenXml;
using System;
using System.IO;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public class EnrollmentProcessService : IEnrollmentProcessService
    {
        private readonly IEnrollmentProcessRepository _enrollmentProcessRepository;
        private readonly IBackgroundJobLogger _backgroundJobLogger;
        private readonly ITrainingRepository _trainingRepository;
        public EnrollmentProcessService(IEnrollmentProcessRepository enrollmentProcessRepository,
            IBackgroundJobLogger backgroundJobLogger, ITrainingRepository trainingRepository)
        {
            _enrollmentProcessRepository = enrollmentProcessRepository;
            _backgroundJobLogger = backgroundJobLogger;
            _trainingRepository = trainingRepository;
        }

        // TODO: Try to combine approve and decline in the same service call if possible  
        public async Task<bool> ApproveApplicationAsync(int applicationId, string managerName)
        {
            var (isApproved, result) = await _enrollmentProcessRepository.ApproveApplicationAsync(applicationId);
            if (isApproved)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = result.EmployeeName,
                    EmployeeEmail = result.EmployeeEmail,
                    ManagerName = managerName,
                    TrainingName = result.TrainingName
                };
                GenerateEmailBody(emailDTO, result: ApplicationStatusEnum.Approved, out string body, out string subject);

#pragma warning disable CS4014
                EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
#pragma warning restore CS4014

                return true;
            }
            return false;
        }

        public async Task<bool> DeclineApplicationAsync(int applicationId, string managerName, string message)
        {
            var (isDeclined, result) = await _enrollmentProcessRepository.DeclineApplicationAsync(applicationId, message);
            if (isDeclined)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = result.EmployeeName,
                    EmployeeEmail = result.EmployeeEmail,
                    ManagerName = managerName,
                    TrainingName = result.TrainingName
                };
                GenerateEmailBody(emailDTO, result: ApplicationStatusEnum.Declined, out string body, out string subject, message: message);

#pragma warning disable CS4014
                EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
#pragma warning restore CS4014

                return true;
            }
            return false;
        }

        public async Task<OperationResult> GetApplicationDocumentAsync(int applicationId)
        {
            var documents = (await _enrollmentProcessRepository.GetApplicationDocumentAsync(applicationId)).ToList();
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
            return await _enrollmentProcessRepository.GetApplicationsAsync(managerId);
        }

        public async Task<SelectedProcessUserDTO> GetSelectedUsersForTrainingAsync(short trainingId)
        {
            TrainingDTO training = await _trainingRepository.GetTrainingByIdAsync(trainingId);
            List<SelectionProcessDTO> listOfSelectedUsers = (await _enrollmentProcessRepository.GetSelectedUsersForTrainingAsync(trainingId)).ToList();
            SelectedProcessUserDTO selectedUsersDTO = new SelectedProcessUserDTO
            {
                TrainingName = training.TrainingName,
                TrainingDepartment = training.DepartmentName,
                TrainingStartDateTime = training.TrainingCourseStartingDateTime,
                SelectedProcessUsers = listOfSelectedUsers
            };
            return selectedUsersDTO;
        }

        public async Task PerformAutomaticSelectionProcessAsync()
        {
            const string DECLINE_REASON =
                @"We regret to inform you that your application for this training program has been declined. 
	              Due to high demand, we have reached our maximum capacity. We appreciate your interest and 
                  encourage you to apply for future opportunities. Thank you for understanding.";
            List<TrainingDTO> trainingDTOs = (await _enrollmentProcessRepository.GetAllExpiredTrainingIdsAsync()).ToList();
            if (trainingDTOs == null || !trainingDTOs.Any())
            {
                _backgroundJobLogger.LogInformation("No expired training sessions found.");
                return;
            }

            foreach (TrainingDTO trainingDTO in trainingDTOs)
            {
                List<SelectionProcessDTO> selectionProcessUsers =
                    (await _enrollmentProcessRepository.ProcessUsersSelectionAsync(trainingDTO.TrainingId, DECLINE_REASON))?.ToList();

                if (selectionProcessUsers == null || !selectionProcessUsers.Any())
                {
                    _backgroundJobLogger.LogInformation($"No employees found to select for the training {trainingDTO.TrainingName}.");
                    continue;
                }
                short selected = 0, declined = 0;
                foreach (SelectionProcessDTO user in selectionProcessUsers)
                {
                    SendEmailDTO emailDTO = new SendEmailDTO
                    {
                        EmployeeName = $"{user.FirstName} {user.LastName}",
                        EmployeeEmail = user.Email,
                        TrainingName = trainingDTO.TrainingName
                    };
                    // Perform filtering which email to send (Selected or rejected)
                    if (user.ApplicationStatus == ApplicationStatusEnum.Selected)
                    {
                        emailDTO.TrainingStartDate = trainingDTO.TrainingCourseStartingDateTime;
                        GenerateEmailBody(emailDTO, ApplicationStatusEnum.Selected, out string body, out string subject);
#pragma warning disable CS4014
                        EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
#pragma warning restore CS4014
                        selected++;
                    }
                    else
                    {
                        GenerateEmailBody(emailDTO, ApplicationStatusEnum.Declined, out string body, out string subject, isDeclinedByCapacity: true);
#pragma warning disable CS4014
                        EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
#pragma warning restore CS4014
                        declined++;
                    }
                }
                _backgroundJobLogger.LogInformation($"{selected} employee(s) have been selected for the training {trainingDTO.TrainingName}.\n " +
                    $"{declined} employee(s) have been declined for the training due to availability of seats.");
            }
        }

        public async Task<OperationResult> PerformManualSelectionProcessAsync(short trainingId)
        {
            if (await _enrollmentProcessRepository.SelectionAlreadyDoneForTodayAsync(trainingId))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Selection process has already been done for this training today."
                };
            }

            const string DECLINE_REASON =
                @"We regret to inform you that your application for this training program has been declined. 
	              Due to high demand, we have reached our maximum capacity. We appreciate your interest and 
                  encourage you to apply for future opportunities. Thank you for understanding.";
            TrainingDTO training = await _trainingRepository.GetTrainingByIdAsync(trainingId);
            List<SelectionProcessDTO> selectionProcessUsers =
                    (await _enrollmentProcessRepository.ProcessUsersSelectionAsync(trainingId, DECLINE_REASON))?.ToList();

            if (selectionProcessUsers == null || !selectionProcessUsers.Any())
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"No employees found to select for the training {training.TrainingName}."
                };
            }
            foreach (SelectionProcessDTO user in selectionProcessUsers)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = $"{user.FirstName} {user.LastName}",
                    EmployeeEmail = user.Email,
                    TrainingName = training.TrainingName
                };
                // Perform filtering which email to send (Selected or rejected)
                if (user.ApplicationStatus == ApplicationStatusEnum.Selected)
                {
                    emailDTO.TrainingStartDate = training.TrainingCourseStartingDateTime;
                    GenerateEmailBody(emailDTO, ApplicationStatusEnum.Selected, out string body, out string subject);
#pragma warning disable CS4014
                    EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
#pragma warning restore CS4014
                }
                else
                {
                    GenerateEmailBody(emailDTO, ApplicationStatusEnum.Declined, out string body, out string subject, isDeclinedByCapacity: true);
#pragma warning disable CS4014
                    EmailSender.SendEmailAsync(subject, body, emailDTO.EmployeeEmail);
#pragma warning restore CS4014
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = $"Selection process completed."
            };
        }

        public async Task<byte[]> ExportToExcel(short trainingId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"ExportedSelectedUsers_{DateTime.Now}");
                var headers = new string[]
                { "Employee_FirstName", "Employee_LastName", "Employee_MobileNumber", "Employee_Email", "Manager_FirstName", "Manager_LastName" };
                for (byte i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Adding Data to worksheet
                List<ExportSelectedEmployeeDTO> exportSelectedEmployees =
                    (await _enrollmentProcessRepository.GetSelectedUserDetailsForExportAsync(trainingId)).ToList();
                byte rowIndex = 2;
                foreach (var employee in exportSelectedEmployees)
                {
                    worksheet.Cells[rowIndex, 1].Value = employee.EmployeeFirstName;
                    worksheet.Cells[rowIndex, 2].Value = employee.EmployeeLastName;
                    worksheet.Cells[rowIndex, 3].Value = employee.EmployeeMobileNumber;
                    worksheet.Cells[rowIndex, 4].Value = employee.EmployeeEmail;
                    worksheet.Cells[rowIndex, 5].Value = employee.ManagerFirstName;
                    worksheet.Cells[rowIndex, 6].Value = employee.ManagerLastName;

                    rowIndex++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                return stream.ToArray();
            }
        }

        // PRIVATE HELPER METHODS
        private void GenerateEmailBody(SendEmailDTO emailDTO, ApplicationStatusEnum result, out string htmlBody, out string subject,
            string message = "", bool isDeclinedByCapacity = false)
        {
            htmlBody = "";
            if (result == ApplicationStatusEnum.Approved)
            {
                htmlBody = $@"
                    <html>
                        <head>
                            <title>Training Notification</title>
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
            else if (result == ApplicationStatusEnum.Declined)
            {
                if (isDeclinedByCapacity)
                {
                    htmlBody = $@"
                    <html>
                        <head>
                            <title>Training Notification</title>
                        </head>
                        <body style='font-family: Arial, sans-serif;'>
                            <p>Dear <strong>{emailDTO.EmployeeName},</strong></p>
                            <p>This is an automated notification from the training system.</p>
                            <p>We regret to inform you that your application for the training program <strong>{emailDTO.TrainingName}</strong> has been declined. 
                               Due to high demand, we have reached our maximum capacity. We appreciate your interest and encourage you to apply for future opportunities. 
                               Thank you for understanding.</p>
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
                            <h5>Reason</h5>
                            <p><em>{message}</em></p>
                            <br/>
                            <p>Feel free to reach out to your manager for further clarification or explore other training opportunities.</p>
                            <br/>
                            <p>Best regards</p>
                        </body>
                    </html>";
                }
            }
            else if (result == ApplicationStatusEnum.Selected)
            {
                htmlBody = $@"
                    <html>
                        <head>
                            <title>Training Notification</title>
                        </head>
                        <body style='font-family: Arial, sans-serif;'>
                            <p>Dear <strong>{emailDTO.EmployeeName},</strong></p>
                            <p>Congratulations! You have been selected for the training program <strong>{emailDTO.TrainingName}</strong>.</p>
                            <br/>
                            <p>Your dedication and commitment have been recognized, and we look forward to your active participation in the upcoming training sessions.</p>
                            <br/>
                            <p>Details of the training:</p>
                            <ul>
                                <li><strong>Training Name:</strong> {emailDTO.TrainingName}</li>
                                <li><strong>Training Start Date:</strong> {emailDTO.TrainingStartDate:dd-MM-yyyy HH:mm}</li>
                            </ul>
                            <br/>
                            <p>Should you have any questions or require further information, please do not hesitate to reach out to your manager.</p>
                            <br/>
                            <p>Best regards</p>
                        </body>
                    </html>";
            }
            subject = $"Training Request for {emailDTO.TrainingName} - {result}";
        }
    }
}