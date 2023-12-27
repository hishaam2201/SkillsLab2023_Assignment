using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public interface IEnrollmentProcessService
    {
        Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<IEnumerable<ApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId);
        Task<bool> ApproveApplicationAsync(int applicationId, string managerName);
        Task<bool> DeclineApplicationAsync(int applicationId, string managerName, string message);
    }
}
