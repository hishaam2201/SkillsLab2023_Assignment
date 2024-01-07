using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace DAL.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IDatabaseCommand<Application> _dbCommand;
        public ApplicationRepository(IDatabaseCommand<Application> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<int> InsertApplicationAndGetId(Application application)
        {
            const string INSERT_APPLICATION_QUERY = @"INSERT INTO [Application] (UserId, TrainingId)
                                                      VALUES (@UserId, @TrainingId)
                                                      SELECT SCOPE_IDENTITY();";
            var excludedApplicationParameters = new List<string> { "Id", "ApplicationStatus", "ApplicationDateTime", "DeclineReason" };
            SqlParameter[] applicationParameters = _dbCommand.GetSqlParametersFromObject(application, excludedApplicationParameters);
            return Convert.ToInt32(await _dbCommand.GetScalerResultAsync(INSERT_APPLICATION_QUERY, applicationParameters));
        }

        public async Task<bool> InsertDocumentUpload(DocumentUpload documentUpload)
        {
            const string INSERT_DOCUMENT_UPLOAD_QUERY = @"INSERT INTO DocumentUpload (ApplicationId, [File], PreRequisiteId, FileName) 
                                                          VALUES (@ApplicationId, @File, @PreRequisiteId, @FileName);";
            var excludedDocumentUploadParameters = new List<string> { "Id" };
            SqlParameter[] documentUploadPreRequisite = _dbCommand.GetSqlParametersFromObject(documentUpload, excludedDocumentUploadParameters);
            return (await _dbCommand.AffectedRowsCountAsync(INSERT_DOCUMENT_UPLOAD_QUERY, documentUploadPreRequisite)) > 0;
        }
    }

}
