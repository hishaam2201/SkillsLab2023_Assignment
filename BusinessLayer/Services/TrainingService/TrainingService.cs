using DAL.DTO;
using DAL.Models;
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

        public async Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId)
        {
            return await _trainingRepository.GetUnappliedTrainingsAsync(userDepartmentId);
        }

        public async Task<TrainingDTO> GetTrainingByIdAsync(int id)
        {
            return await _trainingRepository.GetTrainingByIdAsync(id);
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync()
        {
            return await _trainingRepository.GetAllTrainingsAsync();
        }

        public async Task UpdateDeadlineExpiryStatusAsync()
        {
            await _trainingRepository.UpdateDeadlineExpiryStatusAsync();
        }

        public async Task<OperationResult> DeleteTrainingAsync(int trainingId)
        {
            if (!await AreUsersSelectedForTrainingAsync(trainingId))
            {
                bool isDeleted = await _trainingRepository.DeleteTrainingAsync(trainingId);
                return new OperationResult
                {
                    Success = isDeleted,
                    Messages = isDeleted
                            ? new List<string> { "Training deleted successfully." }
                            : new List<string> { "Failed to delete training" }
                };

            }
            return new OperationResult
            {
                Success = false,
                Messages = { "Users are selected for this training." }
            };
        }

        // Private Helper Method
        private async Task<bool> AreUsersSelectedForTrainingAsync(int trainingId)
        {
            return await _trainingRepository.AreUsersSelectedForTrainingAsync(trainingId);
        }
    }
}
