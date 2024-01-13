using DAL.DTO;
using DAL.Models;
using System.Threading.Tasks;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public interface IEnrollmentProcessService
    {
        Task PerformAutomaticSelectionProcessAsync();
        Task<OperationResult> PerformManualSelectionProcessAsync(short trainingId);
        Task<SelectionProcessDTO> GetSelectedUsersForTrainingAsync(short trainingId);
        Task<byte[]> ExportToExcel(short trainingId);
    }
}
