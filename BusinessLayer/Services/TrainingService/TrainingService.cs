using DAL.DTO;
using DAL.Repositories.TrainingRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.TrainingService
{
    public class TrainingService : ITrainingService
    {
        private readonly ITrainingRepository _trainingRepository;
        public TrainingService(ITrainingRepository trainingRepository)
        {
            _trainingRepository = trainingRepository;
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync()
        {
            return await _trainingRepository.GetAllTrainingsAsync();
        }

        public async Task<TrainingDTO> GetTrainingByIdAsync(int id)
        {
            return await _trainingRepository.GetTrainingByIdAsync(id);
        }
    }
}
