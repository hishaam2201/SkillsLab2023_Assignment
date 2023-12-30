using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId);
        Task<TrainingDTO> GetTrainingByIdAsync(int id);
        Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync();
        Task UpdateDeadlineExpiryStatusAsync();
        Task<bool> AreUsersSelectedForTrainingAsync(int trainingId);
        Task<bool> DeleteTrainingAsync(int trainingId);
    }
}
