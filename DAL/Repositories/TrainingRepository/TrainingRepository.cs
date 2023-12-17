﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Framework.DatabaseCommand.DatabaseCommand;
using DAL.DTO;
using DAL.Models;


namespace DAL.Repositories.TrainingRepository
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
                        TrainingName = training.TrainingName,
                        Deadline = training.Deadline.ToString("d MMMM, yyyy"),
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
                    TrainingId = training.Id,
                    TrainingName = training.TrainingName,
                    Description = training.Description,
                    Capacity = training.Capacity,
                    StartingDate = training.StartingDate.ToString("d MMMM, yyyy"),
                    Deadline = training.Deadline.ToString("d MMMM, yyyy"),
                    DepartmentName = departmentName?.ToString(),
                    PreRequisites = GetTrainingPreRequisites(training.Id)
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

        private List<TrainingPreRequisteDTO> GetTrainingPreRequisites(int trainingId)
        {
            try
            {
                string GET_TRAINING_PRE_REQUISITES_QUERY =
                    $@"SELECT t.Id, p.PreRequisiteName FROM Training AS t
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
                        TrainingId = reader["Id"] == DBNull.Value ? (short)0 : (short)reader["Id"],
                        PreRequisiteDescription = reader["PreRequisiteName"]?.ToString()
                    };
                };

                return _dbCommand.ExecuteSelectQuery
                    (GET_TRAINING_PRE_REQUISITES_QUERY, parameters, mapFunction).ToList();
            }
            catch (Exception) { throw; }
        }
    }
}


