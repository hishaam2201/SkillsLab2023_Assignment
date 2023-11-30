using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private IDataAccessLayer _dataAccessLayer;

        public ApplicationRepository(IDataAccessLayer dataAccessLayer) 
        {
            _dataAccessLayer = dataAccessLayer;
        }

    }

}
