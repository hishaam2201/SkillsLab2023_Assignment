using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.EnrollmentProcessRepository
{
    public interface IEnrollmentProcessRepository
    {
        Task<bool> SelectionAlreadyDoneForTodayAsync(short trainingId);
        Task<IEnumerable<SelectedUserDTO>> GetSelectedUsersForTrainingAsync(short trainingId);
        Task<IEnumerable<TrainingDTO>> GetAllExpiredTrainingIdsAsync();
        Task<IEnumerable<SelectedUserDTO>> ProcessUsersSelectionAsync(short trainingId, string declineReason = "");
        Task<IEnumerable<ExportSelectedEmployeeDTO>> GetSelectedUserDetailsForExportAsync(short trainingId);
    }
}
