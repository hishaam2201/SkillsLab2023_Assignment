

using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.ApplicationRepository
{
    public interface IApplicationRepository
    {
        Task<bool> ApplyForTraining(Application application, DocumentUpload document);
    }
}
