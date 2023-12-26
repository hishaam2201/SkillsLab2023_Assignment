using DAL.DTO;
using DAL.Repositories.EnrollmentProcessRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.EnrollmentProcessService
{
    public class EnrollmentProcessService : IEnrollmentProcessService
    {
        private readonly IEnrollmentProcessRepository _enrollmentProcessRepository;
        public EnrollmentProcessService(IEnrollmentProcessRepository enrollmentProcessRepository)
        {
            _enrollmentProcessRepository = enrollmentProcessRepository;
        }

        public async Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId)
        {
            return await _enrollmentProcessRepository.GetApplicationDocumentAsync(applicationId);
        }

        public async Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            return await _enrollmentProcessRepository.GetApplicationsAsync(managerId);
        }


    }
}
