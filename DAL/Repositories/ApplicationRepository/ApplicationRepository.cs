using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using DAL.DTO;
using System.Data;
using Framework.Enums;
using System.Linq;

namespace DAL.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IDatabaseCommand<Application> _dbCommand;
        public ApplicationRepository(IDatabaseCommand<Application> dbCommand)
        {
            _dbCommand = dbCommand;
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
            SendEmailDTO mapFunction(IDataReader reader)
            {
                string firstName = reader["FirstName"].ToString();
                string lastName = reader["LastName"].ToString();
                return new SendEmailDTO
                {
                    EmployeeName = $"{firstName} {lastName}",
                    Email = reader["Email"].ToString(),
                    TrainingName = reader["TrainingName"].ToString()
                };
            }
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
                string firstName = reader["FirstName"].ToString();
                string lastName = reader["LastName"]?.ToString();
                return new SendEmailDTO
                {
                    EmployeeName = $"{firstName} {lastName}",
                    Email = reader["Email"].ToString(),
                    TrainingName = reader["TrainingName"].ToString()
                };
            };
            var result = (await _dbCommand.ExecuteSelectQueryAsync(DECLINE_APPLICATION_QUERY, parameters, mapFunction)).FirstOrDefault();
            return (true, result);
        }

        public async Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            const string GET_APPLICATIONS_QUERY =
            @"SELECT a.Id AS ApplicationId, u.FirstName, u.LastName, u.Email, t.TrainingName, d.DepartmentName AS TrainingDepartment, ApplicationStatus
                  FROM [Application] AS a
                  INNER JOIN
                      [User] AS u ON u.Id = a.UserId
                  INNER JOIN
                      [User] AS m ON m.Id = u.ManagerId
                  INNER JOIN
                      Training AS t ON t.Id = a.TrainingId
                  INNER JOIN 
	              Department AS d ON d.Id = t.DepartmentId
                  WHERE ApplicationStatus = @Pending AND u.ManagerId = @ManagerId
                  ORDER BY
                        IIF(m.DepartmentId = t.DepartmentId, 0, 1)";

            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                ManagerId = managerId,
                Pending = ApplicationStatusEnum.Pending.ToString()
            });
            ApplicationDTO mapFunction(IDataReader reader)
            {
                return new ApplicationDTO
                {
                    ApplicationId = Convert.ToInt32(reader["ApplicationId"]),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    TrainingName = reader["TrainingName"].ToString(),
                    TrainingDepartment = reader["TrainingDepartment"].ToString(),
                    ApplicationStatus = reader["ApplicationStatus"].ToString()
                };
            }
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
                    FileName = reader["FileName"].ToString(),
                    PreRequisiteName = reader["Name"].ToString(),
                    PreRequisiteDescription = reader["PreRequisiteDescription"].ToString(),
                };
            };
            var result = await _dbCommand.ExecuteSelectQueryAsync(GET_APPLICATION_DOCUMENT_QUERY, parameters, mapFunction);
            return result;
        }

        public async Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserIdAsync(short userId)
        {
            const string GET_USER_APPLICATION_QUERY =
                @"SELECT a.ApplicationStatus, t.TrainingName, d.DepartmentName AS TrainingDepartment, a.DeclineReason, a.ApplicationDateTime
                  FROM [Application] AS a
                  INNER JOIN [User] AS u ON u.Id = a.UserId
                  INNER JOIN Training AS t ON t.Id = a.TrainingId
                  INNER JOIN Department AS d ON d.Id = t.DepartmentId
                  WHERE u.Id = @UserId
                  ORDER BY
                  CASE a.ApplicationStatus
                        WHEN 'Selected' THEN 1
                        WHEN 'Approved' THEN 2
                        WHEN 'Pending' THEN 3
                        WHEN 'Declined' THEN 4
                        ELSE 5
                    END,
                    IIF(u.DepartmentId = t.DepartmentId, 0, 1);";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { UserId = userId });
            UserApplicationDTO mapFunction(IDataReader reader)
            {
                return new UserApplicationDTO
                {
                    ApplicationStatus = Enum.TryParse(reader["ApplicationStatus"].ToString(), out ApplicationStatusEnum status) ? status : default,
                    TrainingName = reader["TrainingName"].ToString(),
                    TrainingDepartment = reader["TrainingDepartment"].ToString(),
                    DeclineReason = reader["DeclineReason"] != DBNull.Value ? reader["DeclineReason"].ToString() : null,
                    ApplicationDateTime = (DateTime)reader["ApplicationDateTime"]
                };
            }
            return await _dbCommand.ExecuteSelectQueryAsync(GET_USER_APPLICATION_QUERY, parameters, mapFunction);
        }

        public async Task<ApplicationDTO> InsertApplicationAndGetIdAsync(Application application)
        {
            const string INSERT_APPLICATION_QUERY = @"INSERT INTO [Application] (UserId, TrainingId)
                                                      VALUES (@UserId, @TrainingId)
                                                      
                                                      SELECT SCOPE_IDENTITY() AS ApplicationId, t.TrainingName 
                                                      FROM [Application] AS a 
                                                      INNER JOIN Training AS t ON t.Id = a.TrainingId 
                                                      WHERE a.Id = SCOPE_IDENTITY()";
            var excludedApplicationParameters = new List<string> { "Id", "ApplicationStatus", "ApplicationDateTime", "DeclineReason" };
            SqlParameter[] applicationParameters = _dbCommand.GetSqlParametersFromObject(application, excludedApplicationParameters);
            ApplicationDTO mapFunction(IDataReader reader)
            {
                return new ApplicationDTO
                {
                    ApplicationId = Convert.ToInt32(reader["ApplicationId"]),
                    TrainingName = reader["TrainingName"].ToString()
                };
            }
            var result = (await _dbCommand.ExecuteSelectQueryAsync(INSERT_APPLICATION_QUERY, applicationParameters, mapFunction)).FirstOrDefault();
            return result;
        }

        public async Task<bool> InsertDocumentUploadAsync(DocumentUpload documentUpload)
        {
            const string INSERT_DOCUMENT_UPLOAD_QUERY = @"INSERT INTO DocumentUpload (ApplicationId, [File], PreRequisiteId, FileName) 
                                                          VALUES (@ApplicationId, @File, @PreRequisiteId, @FileName);";
            var excludedDocumentUploadParameters = new List<string> { "Id" };
            SqlParameter[] documentUploadPreRequisite = _dbCommand.GetSqlParametersFromObject(documentUpload, excludedDocumentUploadParameters);
            return (await _dbCommand.AffectedRowsCountAsync(INSERT_DOCUMENT_UPLOAD_QUERY, documentUploadPreRequisite)) > 0;
        }
    }

}
