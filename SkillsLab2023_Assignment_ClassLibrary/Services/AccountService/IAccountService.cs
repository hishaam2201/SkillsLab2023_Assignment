using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.AccountService
{
    public interface IAccountService : IGenericService<Account>
    {
        bool Login(Account account);
        bool Register(User user);
    }
}
