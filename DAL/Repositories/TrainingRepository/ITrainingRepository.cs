using DAL.DTO;
using System.Collections.Generic;


namespace DAL.Repositories.TrainingRepository
{
    public interface ITrainingRepository
    {
        IEnumerable<TrainingDTO> GetAllTrainings();
        TrainingDTO GetTrainingById(int id);
    }
}
