using DAL.DTO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public interface IApplicationService
    {
        Task<bool> ProcessApplication(List<DocumentUploadDTO> documentUploads);
    }
}
