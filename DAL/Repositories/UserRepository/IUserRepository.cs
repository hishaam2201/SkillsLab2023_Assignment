using DAL.DTO;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<PasswordDTO> GetUserHashedPasswordAndSaltAsync(string email);
        Task<bool> IsFieldInUseAsync(string columnName, string columnValue);
        Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email);
        Task<UserDTO> GetUserDataAsync(string email, byte roleId);
        Task<bool> RegisterUserAsync(User user);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(byte departmentId);
    }
}
