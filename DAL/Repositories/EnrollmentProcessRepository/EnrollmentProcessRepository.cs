using DAL.DTO;
using DAL.Models;
using DAL.Repositories.TrainingRepository;
using Framework.AppLogger;
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
        private readonly ILogger _logger;
        public EnrollmentProcessRepository(IDatabaseCommand<User> dbCommand, ILogger logger)
        {
            _dbCommand = dbCommand;
            _logger = logger;
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
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    TrainingName = reader["TrainingName"].ToString(),
                    DepartmentName = reader["DepartmentName"].ToString(),
                    ApplicationStatus = reader["ApplicationStatus"].ToString()
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
                    FileName = reader["FileName"].ToString(),
                    PreRequisiteName = reader["Name"].ToString(),
                    PreRequisiteDescription = reader["PreRequisiteDescription"].ToString(),
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
                string firstName = reader["FirstName"].ToString();
                string lastName = reader["LastName"].ToString();
                return new SendEmailDTO
                {
                    EmployeeName = $"{firstName} {lastName}",
                    EmployeeEmail = reader["Email"].ToString(),
                    TrainingName = reader["TrainingName"].ToString()
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
                string firstName = reader["FirstName"].ToString();
                string lastName = reader["LastName"]?.ToString();
                return new SendEmailDTO
                {
                    EmployeeName = $"{firstName} {lastName}",
                    EmployeeEmail = reader["Email"].ToString(),
                    TrainingName = reader["TrainingName"].ToString()
                };
            };
            var result = (await _dbCommand.ExecuteSelectQueryAsync(DECLINE_APPLICATION_QUERY, parameters, mapFunction)).FirstOrDefault();
            return (true, result);
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllExpiredTrainingIdsAsync()
        {
            try
            {
                const string GET_EXPIRED_TRAINING_IDS =
                @"SELECT Id, TrainingName, TrainingCourseStartingDateTime FROM Training
                  WHERE
                    DeadlineOfApplication = @DeadlineOfApplication;";
                SqlParameter[] parameter = _dbCommand.GetSqlParametersFromObject(new { DeadlineOfApplication = DateTime.Now.Date });
                Func<IDataReader, TrainingDTO> mapFunction = reader =>
                {
                    return new TrainingDTO
                    {
                        TrainingId = (short)reader["Id"],
                        TrainingName = reader["TrainingName"].ToString(),
                        TrainingCourseStartingDateTime = (DateTime)reader["TrainingCourseStartingDateTime"]
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_EXPIRED_TRAINING_IDS, parameter, mapFunction);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Guid.NewGuid());
                throw;
            }
        }

        public async Task<IEnumerable<SelectionProcessDTO>> ProcessUsersSelectionAsync(short trainingId, string declineReason = "")
        {
            try
            {
                const string PROCESS_SELECTION_QUERY =
                @"CREATE TABLE #ApprovedUsers (
                        UserId SMALLINT,
                        FirstName NVARCHAR(64),
                        LastName NVARCHAR(129),
                        Email NVARCHAR(128),
	                    ApplicationStatus varchar(20)
                    );

                    INSERT INTO #ApprovedUsers (UserId, Email, FirstName, LastName)
                    SELECT TOP (SELECT Capacity FROM Training WHERE Id = @TrainingId)
                        u.Id AS UserId, u.Email, u.FirstName, u.LastName
                    FROM [User] AS u
                    JOIN [Application] a ON a.UserId = u.Id
                    JOIN Training t ON t.Id = a.TrainingId
                    WHERE t.Id = @TrainingId AND a.ApplicationStatus = @Approved
                    ORDER BY
                        IIF(u.DepartmentId = t.DepartmentId, 0, 1),
                        a.ApplicationDateTime;

                    DECLARE @SelectedUsers TABLE (
                        UserId SMALLINT,
                        FirstName NVARCHAR(64),
                        LastName NVARCHAR(129),
                        Email NVARCHAR(128),
	                    ApplicationStatus varchar(20)
                    );

                    UPDATE [Application]
                    SET ApplicationStatus = @Selected, SelectedDate = @SelectedDate
                    OUTPUT inserted.UserId, au.FirstName, au.LastName, au.Email, inserted.ApplicationStatus
                    INTO @SelectedUsers
                    FROM [Application] AS a
                    INNER JOIN #ApprovedUsers au ON au.UserId = a.UserId
                    WHERE TrainingId = @TrainingId;

                    DECLARE @RejectedUsers TABLE (
                        UserId SMALLINT,
                        FirstName NVARCHAR(64),
                        LastName NVARCHAR(129),
                        Email NVARCHAR(128),
	                    ApplicationStatus varchar(20)
                    );

                    UPDATE [Application]
                    SET 
	                    ApplicationStatus = @Declined, 
	                    DeclineReason = @DeclineReason
                    OUTPUT inserted.UserId, u.FirstName, u.LastName, u.Email, inserted.ApplicationStatus
                    INTO @RejectedUsers
                    FROM [Application] AS a
                    INNER JOIN [User] AS u ON u.Id = a.UserId
                    INNER JOIN Training AS t ON t.Id = a.TrainingId
                    WHERE TrainingId = @TrainingId
                        AND ApplicationStatus = @Approved

                    SELECT UserId, FirstName, LastName, Email, ApplicationStatus FROM @SelectedUsers
                    UNION ALL
                    SELECT UserId, FirstName, LastName, Email, ApplicationStatus FROM @RejectedUsers;

                    DROP TABLE #ApprovedUsers;";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
                {
                    TrainingId = trainingId,
                    Approved = ApplicationStatusEnum.Approved.ToString(),
                    Selected = ApplicationStatusEnum.Selected.ToString(),
                    SelectedDate = DateTime.Now,
                    Declined = ApplicationStatusEnum.Declined.ToString(),
                    DeclineReason = declineReason
                });
                SelectionProcessDTO mapFunction(IDataReader reader)
                {
                    return new SelectionProcessDTO
                    {
                        UserId = (short)reader["UserId"],
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        ApplicationStatus = Enum.TryParse(reader["ApplicationStatus"].ToString(), out ApplicationStatusEnum status)
                                            ? status : default
                    };
                }
                var result = await _dbCommand.ExecuteSelectQueryAsync(PROCESS_SELECTION_QUERY, parameters, mapFunction);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Guid.NewGuid());
                throw;
            }
        }
    }
}
