using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.ApplicationRepository
{
    public interface IApplicationRepository
    {
        Task<(bool, SendEmailDTO)> ApproveApplicationAsync(int applicationId);
        Task<(bool, SendEmailDTO)> DeclineApplicationAsync(int applicationId, string message);
        Task<IEnumerable<ApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<IEnumerable<ApplicationDocumentDTO>> GetApplicationDocumentAsync(int applicationId);
        Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserIdAsync(short userId);
        Task<bool> InsertIntoApplicationAsync(short userId, short trainingId);
        Task<bool> ProcessEmployeeApplicationAsync(short userId, short trainingId, List<DocumentUpload> documentUploads);
    }
}
