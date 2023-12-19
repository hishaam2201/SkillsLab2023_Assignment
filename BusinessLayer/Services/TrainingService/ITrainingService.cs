using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.TrainingService
{
    public interface ITrainingService
    {
        Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync();
        Task<TrainingDTO> GetTrainingByIdAsync(int id);
    }
}
