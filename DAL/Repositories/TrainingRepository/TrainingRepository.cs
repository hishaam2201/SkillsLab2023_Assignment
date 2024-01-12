using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Framework.DatabaseCommand.DatabaseCommand;
using DAL.DTO;
using DAL.Models;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Framework.Enums;

namespace DAL.Repositories.TrainingRepository
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly IDatabaseCommand<Training> _dbCommand;
        public TrainingRepository(IDatabaseCommand<Training> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId, short userId)
        {
            List<TrainingDTO> trainingDTOs = new List<TrainingDTO>();
            const string GET_ALL_UNAPPLIED_TRAININGS_QUERY = @"
                                SELECT t.* FROM Training AS t
                                WHERE 
	                                t.IsDeadlineExpired = @IsDeadlineExpired
	                                AND NOT EXISTS (
		                                SELECT 1 FROM [Application] AS a
		                                WHERE a.TrainingId = t.Id
		                                AND a.UserId = @UserId
	                                )
                                ORDER BY 
                                CASE 
                                  WHEN DepartmentId = @UserDepartmentId THEN 0
                                  ELSE 1
                                END,
                                DepartmentId;";
            SqlParameter[] parameter = _dbCommand.GetSqlParametersFromObject(new { IsDeadlineExpired = 0, UserId = userId, UserDepartmentId = userDepartmentId });
            List<Training> trainingList = (await _dbCommand.ExecuteSelectQueryAsync<Training>(GET_ALL_UNAPPLIED_TRAININGS_QUERY, parameter)).ToList();

            foreach (Training training in trainingList)
            {
                trainingDTOs.Add(new TrainingDTO
                {
                    TrainingId = (short)training.Id,
                    TrainingName = training.TrainingName,
                    DeadlineOfApplication = training.DeadlineOfApplication,
                    Capacity = training.Capacity,
                    DepartmentName = (await RetrieveDepartmentNameAsync((byte)training.DepartmentId)).ToString()
                });
            }
            return trainingDTOs;
        }

        public async Task<TrainingDTO> GetTrainingByIdAsync(int trainingId)
        {
            Training training = await _dbCommand.GetByIdAsync(trainingId);
            return new TrainingDTO
            {
                TrainingId = (short)training.Id,
                TrainingName = training.TrainingName,
                Description = training.Description,
                DeadlineOfApplication = training.DeadlineOfApplication,
                Capacity = training.Capacity,
                DepartmentName = (await RetrieveDepartmentNameAsync((byte)training.DepartmentId)).ToString(),
                TrainingCourseStartingDateTime = training.TrainingCourseStartingDateTime,
                PreRequisites = (await RetrieveTrainingPreRequisitesAsync(training.Id)).ToList()
            };
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync()
        {
            List<Training> trainingList = (await _dbCommand.ExecuteSelectQueryAsync<Training>()).ToList();
            List<TrainingDTO> trainingDTOs = new List<TrainingDTO>();

            foreach (Training training in trainingList)
            {
                object departmentName = await RetrieveDepartmentNameAsync((byte)training.DepartmentId);

                trainingDTOs.Add(new TrainingDTO
                {
                    TrainingId = (short)training.Id,
                    TrainingName = training.TrainingName,
                    Description = training.Description,
                    TrainingCourseStartingDateTime = training.TrainingCourseStartingDateTime,
                    DeadlineOfApplication = training.DeadlineOfApplication,
                    Capacity = training.Capacity,
                    DepartmentName = departmentName?.ToString(),
                    IsDeadlineExpired = training.IsDeadlineExpired,
                    PreRequisites = (await RetrieveTrainingPreRequisitesAsync(training.Id)).ToList()
                });
            }
            return trainingDTOs;
        }

        public async Task UpdateDeadlineExpiryStatusAsync()
        {
            const string UPDATE_DEADLINE_EXPIRY_STATUS_QUERY = @"
                                UPDATE Training
                                SET IsDeadlineExpired = @IsExpired
                                WHERE GETDATE() > DeadlineOfApplication 
                                AND IsDeadlineExpired = @IsNotExpired;";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { IsExpired = 1, IsNotExpired = 0 });
            await _dbCommand.AffectedRowsCountAsync(UPDATE_DEADLINE_EXPIRY_STATUS_QUERY, parameters);
        }

        public async Task<IEnumerable<PreRequisite>> GetAllPreRequisitesAsync()
        {
            return await _dbCommand.ExecuteSelectQueryAsync<PreRequisite>();
        }

        public async Task<bool> AddTrainingAsync(Training training, string preRequisiteIds)
        {
            const string INSERT_TRAINING_QUERY = @"
                    INSERT INTO Training (TrainingName, Description, DeadlineOfApplication, Capacity, DepartmentId, 
                                          TrainingCourseStartingDateTime, IsDeadlineExpired)
                    VALUES (@TrainingName, @Description, @DeadlineOfApplication, @Capacity, @DepartmentId, 
                            @TrainingCourseStartingDateTime, @IsDeadlineExpired)
                    DECLARE @Id INT
                    SET @Id = SCOPE_IDENTITY()
                    IF (@PreRequisiteIds IS NOT NULL AND LEN(@PreRequisiteIds) > 0)
                        BEGIN
                            INSERT INTO TrainingPreRequisites (TrainingId, PreRequisiteId)
                            SELECT @Id, P.value
                            FROM STRING_SPLIT(@PreRequisiteIds, ',') AS P;
                        END";
            return await ExecuteTrainingTransactionAsync(INSERT_TRAINING_QUERY, training, preRequisiteIds);
        }

        public async Task<bool> UpdateTrainingAsync(Training training, string preRequisiteIds)
        {
            const string UPDATE_TRAINING_QUERY = @"
                    UPDATE Training
                    SET TrainingName = @TrainingName,
	                    [Description] = @Description,
	                    DeadlineOfApplication = @DeadlineOfApplication,
	                    Capacity = @Capacity,
	                    DepartmentId = @DepartmentId,
	                    TrainingCourseStartingDateTime = @TrainingCourseStartingDateTime,
	                    IsDeadlineExpired = @IsDeadlineExpired
                    WHERE Id = @Id
                    DELETE FROM TrainingPreRequisites WHERE TrainingId = @Id
                    IF (@PreRequisiteIds IS NOT NULL AND LEN(@PreRequisiteIds) > 0)
                        BEGIN
                            INSERT INTO TrainingPreRequisites (TrainingId, PreRequisiteId)
                            SELECT @Id, P.value
                            FROM STRING_SPLIT(@PreRequisiteIds, ',') AS P;
                        END";
            return await ExecuteTrainingTransactionAsync(UPDATE_TRAINING_QUERY, training, preRequisiteIds, excludeTrainingId: false);
        }

        public async Task<bool> DeleteTrainingAsync(int trainingId)
        {
            const string DELETE_TRAINING_QUERY = @"
                    DECLARE @DeletedApplicationIds TABLE (Id INT);

                    INSERT INTO @DeletedApplicationIds (Id)
                    SELECT Id FROM [Application] WHERE TrainingId = @TrainingId;

                    DELETE FROM TrainingPreRequisites 
                    WHERE TrainingId = @TrainingId;

                    DELETE FROM DocumentUpload 
                    WHERE ApplicationId IN (SELECT Id FROM @DeletedApplicationIds);

                    DELETE FROM [Application] 
                    WHERE TrainingId = @TrainingId;

                    DELETE FROM Training 
                    WHERE Id = @TrainingId;";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { TrainingId = trainingId });
            return await _dbCommand.ExecuteTransactionAsync(new SqlCommand(DELETE_TRAINING_QUERY), parameters);
        }

        public async Task<bool> AreUsersSelectedForTrainingAsync(int trainingId)
        {
            const string IS_ANY_USER_SELECTED_FOR_TRAINING =
                @"SELECT 1 FROM [Application] WHERE TrainingId = @TrainingId";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                TrainingId = trainingId,
            });
            return await _dbCommand.IsRecordPresentAsync(IS_ANY_USER_SELECTED_FOR_TRAINING, parameters);
        }

        // PRIVATE HELPER METHODS
        private async Task<object> RetrieveDepartmentNameAsync(byte departmentId)
        {
            try
            {
                string GET_DEPARTMENT_NAME_QUERY = @"SELECT DepartmentName FROM Department WHERE Id = @DepartmentId";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { DepartmentId = departmentId });

                return await _dbCommand.GetScalerResultAsync(GET_DEPARTMENT_NAME_QUERY, parameters);
            }
            catch (Exception) { throw; }
        }

        private async Task<IEnumerable<TrainingPreRequisteDTO>> RetrieveTrainingPreRequisitesAsync(int trainingId)
        {
            try
            {
                string GET_TRAINING_PRE_REQUISITES_QUERY =
                    $@"SELECT t.Id as TrainingId, p.Id as PreRequisiteId, p.[Name], p.PreRequisiteDescription FROM Training AS t
                       INNER JOIN TrainingPreRequisites
                       ON TrainingPreRequisites.TrainingId = t.Id
                       INNER JOIN PreRequisite AS p
                       ON TrainingPreRequisites.PreRequisiteId = p.Id
                       WHERE t.Id = @Id";

                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Id = trainingId });
                TrainingPreRequisteDTO mapFunction(IDataReader reader)
                {
                    return new TrainingPreRequisteDTO
                    {
                        TrainingId = reader["TrainingId"] == DBNull.Value ? (short)0 : (short)reader["TrainingId"],
                        PreRequisiteId = reader["PreRequisiteId"] == DBNull.Value ? 0 : (int)reader["PreRequisiteId"],
                        PreRequisiteName = reader["Name"]?.ToString(),
                        PreRequisiteDescription = reader["PreRequisiteDescription"]?.ToString()
                    };
                }

                return await _dbCommand.ExecuteSelectQueryAsync(
                    GET_TRAINING_PRE_REQUISITES_QUERY, parameters, mapFunction);
            }
            catch (Exception) { throw; }
        }

        private async Task<bool> ExecuteTrainingTransactionAsync(string query, Training training, string preRequisiteIds, bool excludeTrainingId = true)
        {
            List<string> excludeParameters = excludeTrainingId ? new List<string> { "Id" } : null;
            SqlParameter[] trainingParameters = _dbCommand.GetSqlParametersFromObject(training, excludeParameters);
            SqlParameter[] preRequisiteIdsParameters = _dbCommand.GetSqlParametersFromObject(new { PreRequisiteIds = preRequisiteIds });
            SqlParameter[] allParameters = trainingParameters.Concat(preRequisiteIdsParameters).ToArray();
            return await _dbCommand.ExecuteTransactionAsync(new SqlCommand(query), allParameters);
        }
    }
}


