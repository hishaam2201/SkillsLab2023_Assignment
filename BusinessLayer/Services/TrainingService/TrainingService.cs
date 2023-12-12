using DAL.DTO;
using DAL.Repositories.TrainingRepository;
using System.Collections.Generic;


namespace BusinessLayer.Services.TrainingService
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
            return _trainingRepository.GetAllTrainings();
        }

        public TrainingDTO GetTrainingById(int id)
        {
            return _trainingRepository.GetTrainingById(id);
        }
    }
}
