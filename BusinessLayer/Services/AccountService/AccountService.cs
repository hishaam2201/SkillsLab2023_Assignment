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
        private const string EMAIL_PROPERTY = "Email";
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<OperationResult> AuthenticateLoginCredentialsAsync(string email, string password)
        {
            OperationResult emailResult = await CheckLoginValuesInUseAsync(EMAIL_PROPERTY, email, "Email is not registered");
            if (emailResult.Success) 
            {
                PasswordDTO passwordDTO = await _accountRepository.GetUserHashedPasswordAndSaltAsync(email);
                if (passwordDTO != null)
                {
                    byte[] hashUserEnteredPassword = HashPassword(password, passwordDTO.Salt);
                    bool isPasswordSame = CompareByteArrays(hashUserEnteredPassword, passwordDTO.HashedPassword);
                    return new OperationResult
                    {
                        Success = isPasswordSame,
                        Message = isPasswordSame ? "Login Successful" : "Invalid Password"
                    };
                }
            }
            return emailResult;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            return await _accountRepository.GetAllDepartmentsAsync();
        }

        public async Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(byte departmentId)
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

        public async Task<OperationResult> RegisterUserAsync(User user, string email, string password)
        {
            const string NATIONAL_IDENTITY_CARD = "NationalIdentityCard";
            const string MOBILE_NUMBER = "MobileNumber";

            OperationResult mobileResult = await CheckRegisterValuesInUseAsync(MOBILE_NUMBER, user.MobileNumber, "Mobile Number is in use");
            if (!mobileResult.Success) return mobileResult;

            OperationResult nicResult = await CheckRegisterValuesInUseAsync(NATIONAL_IDENTITY_CARD, user.NationalIdentityCard, "National Identity Card number is in use");
            if (!nicResult.Success) return nicResult;

            OperationResult emailResult = await CheckRegisterValuesInUseAsync(EMAIL_PROPERTY, email, "Email is in use");
            if (!emailResult.Success) return emailResult;
            
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(password, salt);
            user.Password = hashedPassword;
            user.Salt = salt;

            bool isRegistrationSuccessful = await _accountRepository.RegisterUserAsync(user, email);
            return new OperationResult
            {
                Success = isRegistrationSuccessful,
                Message = isRegistrationSuccessful ? "Registration Successful" : "Could not perform registration"
            };
        }

        // PRIVATE HELPER METHODS
        private async Task<OperationResult> CheckLoginValuesInUseAsync(string propertyName, string value, string message)
        {
            if (!await _accountRepository.IsFieldInUseAsync(propertyName, value))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = message
                };
            }
            return new OperationResult { Success = true };
        }
        private async Task<OperationResult> CheckRegisterValuesInUseAsync(string propertyName, string value, string message)
        {
            if (await _accountRepository.IsFieldInUseAsync(propertyName, value))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = message
                };
            }
            return new OperationResult { Success = true };
        }

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
