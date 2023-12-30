using DAL.DTO;
using DAL.Models;
using Framework.DatabaseCommand.DatabaseCommand;
using Framework.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.EnrollmentProcessRepository
{
    public class EnrollmentProcessRepository : IEnrollmentProcessRepository
    {
        private readonly IDatabaseCommand<User> _dbCommand;
        public EnrollmentProcessRepository(IDatabaseCommand<User> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            const string GET_APPLICATIONS_QUERY =
            @"SELECT a.Id AS ApplicationId, u.FirstName, u.LastName, u.Email, t.TrainingName, d.DepartmentName, ApplicationStatus
                  FROM [Application] AS a
                  INNER JOIN
                      [User] AS u ON u.Id = a.UserId
                  INNER JOIN
                      Training AS t ON t.Id = a.TrainingId
                  INNER JOIN 
	              Department AS d ON d.Id = t.DepartmentId
                  WHERE ApplicationStatus = 'Pending' AND u.ManagerId = @ManagerId;";

            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { ManagerId = managerId });
            Func<IDataReader, ApplicationDTO> mapFunction = reader =>
            {
                return new ApplicationDTO
                {
                    ApplicationId = (int)reader["ApplicationId"],
                    FirstName = reader["FirstName"]?.ToString(),
                    LastName = reader["LastName"]?.ToString(),
                    Email = reader["Email"]?.ToString(),
                    TrainingName = reader["TrainingName"]?.ToString(),
                    DepartmentName = reader["DepartmentName"]?.ToString(),
                    ApplicationStatus = reader["ApplicationStatus"]?.ToString()
                };
            };
            var result = await _dbCommand.ExecuteSelectQueryAsync(GET_APPLICATIONS_QUERY, parameters, mapFunction);
            return result;
        }

        public async Task<IEnumerable<ApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId)
        {

            const string GET_APPLICATION_DOCUMENT_QUERY =
                @"SELECT du.Id AS AttachmentId, du.[File], pr.[Name], pr.PreRequisiteDescription, du.[FileName]
                  FROM DocumentUpload AS du
                  INNER JOIN
                      PreRequisite pr ON pr.Id = du.PreRequisiteId
                  WHERE du.ApplicationId = @ApplicationId;";

            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { ApplicationId = applicationId });
            Func<IDataReader, ApplicationDocumentDTO> mapFunction = reader =>
            {
                return new ApplicationDocumentDTO
                {
                    AttachmentId = (int)reader["AttachmentId"],
                    File = reader["File"] as byte[],
                    FileName = reader["FileName"]?.ToString(),
                    PreRequisiteName = reader["Name"]?.ToString(),
                    PreRequisiteDescription = reader["PreRequisiteDescription"]?.ToString(),
                };
            };
            var result = await _dbCommand.ExecuteSelectQueryAsync(GET_APPLICATION_DOCUMENT_QUERY, parameters, mapFunction);
            return result;
        }

        public async Task<(bool, SendEmailDTO)> ApproveApplicationAsync(int applicationId)
        {
            const string APPROVE_APPLICATION_QUERY = @"
                                            UPDATE [Application]
                                            SET ApplicationStatus = @ApplicationStatus
                                            WHERE Id = @ApplicationId;

                                            SELECT u.FirstName, u.LastName, u.Email, t.TrainingName FROM [Application]
                                            INNER JOIN [User] AS u
                                            ON u.Id = [Application].UserId
                                            INNER JOIN Training AS t
                                            ON t.Id = [Application].TrainingId
                                            WHERE [Application].Id = @ApplicationId";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                ApplicationStatus = (ApplicationStatusEnum.Approved.ToString()),
                ApplicationId = applicationId
            });
            Func<IDataReader, SendEmailDTO> mapFunction = reader =>
            {
                string firstName = reader["FirstName"]?.ToString();
                string lastName = reader["LastName"]?.ToString();
                return new SendEmailDTO
                {
                    EmployeeName = $"{firstName} {lastName}",
                    EmployeeEmail = reader["Email"]?.ToString(),
                    TrainingName = reader["TrainingName"]?.ToString()
                };
            };
            var result = (await _dbCommand.ExecuteSelectQueryAsync(APPROVE_APPLICATION_QUERY, parameters, mapFunction)).FirstOrDefault();
            return (true, result);
        }

        public async Task<(bool, SendEmailDTO)> DeclineApplicationAsync(int applicationId, string message)
        {

            const string DECLINE_APPLICATION_QUERY = @"UPDATE [Application]
                                                           SET ApplicationStatus = @ApplicationStatus,
                                                           DeclineReason = @DeclineReason
                                                           WHERE 
	                                                       Id = @ApplicationId;

                                                           SELECT u.FirstName, u.LastName, u.Email, t.TrainingName FROM [Application]
                                                           INNER JOIN [User] AS u
                                                           ON u.Id = [Application].UserId
                                                           INNER JOIN Training AS t
                                                           ON t.Id = [Application].TrainingId
                                                           WHERE [Application].Id = @ApplicationId";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                ApplicationStatus = (ApplicationStatusEnum.Declined.ToString()),
                DeclineReason = message,
                ApplicationId = applicationId
            });
            Func<IDataReader, SendEmailDTO> mapFunction = reader =>
            {
                string firstName = reader["FirstName"]?.ToString();
                string lastName = reader["LastName"]?.ToString();
                return new SendEmailDTO
                {
                    EmployeeName = $"{firstName} {lastName}",
                    EmployeeEmail = reader["Email"]?.ToString(),
                    TrainingName = reader["TrainingName"]?.ToString()
                };
            };
            var result = (await _dbCommand.ExecuteSelectQueryAsync(DECLINE_APPLICATION_QUERY, parameters, mapFunction)).FirstOrDefault();
            return (true, result);
        }
    }
}
