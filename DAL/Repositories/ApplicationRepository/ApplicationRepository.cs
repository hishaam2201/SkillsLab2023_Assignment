using Framework.DatabaseCommand.DatabaseCommand;
using DAL.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<bool> ApplyForTraining(Application application, DocumentUpload document)
        {
            try
            {
                const string APPLY_FOR_TRAINING_QUERY =
                @"INSERT INTO [Application] (UserId, TrainingId) Values (@UserId, @TrainingId);

                  DECLARE @ApplicationId INT
                  SET @ApplicationId = SCOPE_IDENTITY()

                  INSERT INTO DocumentUpload (ApplicationId, [File], PreRequisiteId) 
                  VALUES (@ApplicationId, @File, @PreRequisiteId);";

                var excludedApplicationParameters = new List<string> { "Id", "ApplicationStatus", "ApplicationDateTime" };
                SqlParameter[] applicationParameters = _dbCommand.GetSqlParametersFromObject(application, excludedApplicationParameters);

                var excludedDocumentUploadParameters = new List<string> { "Id", "ApplicationId" };
                SqlParameter[] documentUploadParameters = _dbCommand.GetSqlParametersFromObject(document, excludedDocumentUploadParameters);

                SqlParameter[] allParameters = applicationParameters.Concat(documentUploadParameters).ToArray();
                return await _dbCommand.ExecuteTransactionAsync(new SqlCommand(APPLY_FOR_TRAINING_QUERY), allParameters);
            }
            catch (Exception) { throw; }
        }
    }

}
