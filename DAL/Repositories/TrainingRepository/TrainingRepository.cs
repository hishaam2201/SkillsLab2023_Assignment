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

        public async Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId)
        {
            List<TrainingDTO> trainingDTOs = new List<TrainingDTO>();
            // TODO: An employee cannot apply for the same training twice (Riss tou training ki so userid pa dn applicsation)
            const string GET_ALL_UNEXPIRED_TRAININGS_QUERY = @"SELECT * FROM Training WHERE IsDeadlineExpired = @IsDeadlineExpired
                                                               ORDER BY 
                                                               CASE 
                                                                 WHEN DepartmentId = @UserDepartmentId THEN 0
                                                                 ELSE 1
                                                               END,
                                                               DepartmentId;";
            SqlParameter[] parameter = _dbCommand.GetSqlParametersFromObject(new { IsDeadlineExpired = 0, UserDepartmentId = userDepartmentId });
            List<Training> trainingList = (await _dbCommand.ExecuteSelectQueryAsync<Training>(GET_ALL_UNEXPIRED_TRAININGS_QUERY, parameter)).ToList();

            foreach (Training training in trainingList)
            {
                object departmentName = await RetrieveDepartmentNameAsync(training.DepartmentId);

                trainingDTOs.Add(new TrainingDTO
                {
                    TrainingId = training.Id,
                    TrainingName = training.TrainingName,
                    DeadlineOfApplication = training.DeadlineOfApplication.ToString("d MMMM, yyyy"),
                    Capacity = training.Capacity,
                    DepartmentName = departmentName?.ToString()
                });
            }
            return trainingDTOs;
        }

        public async Task<TrainingDTO> GetTrainingByIdAsync(int id)
        {
            Training training = await _dbCommand.GetByIdAsync(id);
            object departmentName = await RetrieveDepartmentNameAsync(training.DepartmentId);

            return new TrainingDTO
            {
                TrainingId = training.Id,
                TrainingName = training.TrainingName,
                Description = training.Description,
                Capacity = training.Capacity,
                TrainingCourseStartingDateTime = training.TrainingCourseStartingDateTime.ToString("d MMMM, yyyy 'at' HH:mm"),
                DeadlineOfApplication = training.DeadlineOfApplication.ToString("d MMMM, yyyy"),
                DepartmentName = departmentName?.ToString(),
                PreRequisites = (await RetrieveTrainingPreRequisitesAsync(training.Id)).ToList()
            };
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync()
        {
            List<Training> trainingList = (await _dbCommand.ExecuteSelectQueryAsync<Training>()).ToList();
            List<TrainingDTO> trainingDTOs = new List<TrainingDTO>();

            foreach (Training training in trainingList)
            {
                object departmentName = await RetrieveDepartmentNameAsync(training.DepartmentId);

                trainingDTOs.Add(new TrainingDTO
                {
                    TrainingId = training.Id,
                    TrainingName = training.TrainingName,
                    Description = training.Description,
                    TrainingCourseStartingDateTime = training.TrainingCourseStartingDateTime.ToString("d MMMM, yyyy 'at' HH:mm"),
                    DeadlineOfApplication = training.DeadlineOfApplication.ToString("d MMMM, yyyy"),
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
                @"SELECT 1 FROM [Application] WHERE TrainingId = @TrainingId AND ApplicationStatus = @ApplicationStatus";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new
            {
                TrainingId = trainingId,
                // TODO: To change to selected once functionality is implemented
                ApplicationStatus = ApplicationStatusEnum.Approved.ToString()
            });
            return await _dbCommand.IsRecordPresentAsync(IS_ANY_USER_SELECTED_FOR_TRAINING, parameters);
        }

        // PRIVATE HELPER METHODS
        private async Task<object> RetrieveDepartmentNameAsync(int departmentId)
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
                Func<IDataReader, TrainingPreRequisteDTO> mapFunction = reader =>
                {
                    return new TrainingPreRequisteDTO
                    {
                        TrainingId = reader["TrainingId"] == DBNull.Value ? (short)0 : (short)reader["TrainingId"],
                        PreRequisiteId = reader["PreRequisiteId"] == DBNull.Value ? 0 : (int)reader["PreRequisiteId"],
                        PreRequisiteName = reader["Name"]?.ToString(),
                        PreRequisiteDescription = reader["PreRequisiteDescription"]?.ToString()
                    };
                };

                return await _dbCommand.ExecuteSelectQueryAsync(
                    GET_TRAINING_PRE_REQUISITES_QUERY, parameters, mapFunction);
            }
            catch (Exception) { throw; }
        }
    }
}


