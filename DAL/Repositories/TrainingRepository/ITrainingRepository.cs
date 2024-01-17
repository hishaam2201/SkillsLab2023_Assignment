using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId, short userId);
        Task<TrainingDTO> GetTrainingByIdAsync(int trainingId);
        Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync();
        Task UpdateDeadlineExpiryStatusAsync();
        Task<bool> HaveUsersAppliedForTrainingAsync(int trainingId);
        Task<IEnumerable<PreRequisite>> GetAllPreRequisitesAsync();
        Task<bool> AddTrainingAsync(Training training, string preRequisiteIds);
        Task<bool> UpdateTrainingAsync(Training training, string preRequisiteIds);
        Task<bool> DeleteTrainingAsync(int trainingId);
    }
}
