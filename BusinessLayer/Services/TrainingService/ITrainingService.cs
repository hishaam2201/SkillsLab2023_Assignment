using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.TrainingService
{
    public interface ITrainingService
    {
        Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId, short userId);
        Task<TrainingDTO> GetTrainingByIdAsync(int trainingId);
        Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync();
        Task PerformAutomaticDeadlineExpiryStatusUpdateAsync();
        Task<OperationResult> GetAllPreRequisitesAsync();
        Task<OperationResult> SaveTrainingAsync(Training training, string preRequisiteIds, bool isUpdate);
        Task<OperationResult> DeleteTrainingAsync(int trainingId);
    }
}
