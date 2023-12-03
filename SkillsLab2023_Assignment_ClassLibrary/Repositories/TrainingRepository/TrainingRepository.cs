using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;
using System;
using System.Collections.Generic;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly IDatabaseCommand _dbCommand;
        public TrainingRepository(IDatabaseCommand dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public IEnumerable<Training> GetAllTrainings()
        {
            throw new NotImplementedException();
        }
    }
}
