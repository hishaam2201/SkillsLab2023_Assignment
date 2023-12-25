using DAL.DTO;
using DAL.Models;
using DAL.Repositories.AccountRepository;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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
            if (await IsEmailInUseAsync(email))
            {
                PasswordDTO passwordDTO = await _accountRepository.GetUserHashedPasswordAndSalt(email);
                if (passwordDTO != null)
                {
                    byte[] hashEnteredPassword = HashPassword(password, passwordDTO.Salt);
                    return CompareByteArrays(hashEnteredPassword, passwordDTO.HashedPassword);
                }
            }
            return false;
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

            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(password, salt);
            user.Password = hashedPassword;
            user.Salt = salt;

            bool isRegistrationSuccessful = await _accountRepository.RegisterUserAsync(user, email);
            return isRegistrationSuccessful;
        }

        // PRIVATE HELPER METHODS
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var sha512 = new SHA512Managed())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                return sha512.ComputeHash(saltedPassword);
            }
        }

        private bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1 == null || array2 == null || array1.Length != array2.Length)
            {
                return false;
            }

            for (int index = 0; index < array1.Length; index++)
            {
                if (array1[index] != array2[index])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
