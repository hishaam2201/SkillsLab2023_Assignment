using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Framework.DatabaseCommand.DatabaseCommand;
using DAL.DTO;
using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.TrainingRepository
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly IDatabaseCommand<Training> _dbCommand;
        public TrainingRepository(IDatabaseCommand<Training> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync()
        {
            List<TrainingDTO> trainingDTOs = new List<TrainingDTO>();
            try
            {
                List<Training> trainingList = (await _dbCommand.ExecuteSelectQueryAsync<Training>()).ToList();

                foreach (Training training in trainingList)
                {
                    object departmentName = await RetrieveDepartmentNameAsync(training.Id);

                    trainingDTOs.Add(new TrainingDTO
                    {
                        TrainingId = training.Id,
                        TrainingName = training.TrainingName,
                        Deadline = training.Deadline.ToString("d MMMM, yyyy"),
                        Capacity = training.Capacity,
                        DepartmentName = departmentName?.ToString()
                    });
                }
            }
            catch (Exception) { throw; }
            return trainingDTOs;
        }

        public async Task<TrainingDTO> GetTrainingByIdAsync(int id)
        {
            try
            {
                Training training = await _dbCommand.GetByIdAsync(id);
                object departmentName = await RetrieveDepartmentNameAsync(training.Id);

                return new TrainingDTO
                {
                    TrainingId = training.Id,
                    TrainingName = training.TrainingName,
                    Description = training.Description,
                    Capacity = training.Capacity,
                    StartingDate = training.StartingDate.ToString("d MMMM, yyyy"),
                    Deadline = training.Deadline.ToString("d MMMM, yyyy"),
                    DepartmentName = departmentName?.ToString(),
                    PreRequisites = (await RetrieveTrainingPreRequisitesAsync(training.Id)).ToList()
                };
            }
            catch (Exception) { throw; }
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
                    $@"SELECT t.Id as TrainingId, p.Id as PreRequisiteId, p.PreRequisiteName FROM Training AS t
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
                        PreRequisiteDescription = reader["PreRequisiteName"]?.ToString()
                    };
                };

                return await _dbCommand.ExecuteSelectQueryAsync(
                    GET_TRAINING_PRE_REQUISITES_QUERY, parameters, mapFunction);
            }
            catch (Exception) { throw; }
        }
    }
}


