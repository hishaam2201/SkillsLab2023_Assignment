using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public bool ValidateLoginCredentials(string email, string password)
        {
            return _accountRepository.ValidateLoginCredentials(email, password);
        }

        public bool RegisterUser(User user)
        {
            if (!DoesEmailExist(user.Email))
            {
                return _accountRepository.RegisterUser(user);
            }
            else
            {
                return false;
            }
        }

        public bool DoesEmailExist(string email)
        {
            return _accountRepository.DoesEmailExist(email);
        }
    }
}
