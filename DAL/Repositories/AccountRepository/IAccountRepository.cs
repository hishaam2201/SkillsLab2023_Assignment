﻿using DAL.DTO;
using DAL.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Task<PasswordDTO> GetUserHashedPasswordAndSalt(string email);
        Task<bool> IsEmailInUseAsync(string email);
        Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email);
        Task<UserDTO> GetUserDataAsync(string email, byte roleId);
        Task<bool> RegisterUserAsync(User user, string email);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(int departmentId);
    }
}
