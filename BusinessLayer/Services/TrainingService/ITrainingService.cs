using DAL.DTO;
using System.Collections.Generic;


namespace BusinessLayer.Services.TrainingService
{
    public interface ITrainingService
    {
        IEnumerable<TrainingDTO> GetAllTrainings();
        TrainingDTO GetTrainingById(int id);
    }
}
