using DAL.DTO;
using DAL.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Task<bool> AuthenticateLoginCredentialsAsync(string email, string password);
        Task<bool> IsEmailInUseAsync(string email);
        Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email);
        Task<UserDTO> GetUserDataAsync(string email, byte roleId);
        Task<bool> RegisterUserAsync(User user, string email, string password);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(int departmentId);
    }
}
