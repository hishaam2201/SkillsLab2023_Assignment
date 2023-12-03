using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository;


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
            return !EmailExists(user.Email) ? _accountRepository.Register(user) : false;
        }
    }
}
