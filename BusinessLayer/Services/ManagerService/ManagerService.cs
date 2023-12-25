using DAL.DTO;
using DAL.Repositories.ManagerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ManagerService
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;
        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public Task<IEnumerable<PendingApplicationDocumentDTO>> GetApplicationDocumentAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PendingApplicationDTO>> GetApplicationsAsync(short managerId)
        {
            return await _managerRepository.GetApplicationsAsync(managerId);
        }
    }
}
