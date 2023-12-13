using DAL.DTO;
using DAL.Models;
using System.Collections;
using System.Collections.Generic;

namespace BusinessLayer.Services.AccountService
{
    public interface IAccountService
    {
        bool AuthenticateLoginCredentials(string email, string password);
        bool EmailExists(string email);
        bool Register(User user, string email, string password);
        IEnumerable<DepartmentDTO> GetAllDepartments();
        IEnumerable<ManagerDTO> GetAllManagersFromDepartment(int departmentId);
        UserDTO GetUserData(string email);
    }
}
