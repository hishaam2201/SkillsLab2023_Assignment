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

        public async Task<TrainingDTO> GetTrainingByIdAsync(int trainingId)
        {
            return await _trainingRepository.GetTrainingByIdAsync(trainingId);
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
            var preRequisites = (await _trainingRepository.GetAllPreRequisitesAsync());
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

            var isSuccessful = await operation(training, preRequisitesIds);
            return new OperationResult
            {
                Success = isSuccessful,
                Message = isSuccessful
                    ? $"Training {(isUpdate ? "updated" : "added")} successfully."
                    : $"An error occurred while {(isUpdate ? "updating" : "adding")} training."
            };
        }

        public async Task<OperationResult> DeleteTrainingAsync(int trainingId)
        {
            if (await AreUsersSelectedForTrainingAsync(trainingId))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Employees have applied for this training."
                };
            }
            bool isDeleted = await _trainingRepository.DeleteTrainingAsync(trainingId);
            return new OperationResult
            {
                Success = isDeleted,
                Message = isDeleted ? "Training deleted successfully." : "Failed to delete training."
            };
        }

        // Private Helper Method
        private async Task<bool> AreUsersSelectedForTrainingAsync(int trainingId)
        {
            return await _trainingRepository.AreUsersSelectedForTrainingAsync(trainingId);
        }
    }
}
