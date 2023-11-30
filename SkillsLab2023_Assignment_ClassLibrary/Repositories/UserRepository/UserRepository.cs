using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataAccessLayer _dataAccessLayer;
        public UserRepository(IDataAccessLayer dataAccessLayer) 
        {
            _dataAccessLayer = dataAccessLayer;
        }
    }
}
