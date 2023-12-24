

using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.ApplicationRepository
{
    public interface IApplicationRepository
    {
        Task<int> InsertApplicationAndGetId(Application application);
        Task<bool> InsertDocumentUpload(DocumentUpload documentUpload);
    }
}
