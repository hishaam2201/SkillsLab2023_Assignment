using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<OperationResult> GetApplicationDocumentAsync(int applicationId);
        Task<OperationResult> ApproveApplicationAsync(int applicationId, string managerName);
        Task<OperationResult> DeclineApplicationAsync(int applicationId, string managerName, string message);
        Task<OperationResult> ProcessEmployeeApplicationAsync(UserDTO userInformation, List<DocumentUploadDTO> enrollmentDataList);
        Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserId(short userId);
    }
}
