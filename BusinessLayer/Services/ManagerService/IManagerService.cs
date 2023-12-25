using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ManagerService
{
    public interface IManagerService
    {
        Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId);
        Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync();
    }
}
