using SkillsLab2023_Assignment_ClassLibrary.DTO;
using System.Collections.Generic;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        IEnumerable<TrainingDTO> GetAllTrainings();
        TrainingDTO GetTrainingById(int id);
    }
}
