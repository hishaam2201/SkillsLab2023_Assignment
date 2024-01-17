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
using BusinessLayer.EmailGenerator;

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

        public async Task<SelectionProcessDTO> GetSelectedUsersForTrainingAsync(short trainingId)
        {
            TrainingDTO training = await _trainingRepository.GetTrainingByIdAsync(trainingId);
            List<SelectedUserDTO> listOfSelectedUsers = (await _enrollmentProcessRepository.GetSelectedUsersForTrainingAsync(trainingId)).ToList();
            SelectionProcessDTO selectionProcessDTO = new SelectionProcessDTO
            {
                TrainingId = trainingId,
                TrainingName = training.TrainingName,
                TrainingDepartment = training.DepartmentName,
                TrainingStartDateTime = training.TrainingCourseStartingDateTime,
                SelectedUsersList = listOfSelectedUsers
            };
            return selectionProcessDTO;
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
                List<SelectedUserDTO> selectionProcessUsers =
                    (await _enrollmentProcessRepository.ProcessUsersSelectionAsync(trainingDTO.TrainingId, DECLINE_REASON))?.ToList();

                if (selectionProcessUsers == null || !selectionProcessUsers.Any())
                {
                    _backgroundJobLogger.LogInformation($"No employees found to select for the training {trainingDTO.TrainingName}.");
                    continue;
                }
                short selected = 0, declined = 0;
                foreach (SelectedUserDTO user in selectionProcessUsers)
                {
                    SendEmailDTO emailDTO = new SendEmailDTO
                    {
                        EmployeeName = $"{user.FirstName} {user.LastName}",
                        Email = user.Email,
                        TrainingName = trainingDTO.TrainingName
                    };
                    // Perform filtering which email to send (Selected or rejected)
                    if (user.ApplicationStatus == ApplicationStatusEnum.Selected)
                    {
                        emailDTO.TrainingStartDate = trainingDTO.TrainingCourseStartingDateTime;
                        EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Selected, out string body, out string subject);
#pragma warning disable CS4014
                        EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
                        selected++;
                    }
                    else
                    {
                        EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Declined, 
                            out string body, out string subject, isDeclinedByCapacity: true);
#pragma warning disable CS4014
                        EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
                        declined++;
                    }
                }
                _backgroundJobLogger.LogInformation($"{selected} employee(s) have been selected for the training {trainingDTO.TrainingName}.\n" +
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
            List<SelectedUserDTO> selectionProcessUsers =
                    (await _enrollmentProcessRepository.ProcessUsersSelectionAsync(trainingId, DECLINE_REASON))?.ToList();

            if (selectionProcessUsers == null || !selectionProcessUsers.Any())
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"No employees found to select for the training {training.TrainingName}."
                };
            }
            foreach (SelectedUserDTO user in selectionProcessUsers)
            {
                SendEmailDTO emailDTO = new SendEmailDTO
                {
                    EmployeeName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    TrainingName = training.TrainingName
                };
                // Perform filtering which email to send (Selected or rejected)
                if (user.ApplicationStatus == ApplicationStatusEnum.Selected)
                {
                    emailDTO.TrainingStartDate = training.TrainingCourseStartingDateTime;
                    EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Selected, out string body, out string subject);
#pragma warning disable CS4014
                    EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
#pragma warning restore CS4014
                }
                else
                {
                    EmailContentGenerator.GenerateTrainingNotificationEmailBody(emailDTO, ApplicationStatusEnum.Declined, 
                        out string body, out string subject, isDeclinedByCapacity: true);
#pragma warning disable CS4014
                    EmailSender.SendEmailAsync(subject, body, emailDTO.Email);
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
    }
}