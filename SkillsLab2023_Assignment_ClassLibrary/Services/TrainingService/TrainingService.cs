using SkillsLab2023_Assignment_ClassLibrary.DTO;
using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.TrainingRepository;
using System;
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

        public IEnumerable<TrainingDTO> GetAllTrainings()
        {
            /*try
            {
                return _trainingRepository.GetAllTrainings();
            }
            catch (Exception) { throw; }*/
            return _trainingRepository.GetAllTrainings();
        }

        public TrainingDTO GetTrainingById(int id)
        {
            try
            {
                return _trainingRepository.GetTrainingById(id);
            }
            catch (Exception) { throw; }
        }
    }
}
