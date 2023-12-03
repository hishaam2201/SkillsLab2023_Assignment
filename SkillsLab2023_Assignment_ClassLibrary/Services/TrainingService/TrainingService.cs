using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository;
using System.Collections.Generic;


namespace SkillsLab2023_Assignment_ClassLibrary.Services.TrainingService
{
    public class TrainingService : ITrainingService
    {
        private readonly ITrainingRepository _trainingRepository;
        public TrainingService(ITrainingRepository trainingRepository)
        {
            _trainingRepository = trainingRepository;
        }

        public IEnumerable<Training> GetAllTrainings()
        {
            return _trainingRepository.GetAllTrainings();
        }
    }
}
