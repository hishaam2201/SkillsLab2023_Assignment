using DAL.DTO;
using DAL.Models;
using DAL.Repositories.AccountRepository;
using System.Collections.Generic;

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

        public IEnumerable<DepartmentDTO> GetAllDepartments()
        {
            return _accountRepository.GetAllDepartments();
        }

        public IEnumerable<ManagerDTO> GetAllManagersFromDepartment()
        {
            throw new System.NotImplementedException();
        }

        public bool Register(User user, string email, string password)
        {
            if (EmailExists(email)) return false;

            bool registrationSuccessful = _accountRepository.Register(user, email, password);
            return registrationSuccessful;
        }
    }
}
