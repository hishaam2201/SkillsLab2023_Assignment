using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.EnrollmentProcessRepository
{
    public interface IEnrollmentProcessRepository
    {
        Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<IEnumerable<ApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId);
        Task<IEnumerable<SelectionProcessDTO>> GetSelectedUsersForTrainingAsync(short trainingId);
        // TODO: Change to OperationResult for approve and decline application async
        Task<(bool, SendEmailDTO)> ApproveApplicationAsync(int applicationId);
        Task<(bool, SendEmailDTO)> DeclineApplicationAsync(int applicationId, string message);
        Task<IEnumerable<TrainingDTO>> GetAllExpiredTrainingIdsAsync();
        Task<IEnumerable<SelectionProcessDTO>> ProcessUsersSelectionAsync(short trainingId, string declineReason = "");
    }
}
