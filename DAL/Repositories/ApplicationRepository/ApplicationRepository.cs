using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using DAL.DTO;
using System.Data;
using Framework.Enums;

namespace DAL.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IDatabaseCommand<Application> _dbCommand;
        public ApplicationRepository(IDatabaseCommand<Application> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserIdAsync(short userId)
        {
            const string GET_USER_APPLICATION_QUERY =
                @"SELECT a.ApplicationStatus, t.TrainingName, d.DepartmentName AS TrainingDepartment, a.DeclineReason, a.ApplicationDateTime
                  FROM [Application] AS a
                  INNER JOIN [User] AS u ON u.Id = a.UserId
                  INNER JOIN Training AS t ON t.Id = a.TrainingId
                  INNER JOIN Department AS d ON d.Id = t.DepartmentId
                  WHERE u.Id = @UserId";
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

        public async Task<int> InsertApplicationAndGetIdAsync(Application application)
        {
            const string INSERT_APPLICATION_QUERY = @"INSERT INTO [Application] (UserId, TrainingId)
                                                      VALUES (@UserId, @TrainingId)
                                                      SELECT SCOPE_IDENTITY();";
            var excludedApplicationParameters = new List<string> { "Id", "ApplicationStatus", "ApplicationDateTime", "DeclineReason" };
            SqlParameter[] applicationParameters = _dbCommand.GetSqlParametersFromObject(application, excludedApplicationParameters);
            return Convert.ToInt32(await _dbCommand.GetScalerResultAsync(INSERT_APPLICATION_QUERY, applicationParameters));
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
