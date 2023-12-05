using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;
using System;
using System.Collections.Generic;
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

        public IEnumerable<Training> GetAllTrainings()
        {
            List<Training> trainingList = _dbCommand.GetAll().ToList();

            foreach(Training training in trainingList)
            {
                int departmentId = training.DepartmentId;

                string getDepartmentNameQuery = @"SELECT DepartmentName FROM Department WHERE Id = @DepartmentId";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { DepartmentId = departmentId });

                object departmentName = _dbCommand.ReturnFirstColumnOfFirstRow(getDepartmentNameQuery, parameters);

                training.DepartmentName = departmentName?.ToString();
            }

            return trainingList;
        }

        // Use DTO to send to frontend
    }
}
