using DAL.DTO;
using DAL.Models;
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
        Task<IEnumerable<PreRequisite>> GetAllPreRequisites();
        Task<bool> DeleteTrainingAsync(int trainingId);
    }
}
