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
        Task<bool> ApproveApplicationAsync(int applicationId, string managerName);
        Task<bool> DeclineApplicationAsync(int applicationId, string managerName, string message);
        Task<bool> ProcessEmployeeApplicationAsync(UserDTO userInformation, List<DocumentUploadDTO> enrollmentDataList);
        Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserId(short userId);
    }
}
