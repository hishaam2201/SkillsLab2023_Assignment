using DAL.DTO;
using DAL.Models;
using Framework.DatabaseCommand.DatabaseCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAL.Repositories.ManagerRepository
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly IDatabaseCommand<User> _dbCommand;
        public ManagerRepository(IDatabaseCommand<User> dbCommand) 
        {
            _dbCommand = dbCommand;
        }

        public async Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            try
            {
                const string GET_APPLICATIONS_QUERY =
                @"SELECT a.Id AS ApplicationId, u.FirstName, u.LastName, u.Email, t.TrainingName, ApplicationStatus
                  FROM [Application] AS a
                  INNER JOIN
                      [User] AS u ON u.Id = a.UserId
                  INNER JOIN
                      Training AS t ON t.Id = a.TrainingId
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
                        ApplicationStatus = reader["ApplicationStatus"]?.ToString()
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_APPLICATIONS_QUERY, parameters, mapFunction);
                return result;  
            }
            catch(Exception) { throw; }

        }

        public Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
