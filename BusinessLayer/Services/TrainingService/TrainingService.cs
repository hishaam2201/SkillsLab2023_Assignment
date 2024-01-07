using DAL.DTO;
using DAL.Models;
using DAL.Repositories.TrainingRepository;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId, short userId)
        {
            return await _trainingRepository.GetUnappliedTrainingsAsync(userDepartmentId, userId);
        }

        public async Task<TrainingDTO> GetTrainingByIdAsync(int id)
        {
            return await _trainingRepository.GetTrainingByIdAsync(id);
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync()
        {
            return await _trainingRepository.GetAllTrainingsAsync();
        }

        public async Task PerformAutomaticDeadlineExpiryStatusUpdateAsync()
        {
            await _trainingRepository.UpdateDeadlineExpiryStatusAsync();
        }

        public async Task<OperationResult> GetAllPreRequisitesAsync()
        {
            IEnumerable<PreRequisite> preRequisites = (await _trainingRepository.GetAllPreRequisitesAsync());
            return new OperationResult
            {
                Success = preRequisites != null && preRequisites.Any(),
                ListOfData = preRequisites
            };
        }

        public async Task<OperationResult> SaveTrainingAsync(Training training, string preRequisitesIds, bool isUpdate)
        {
            preRequisitesIds = preRequisitesIds ?? string.Empty;
            Func<Training, string, Task<bool>> operation = isUpdate
                ? new Func<Training, string, Task<bool>>(_trainingRepository.UpdateTrainingAsync)
                : new Func<Training, string, Task<bool>>(_trainingRepository.AddTrainingAsync);

            var result = await operation(training, preRequisitesIds);
            return new OperationResult
            {
                Success = result,
                Message = result
                    ? $"Training {(isUpdate ? "updated" : "added")} successfully."
                    : $"An error occurred while {(isUpdate ? "updating" : "adding")} training."
            };
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
