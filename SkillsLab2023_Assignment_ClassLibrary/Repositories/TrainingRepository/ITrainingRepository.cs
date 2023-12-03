using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System.Collections.Generic;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        IEnumerable<Training> GetAllTrainings();
    }
}
