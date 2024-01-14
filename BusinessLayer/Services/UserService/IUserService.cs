using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.UserService
{
    public interface IUserService
    {
        Task<OperationResult> AuthenticateLoginCredentialsAsync(string email, string password);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(byte departmentId);
        Task<UserDTO> GetUserDataAsync(string email, byte roleId);
        Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email);
        Task<OperationResult> RegisterUserAsync(User user, string password);
    }
}
