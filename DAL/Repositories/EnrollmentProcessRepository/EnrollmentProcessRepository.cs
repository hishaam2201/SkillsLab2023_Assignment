using DAL.DTO;
using DAL.Models;
using Framework.DatabaseCommand.DatabaseCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public async Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            try
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
                Func<IDataReader, PendingApplicationDTO> mapFunction = reader =>
                {
                    return new PendingApplicationDTO
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
            catch(Exception) { throw; }

        }

        public async Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId)
        {
            try
            {
                const string GET_APPLICATION_DOCUMENT_QUERY =
                @"SELECT du.Id AS AttachmentId, du.[File], pr.PreRequisiteDescription, du.[FileName]
                  FROM DocumentUpload AS du
                  INNER JOIN
                      PreRequisite pr ON pr.Id = du.PreRequisiteId
                  WHERE du.ApplicationId = @ApplicationId;";

                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { ApplicationId = applicationId });
                Func<IDataReader, PendingApplicationDocumentDTO> mapFunction = reader =>
                {
                    return new PendingApplicationDocumentDTO
                    {
                        AttachmentId = (int)reader["AttachmentId"],
                        File = reader["File"] as byte[],
                        FileName = reader["FileName"]?.ToString(),
                        PreRequisiteDescription = reader["PreRequisiteDescription"]?.ToString(),
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_APPLICATION_DOCUMENT_QUERY, parameters, mapFunction);
                return result;
            }
            catch (Exception) { throw; }
        }
    }
}
