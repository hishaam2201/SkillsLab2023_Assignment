using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private IDatabaseCommand<Application> _dbCommand;
        public ApplicationRepository(IDatabaseCommand<Application> dbCommand) 
        {
            _dbCommand = dbCommand;
        }
    }

}
