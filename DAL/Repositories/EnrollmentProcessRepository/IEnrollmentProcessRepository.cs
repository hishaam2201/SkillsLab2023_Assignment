using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.EnrollmentProcessRepository
{
    public interface IEnrollmentProcessRepository
    {
        Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId);
    }
}
