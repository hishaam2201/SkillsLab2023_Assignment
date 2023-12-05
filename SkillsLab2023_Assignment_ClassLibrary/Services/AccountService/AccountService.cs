using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository;
using System;

namespace SkillsLab2023_Assignment_ClassLibrary.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public bool AuthenticateLoginCredentials(string email, string password)
        {
            return _accountRepository.AuthenticateLoginCredentials(email, password);
        }

        public bool EmailExists(string email)
        {
            return _accountRepository.EmailExists(email);
        }

        public bool Register(User user)
        {
            try
            {
                return EmailExists(user.Email) ? false : _accountRepository.Register(user);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
