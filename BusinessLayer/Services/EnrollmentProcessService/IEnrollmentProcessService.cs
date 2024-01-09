using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public interface IEnrollmentProcessService
    {
        Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<OperationResult> GetApplicationDocumentAsync(int applicationId);
        Task<bool> ApproveApplicationAsync(int applicationId, string managerName);
        Task<bool> DeclineApplicationAsync(int applicationId, string managerName, string message);
        Task PerformAutomaticSelectionProcessAsync();
        Task<OperationResult> PerformManualSelectionProcessAsync(short trainingId);
        Task<SelectedProcessUserDTO> GetSelectedUsersForTrainingAsync(short trainingId);
        Task<byte[]> ExportToExcel(short trainingId);
    }
}
