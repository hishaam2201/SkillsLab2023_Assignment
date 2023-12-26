using DAL.DTO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public interface IEnrollmentProcessService
    {
        Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId);
    }
}
