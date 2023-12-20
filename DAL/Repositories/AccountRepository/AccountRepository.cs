using System.Collections.Generic;
using System.Data.SqlClient;
using Framework.DatabaseCommand.DatabaseCommand;
using System;
using DAL.Models;
using System.Linq;
using DAL.DTO;
using Framework.Enums;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDatabaseCommand<Account> _dbCommand;
        public AccountRepository(IDatabaseCommand<Account> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<bool> AuthenticateLoginCredentialsAsync(string email, string password)
        {
            try
            {
                const string AUTHENTICATE_LOGIN_CREDENTIALS_QUERY = @"SELECT 1 FROM [User] WHERE Email=@Email 
                                                                    And Password=@Password";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email, Password = password });
                bool isPresent =  await _dbCommand.IsRecordPresentAsync(AUTHENTICATE_LOGIN_CREDENTIALS_QUERY, parameters);
                return isPresent;
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> IsEmailInUseAsync(string email)
        {
            try
            {
                const string EMAIL_EXISTS_QUERY = @"SELECT 1 FROM [User] WHERE Email=@Email";
                SqlParameter[] emailExistsParams = _dbCommand.GetSqlParametersFromObject(new { Email = email });

                bool isPresent = await _dbCommand.IsRecordPresentAsync(EMAIL_EXISTS_QUERY, emailExistsParams);
                return isPresent;
            }
            catch (Exception) { throw; }
        }

        public async Task<UserDTO> GetUserDataAsync(string email, byte roleId)
        {
            try
            {
                const string GET_USER_DATA_QUERY = @"SELECT Id, FirstName, LastName, DepartmentId, ManagerId, Email, u.[RoleId] FROM [User]
                                                     INNER JOIN UserRole u ON u.UserId = [User].Id
                                                     WHERE Email = @Email AND RoleId = @RoleId";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email, RoleId = roleId });
                Func<IDataReader, UserDTO> mapFunction = reader =>
                {
                    return new UserDTO
                    {
                        Id = (short)reader["Id"],
                        FirstName = reader["FirstName"]?.ToString(),
                        LastName = reader["LastName"]?.ToString(),
                        DepartmentId = reader["DepartmentId"] == DBNull.Value ? (byte?)null : (byte)reader["DepartmentId"],
                        ManagerId = reader["ManagerId"] == DBNull.Value ? (short?)null : (short)reader["ManagerId"],
                        Email = reader["Email"]?.ToString(),
                        RoleId = (byte)reader["RoleId"],
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_USER_DATA_QUERY, parameters, mapFunction);
                return result.FirstOrDefault();
            }
            catch (Exception) { throw; }
        }
        public async Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync(string email)
        {
            try
            {
                const string GET_USER_ROLES_QUERY = @"SELECT u.RoleId, r.RoleName FROM [User]
                                                      INNER JOIN UserRole AS u
                                                      ON u.UserId = [User].Id
                                                      INNER JOIN [Role] AS r
                                                      ON r.Id = u.RoleId
                                                      WHERE Email = @Email";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email });
                Func<IDataReader, UserRoleDTO> mapFunction = reader =>
                {
                    return new UserRoleDTO
                    {
                        RoleId = (byte)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString()
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_USER_ROLES_QUERY, parameters, mapFunction);
                return result;
            }
            catch (Exception) { throw; }
        }

        // TODO: Change to work with new db
        public async Task<bool> RegisterUserAsync(User user, string email, string password)
        {
            try
            {
                const string INSERT_INTO_USER_AND_ACCOUNT_QUERY =
                      @"INSERT INTO [User] 
                      (FirstName, LastName, MobileNumber, NationalIdentityCard, DepartmentId, ManagerId)
                      VALUES (@FirstName, @LastName, @MobileNumber, @NationalIdentityCard, @DepartmentId, @ManagerId); 
                  
                      DECLARE @UserId INT
                      SET @UserId = SCOPE_IDENTITY()

                      INSERT INTO Account (Email, [Password], UserId) VALUES (@Email, @Password, @UserId)";
                List<string> excludedUserProperties = new List<string> { "UserId" };
                SqlParameter[] userQueryParams = _dbCommand.GetSqlParametersFromObject(user, excludedUserProperties);
                SqlParameter[] queryParams = userQueryParams.Concat(new[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Password", password)
                }).ToArray();
                
                bool isSuccessful = await _dbCommand.ExecuteTransactionAsync
                    (new SqlCommand(INSERT_INTO_USER_AND_ACCOUNT_QUERY), queryParams);
                return isSuccessful;
            }
            catch(Exception) { throw; }
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            try
            {
                string GET_ALL_DEPARTMENTS_QUERY = $@"SELECT * FROM Department;";
                Func<IDataReader, DepartmentDTO> mapFunction = reader =>
                {
                    return new DepartmentDTO
                    {
                        Id = (byte)reader["Id"],
                        DepartmentName = reader["DepartmentName"]?.ToString(),
                    };
                };
                return await _dbCommand.ExecuteSelectQueryAsync(query: GET_ALL_DEPARTMENTS_QUERY, mapFunction: mapFunction);
            }
            catch (Exception) { throw; }
        }

        public async Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(int departmentId)
        {
            try
            {
                string GET_MANAGERS_FROM_DEPARTMENT_QUERY =
                    $@"SELECT Id, FirstName, LastName FROM [User] WHERE RoleId = {(int)RoleEnum.Manager}
                               AND DepartmentId = @DepartmentId";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { DepartmentId = departmentId });
                Func<IDataReader, ManagerDTO> mapFunction = reader =>
                {
                    return new ManagerDTO
                    {
                        Id = (short)reader["Id"],
                        FirstName = reader["FirstName"]?.ToString(),
                        LastName = reader["LastName"]?.ToString(),
                    };
                };
                return await _dbCommand.ExecuteSelectQueryAsync
                    (GET_MANAGERS_FROM_DEPARTMENT_QUERY, parameters, mapFunction);
            }
            catch (Exception) { throw; }
        }
    }
}
