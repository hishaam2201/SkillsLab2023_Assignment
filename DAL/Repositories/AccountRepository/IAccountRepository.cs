using DAL.DTO;
using DAL.Models;
using System.Collections;
using System.Collections.Generic;

namespace DAL.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        int GetRoleId(string role); // TODO: Change to not take role as parameter
        bool Register(User user, string email, string password);
        IEnumerable<DepartmentDTO> GetAllDepartments();
        IEnumerable<ManagerDTO> GetAllManagersFromDepartment();
    }
}
