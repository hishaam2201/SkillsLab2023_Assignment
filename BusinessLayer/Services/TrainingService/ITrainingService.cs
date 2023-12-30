using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.TrainingService
{
    public interface ITrainingService
    {
        Task<IEnumerable<TrainingDTO>> GetUnappliedTrainingsAsync(byte userDepartmentId);
        Task<TrainingDTO> GetTrainingByIdAsync(int id);
        Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync();
        Task UpdateDeadlineExpiryStatusAsync();
        Task<OperationResult> DeleteTrainingAsync(int trainingId);
    }
}
