using SkillsLab2023_Assignment_ClassLibrary.DTO;
using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly IDatabaseCommand<Training> _dbCommand;
        public TrainingRepository(IDatabaseCommand<Training> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public IEnumerable<TrainingDTO> GetAllTrainings()
        {
            List<TrainingDTO> trainingDTOs = new List<TrainingDTO>();
            try
            {
                List<Training> trainingList = _dbCommand.ExecuteSelectQuery<Training>().ToList();

                foreach (Training training in trainingList)
                {
                    object departmentName = GetDepartmentNameFromTrainingId(training.Id);

                    trainingDTOs.Add(new TrainingDTO
                    {
                        TrainingId = training.Id,
                        ProgrammeName = training.ProgrammeName,
                        Deadline = training.Deadline,
                        Capacity = training.Capacity,
                        DepartmentName = departmentName?.ToString()

                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return trainingDTOs;
        }

        public TrainingDTO GetTrainingById(int id)
        {
            try
            {
                Training training = _dbCommand.GetById(id);
                object departmentName = GetDepartmentNameFromTrainingId(training.Id);

                return new TrainingDTO
                {
                    ProgrammeName = training.ProgrammeName,
                    Description = training.Description,
                    Capacity = training.Capacity,
                    StartingDate = training.StartingDate,
                    Deadline = training.Deadline,
                    DepartmentName = departmentName?.ToString(),
                    PreRequistes = GetTrainingPreRequisites(training.Id)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        // PRIVATE HELPER METHODS
        private object GetDepartmentNameFromTrainingId(int departmentId)
        {
            try
            {
                string GET_DEPARTMENT_NAME_QUERY = @"SELECT DepartmentName FROM Department WHERE Id = @DepartmentId";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { DepartmentId = departmentId });

                return _dbCommand.ReturnFirstColumnOfFirstRow(GET_DEPARTMENT_NAME_QUERY, parameters);
            }
            catch (Exception) { throw; }
        }

        private List<PreRequiste> GetTrainingPreRequisites(int trainingId)
        {
            try
            {
                string GET_TRAINING_PRE_REQUISITES_QUERY =
                    $@"SELECT t.Id, p.[description] FROM Training AS t
                       INNER JOIN TrainingPreRequisites
                       ON TrainingPreRequisites.TrainingId = t.Id
                       INNER JOIN PreRequisite AS p
                       ON TrainingPreRequisites.PreRequisiteId = p.Id
                       WHERE t.Id = @Id";

                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Id = trainingId });
                Func<IDataReader, PreRequiste> mapFunction = reader =>
                {
                    return new PreRequiste
                    {
                        Id = (int) reader["Id"],
                        Description = reader["Description"].ToString()
                    };
                };

                return _dbCommand.ExecuteSelectQuery
                    (GET_TRAINING_PRE_REQUISITES_QUERY, parameters, mapFunction).ToList();
            }
            catch (Exception) { throw; }
        }
    }
}


