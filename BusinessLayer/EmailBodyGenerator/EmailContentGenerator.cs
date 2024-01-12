using DAL.DTO;
using Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.StaticClass
{
    public static class EmailContentGenerator
    {
        public static void GenerateTrainingNotificationEmailBody(SendEmailDTO emailDTO, ApplicationStatusEnum result, 
            out string htmlBody, out string subject, string message = "", bool isDeclinedByCapacity = false)
        {
            htmlBody = "";

            switch (result)
            {
                case ApplicationStatusEnum.Approved:
                    htmlBody = GenerateApprovedEmailBody(emailDTO);
                    break;
                case ApplicationStatusEnum.Declined:
                    htmlBody = isDeclinedByCapacity ? GenerateDeclinedByCapacityEmailBody(emailDTO) :GenerateDeclinedEmailBody(emailDTO, message); 
                    break;
                case ApplicationStatusEnum.Selected:
                    htmlBody = GenerateSelectedEmailBody(emailDTO);
                    break;
                case ApplicationStatusEnum.Pending:
                    GenerateApplicationSubmissionEmailBody(emailDTO, out htmlBody, out subject);
                    return;
                default:
                    break;
            }
            subject = $"Training Request for {emailDTO.TrainingName} - {result}";
        }

        private static void GenerateApplicationSubmissionEmailBody(SendEmailDTO emailDTO, out string htmlBody, out string subject)
        {
            htmlBody = $@"
            <html>
                <head>
                    <title>Training Application Submission</title>
                </head>
                <body style='font-family: Arial, sans-serif;'>
                    <p>Dear <strong>{emailDTO.ManagerName},</strong></p>
                    <p>A new training application has been submitted by <strong>{emailDTO.EmployeeName}</strong> for the training program <strong>{emailDTO.TrainingName}</strong>.</p>
                    <br/>
                    <p>Please review and take appropriate action.</p>
                    <br/>
                    <p>Best regards</p>
                </body>
            </html>";
            subject = $"New Training Application - {emailDTO.TrainingName}";
        }

        private static string GenerateApprovedEmailBody(SendEmailDTO emailDTO)
        {
            return $@"
            <html>
                <head>
                    <title>Training Notification</title>
                </head>
                <body style='font-family: Arial, sans-serif;'>
                    <p>Dear <strong>{emailDTO.EmployeeName},</strong></p>
                    <p>This is an automated notification from the training system.</p>
                    <p>Your training request for <strong>{emailDTO.TrainingName}</strong> has been <strong>{ApplicationStatusEnum.Approved}</strong> by your manager, 
                     <strong>{emailDTO.ManagerName}</strong>.</p>
                    <br/>
                    <p>You are now one step closer to being selected. Please patiently await the next steps in the selection process to determine if you have been officially enrolled in the training.</p>
                    <br/>
                    <p>Best regards</p>
                </body>
            </html>";
        }

        private static string GenerateDeclinedByCapacityEmailBody(SendEmailDTO emailDTO)
        {
            return $@"
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

        private static string GenerateDeclinedEmailBody(SendEmailDTO emailDTO, string message)
        {
            return $@"
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

        private static string GenerateSelectedEmailBody(SendEmailDTO emailDTO)
        {
            return $@"
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
    }
}
