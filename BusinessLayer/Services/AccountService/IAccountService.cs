using DAL.DTO;
using DAL.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.AccountService
{
    public interface IAccountService
    {
        Task<bool> AuthenticateLoginCredentialsAsync(string email, string password);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(int departmentId);
        Task<UserDTO> GetUserDataAsync(string email);
        Task<bool> IsEmailInUseAsync(string email);
        Task<bool> RegisterUserAsync(User user, string email, string password);
    }
}
