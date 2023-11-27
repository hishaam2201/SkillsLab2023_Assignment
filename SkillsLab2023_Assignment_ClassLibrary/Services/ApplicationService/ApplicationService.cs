using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository;
using SkillsLab2023_Assignment_ClassLibrary.Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.ApplicationService
{
    public class ApplicationService : GenericService<Application>, IApplicationService
    {
        public ApplicationService(IGenericRepository<Application> repository) : base(repository)
        {

        }
    }
}
