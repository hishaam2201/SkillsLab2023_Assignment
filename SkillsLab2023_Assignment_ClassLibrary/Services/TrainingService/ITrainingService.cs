using SkillsLab2023_Assignment_ClassLibrary.DTO;
using System.Collections.Generic;


namespace SkillsLab2023_Assignment_ClassLibrary.Services.TrainingService
{
    public interface ITrainingService
    {
        IEnumerable<TrainingDTO> GetAllTrainings();
        TrainingDTO GetTrainingById(int id);
    }
}
