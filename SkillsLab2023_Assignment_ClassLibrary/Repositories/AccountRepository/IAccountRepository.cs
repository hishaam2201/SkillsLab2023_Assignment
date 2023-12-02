using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        bool Register(User user);
    }
}
