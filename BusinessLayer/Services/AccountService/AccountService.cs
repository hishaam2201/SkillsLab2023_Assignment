using DAL.Models;
using DAL.Repositories.AccountRepository;


namespace BusinessLayer.Services.AccountService
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
            if (EmailExists(user.Email)) return false;

            bool registrationSuccessful = _accountRepository.Register(user);
            return registrationSuccessful;
        }
    }
}
