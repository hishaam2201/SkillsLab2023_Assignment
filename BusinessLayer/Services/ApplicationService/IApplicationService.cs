using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public interface IApplicationService
    {
        Task<string> Upload(Stream stream, string fileName);
    }
}
