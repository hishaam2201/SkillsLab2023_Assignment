using DAL.DTO;
using DAL.Models;
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

        public async Task<bool> SelectionAlreadyDoneForTodayAsync(short trainingId)
        {
            const string GET_SELECTED_USERS_FOR_TODAY_QUERY =
                    @"SELECT COUNT(*) AS SelectedUsersForToday
                      FROM [User] AS u
                      JOIN [Application] a ON a.UserId = u.Id
                      JOIN Training t ON t.Id = a.TrainingId
                      JOIN Department d ON d.Id = u.DepartmentId
                      JOIN Department td ON td.Id = t.DepartmentId
                      WHERE t.Id = @TrainingId AND (a.ApplicationStatus = @Selected AND CAST(a.SelectedDate AS DATE) = @CurrentDate)";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                TrainingId = trainingId,
                Selected = ApplicationStatusEnum.Selected.ToString(),
                CurrentDate = DateTime.Now.Date
            });
            return (int)await _dbCommand.GetScalerResultAsync(GET_SELECTED_USERS_FOR_TODAY_QUERY, parameters) > 0;

        }

        public async Task<IEnumerable<SelectedUserDTO>> GetSelectedUsersForTrainingAsync(short trainingId)
        {
            const string GET_SELECTED_USERS_QUERY =
                @"SELECT 
	                u.FirstName, u.LastName, d.DepartmentName AS UserDepartment, a.ApplicationStatus
	              FROM [User] AS u
	              JOIN [Application] a ON a.UserId = u.Id
	              JOIN Training t ON t.Id = a.TrainingId
	              JOIN Department d ON d.Id = u.DepartmentId
	              JOIN Department td ON td.Id = t.DepartmentId
	              WHERE t.Id = @TrainingId AND (a.ApplicationStatus = @Selected AND CAST(a.SelectedDate AS DATE) = @CurrentDate)
	              ORDER BY
		              IIF(u.DepartmentId = t.DepartmentId, 0, 1),
                      d.DepartmentName";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                TrainingId = trainingId,
                Selected = ApplicationStatusEnum.Selected.ToString(),
                CurrentDate = DateTime.Now.Date
            });
            SelectedUserDTO mapFunction(IDataReader reader)
            {
                return new SelectedUserDTO
                {
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    DepartmentName = reader["UserDepartment"].ToString(),
                    ApplicationStatus = Enum.TryParse(reader["ApplicationStatus"].ToString(), out ApplicationStatusEnum status) ? status : default
                };
            }
            return await _dbCommand.ExecuteSelectQueryAsync(GET_SELECTED_USERS_QUERY, parameters, mapFunction);
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
                TrainingDTO mapFunction(IDataReader reader)
                {
                    return new TrainingDTO
                    {
                        TrainingId = (short)reader["Id"],
                        TrainingName = reader["TrainingName"].ToString(),
                        TrainingCourseStartingDateTime = (DateTime)reader["TrainingCourseStartingDateTime"]
                    };
                }
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_EXPIRED_TRAINING_IDS, parameter, mapFunction);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Guid.NewGuid());
                throw;
            }
        }

        public async Task<IEnumerable<SelectedUserDTO>> ProcessUsersSelectionAsync(short trainingId, string declineReason = "")
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
                    JOIN Department d ON d.Id = u.DepartmentId
                    JOIN Training t ON t.Id = a.TrainingId
                    WHERE t.Id = @TrainingId AND a.ApplicationStatus = @Approved
                    ORDER BY
                        IIF(u.DepartmentId = t.DepartmentId, 0, 1),
                        d.DepartmentName,
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
                SelectedUserDTO mapFunction(IDataReader reader)
                {
                    return new SelectedUserDTO
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

        public async Task<IEnumerable<ExportSelectedEmployeeDTO>> GetSelectedUserDetailsForExportAsync(short trainingId)
        {
            const string GET_SELECTED_USER_DETAILS_QUERY =
                @"SELECT u.FirstName, u.LastName, u.MobileNumber, u.Email AS EmployeeEmail, m.FirstName AS ManagerFirstName, m.LastName AS ManagerLastName
	              FROM [User] AS u
	              INNER JOIN [User] AS m ON m.Id = u.ManagerId
	              INNER JOIN [Application] AS a ON a.UserId = u.Id
	              INNER JOIN Training t ON t.Id = a.TrainingId
	              WHERE t.Id = @TrainingId AND (a.ApplicationStatus = @Selected AND CAST(a.SelectedDate AS DATE) = @CurrentDate)";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                TrainingId = trainingId,
                Selected = ApplicationStatusEnum.Selected.ToString(),
                CurrentDate = DateTime.Now.Date
            });
            ExportSelectedEmployeeDTO mapFunction(IDataReader reader)
            {
                return new ExportSelectedEmployeeDTO
                {
                    EmployeeFirstName = reader["FirstName"].ToString(),
                    EmployeeLastName = reader["LastName"].ToString(),
                    EmployeeMobileNumber = reader["MobileNumber"].ToString(),
                    EmployeeEmail = reader["EmployeeEmail"].ToString(),
                    ManagerFirstName = reader["ManagerFirstName"].ToString(),
                    ManagerLastName = reader["ManagerLastName"].ToString()
                };
            }
            return await _dbCommand.ExecuteSelectQueryAsync(GET_SELECTED_USER_DETAILS_QUERY, parameters, mapFunction);
        }
    }
}
