using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(IDataAccessLayer dataAccessLayer) : base(dataAccessLayer)
        {

        }
    }
}
