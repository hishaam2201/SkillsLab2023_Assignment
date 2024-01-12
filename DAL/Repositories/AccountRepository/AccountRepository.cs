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
using System.Net.NetworkInformation;

namespace DAL.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDatabaseCommand<User> _dbCommand;
        public AccountRepository(IDatabaseCommand<User> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public async Task<PasswordDTO> GetUserHashedPasswordAndSaltAsync(string email)
        {
            try
            {
                const string GET_USER_HASHEDPASSWORD_AND_SALT_QUERY =
                    @"SELECT HashedPassword, Salt FROM [User] WHERE Email = @Email";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email });
                Func<IDataReader, PasswordDTO> mapFunction = reader =>
                {
                    return new PasswordDTO
                    {
                        HashedPassword = (byte[])reader["HashedPassword"],
                        Salt = (byte[])reader["Salt"]
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_USER_HASHEDPASSWORD_AND_SALT_QUERY, parameters, mapFunction);
                return result?.FirstOrDefault();
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> IsFieldInUseAsync(string columnName, string columnValue)
        { 
            string query = $@"SELECT 1 FROM [User] WHERE [{columnName}]=@columnName";
            SqlParameter[] parameter = _dbCommand.GetSqlParametersFromObject(new { columnName = columnValue });
            var result = await _dbCommand.IsRecordPresentAsync(query, parameter);
            return result;
        }

        public async Task<UserDTO> GetUserDataAsync(string email, byte roleId)
        {
            const string GET_USER_DATA_QUERY = @"
                        SELECT u.Id, u.FirstName, u.LastName, u.DepartmentId, u.Email, ur.[RoleId], 
                                u.ManagerId, CONCAT(m.FirstName, ' ',  m.LastName) AS ManagerName, m.Email AS ManagerEmail
                        FROM [User] AS u
                                LEFT JOIN [User] AS m ON m.Id = u.ManagerId
                                INNER JOIN UserRole ur ON ur.UserId = u.Id
                        WHERE u.Email = @Email  AND ur.RoleId = @RoleId";
            SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email, RoleId = roleId });
            Func<IDataReader, UserDTO> mapFunction = reader =>
            {
                return new UserDTO
                {
                    Id = (short)reader["Id"],
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    DepartmentId = reader["DepartmentId"] == DBNull.Value ? (byte?)null : (byte)reader["DepartmentId"],
                    ManagerId = reader["ManagerId"] == DBNull.Value ? (short?)null : (short)reader["ManagerId"],
                    ManagerName = reader["ManagerName"] == DBNull.Value ? null : reader["ManagerName"].ToString(),
                    ManagerEmail = reader["ManagerEmail"] == DBNull.Value ? null : reader["ManagerEmail"].ToString(),
                    Email = reader["Email"].ToString(),
                    RoleId = (byte)reader["RoleId"]
                };
            };
            var result = await _dbCommand.ExecuteSelectQueryAsync(GET_USER_DATA_QUERY, parameters, mapFunction);
            return result.FirstOrDefault();
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

        public async Task<bool> RegisterUserAsync(User user, string email)
        {
            string INSERT_INTO_USER_QUERY =
                  $@"INSERT INTO [User] (FirstName, LastName, MobileNumber, NationalIdentityCard, DepartmentId, ManagerId, 
                         Email, HashedPassword, Salt)
                         VALUES (@FirstName, @LastName, @MobileNumber, @NationalIdentityCard, @DepartmentId, @ManagerId, 
                         @Email, @Password, @Salt);

                         DECLARE @UserId SMALLINT;
                         SET @UserId = SCOPE_IDENTITY();
                         INSERT INTO UserRole (UserId, RoleId) VALUES
                         (@UserId, @RoleId)";

            List<string> excludedUserProperties = new List<string> { "Id" };
            SqlParameter[] userQueryParams = _dbCommand.GetSqlParametersFromObject(user, excludedUserProperties);
            SqlParameter roleIdParam = new SqlParameter("@RoleId", SqlDbType.TinyInt)
            {
                Value = (byte)RoleEnum.Employee
            };
            SqlParameter[] allParams = userQueryParams.Concat(new[] { roleIdParam }).ToArray();

            bool isSuccessful = await _dbCommand.ExecuteTransactionAsync(new SqlCommand(INSERT_INTO_USER_QUERY), allParams);
            return isSuccessful;
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

        public async Task<IEnumerable<ManagerDTO>> GetAllManagersFromDepartmentAsync(byte departmentId)
        {
            try
            {
                string GET_MANAGERS_FROM_DEPARTMENT_QUERY =
                    $@"SELECT Id, FirstName, LastName FROM [User] AS u
                       INNER JOIN UserRole AS ur
                       ON ur.UserId = u.Id
                       WHERE ur.RoleId = {(byte)RoleEnum.Manager} and DepartmentId = @DepartmentId";
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
