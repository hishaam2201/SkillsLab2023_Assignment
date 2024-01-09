using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.ApplicationRepository
{
    public interface IApplicationRepository
    {
        Task<int> InsertApplicationAndGetIdAsync(Application application);
        Task<bool> InsertDocumentUploadAsync(DocumentUpload documentUpload);
        Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserIdAsync(short userId);
    }
}
