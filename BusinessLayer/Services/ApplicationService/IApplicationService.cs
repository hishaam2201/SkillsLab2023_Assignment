using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public interface IApplicationService
    {
        Task<bool> ProcessApplicationAsync(List<DocumentUploadDTO> documentUploads);
        Task<IEnumerable<UserApplicationDTO>> GetApplicationByUserId(short userId);
    }
}
