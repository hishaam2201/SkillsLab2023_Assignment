using Framework.HelperClasses;
using DAL.DTO;
using DAL.Models;
using DAL.Repositories.UserRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.UserService
{
    public class UserService : IUserService
    {
        private const string EMAIL_PROPERTY = "Email";
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository accountRepository)
        {
            _userRepository = accountRepository;
        }

        public async Task<OperationResult> AuthenticateLoginCredentialsAsync(string email, string password)
        {
            OperationResult emailResult = await CheckLoginValuesInUseAsync(EMAIL_PROPERTY, email, "Email is not registered");
            if (emailResult.Success) 
            {
                PasswordDTO passwordDTO = await _userRepository.GetUserHashedPasswordAndSaltAsync(email);
                if (passwordDTO != null)
                {

                    byte[] hashUserEnteredPassword = PasswordHashing.HashPassword(password, passwordDTO.Salt);
                    bool isPasswordSame = PasswordHashing.CompareByteArrays(hashUserEnteredPassword, passwordDTO.HashedPassword);

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
            return await _userRepository.GetAllDepartmentsAsync();
        }

        public async Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(byte departmentId)
        {
            return await _userRepository.GetAllManagersFromDepartmentAsync(departmentId);
        }

        public async Task<UserDTO> GetUserDataAsync(string email, byte roleId)
        {
            return await _userRepository.GetUserDataAsync(email, roleId);
        }

        public async Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email)
        {
            return await _userRepository.GetUserRolesAsync(email);
        }

        public async Task<OperationResult> RegisterUserAsync(User user, string password)
        {
            const string NATIONAL_IDENTITY_CARD = "NationalIdentityCard";
            const string MOBILE_NUMBER = "MobileNumber";

            OperationResult mobileResult = await CheckRegisterValuesInUseAsync(MOBILE_NUMBER, user.MobileNumber, "Mobile Number is in use");
            if (!mobileResult.Success) return mobileResult;

            OperationResult nicResult = await CheckRegisterValuesInUseAsync(NATIONAL_IDENTITY_CARD, user.NationalIdentityCard, "National Identity Card number is in use");
            if (!nicResult.Success) return nicResult;

            OperationResult emailResult = await CheckRegisterValuesInUseAsync(EMAIL_PROPERTY, user.Email, "Email is in use");
            if (!emailResult.Success) return emailResult;
            
            byte[] salt = PasswordHashing.GenerateSalt();
            byte[] hashedPassword = PasswordHashing.HashPassword(password, salt);
            user.Salt = salt;
            user.Password = hashedPassword;

            bool isRegistrationSuccessful = await _userRepository.RegisterUserAsync(user);
            return new OperationResult
            {
                Success = isRegistrationSuccessful,
                Message = isRegistrationSuccessful ? "Registration Successful" : "Could not perform registration"
            };
        }


        // PRIVATE HELPER METHODS
        private async Task<OperationResult> CheckLoginValuesInUseAsync(string propertyName, string value, string message)
        {
            if (!await _userRepository.IsFieldInUseAsync(propertyName, value))
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
            if (await _userRepository.IsFieldInUseAsync(propertyName, value))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = message
                };
            }
            return new OperationResult { Success = true };
        }
    }
}
