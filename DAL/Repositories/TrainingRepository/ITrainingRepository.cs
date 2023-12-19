using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<TrainingDTO>> GetAllTrainingsAsync();
        Task<TrainingDTO> GetTrainingByIdAsync(int id);
    }
}
