using DAL.DTO;
using DAL.Models;
using DAL.Repositories.AccountRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> AuthenticateLoginCredentialsAsync(string email, string password)
        {
            return await _accountRepository.AuthenticateLoginCredentialsAsync(email, password);
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            return await _accountRepository.GetAllDepartmentsAsync();
        }

        public async Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(int departmentId)
        {
            return await _accountRepository.GetAllManagersFromDepartmentAsync(departmentId);
        }

        public async Task<UserDTO> GetUserDataAsync(string email, byte roleId)
        {
            return await _accountRepository.GetUserDataAsync(email, roleId);
        }

        public async Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email)
        {
            return await _accountRepository.GetUserRolesAsync(email);
        }

        public async Task<bool> IsEmailInUseAsync(string email)
        {
            return await _accountRepository.IsEmailInUseAsync(email);
        }

        public async Task<bool> RegisterUserAsync(User user, string email, string password)
        {
            if (await IsEmailInUseAsync(email)) return false;

            bool isRegistrationSuccessful = await _accountRepository.RegisterUserAsync(user, email, password);
            return isRegistrationSuccessful;
        }
    }
}
