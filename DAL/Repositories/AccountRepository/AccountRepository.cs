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
using System.Security.Cryptography.X509Certificates;

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
                const string AUTHENTICATE_LOGIN_CREDENTIALS_QUERY = @"SELECT Email, Password FROM Account WHERE Email=@Email 
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
                const string EMAIL_EXISTS_QUERY = @"SELECT 1 FROM Account WHERE Email=@Email";
                SqlParameter[] emailExistsParams = _dbCommand.GetSqlParametersFromObject(new { Email = email });

                bool isPresent = await _dbCommand.IsRecordPresentAsync(EMAIL_EXISTS_QUERY, emailExistsParams);
                return isPresent;
            }
            catch (Exception) { throw; }
        }

        public async Task<UserDTO> GetUserDataAsync(string email)
        {
            try
            {
                const string GET_USER_DATA_QUERY = @"SELECT u.Id, u.FirstName, u.LastName, u.DepartmentId, u.RoleId, u.ManagerId, a.Email
                                                   FROM [User] as u
                                                   INNER JOIN Account a
                                                   ON a.Id = u.Id
                                                   WHERE a.Email = @Email";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email });
                Func<IDataReader, UserDTO> mapFunction = reader =>
                {
                    return new UserDTO
                    {
                        Id = (short)reader["Id"],
                        FirstName = reader["FirstName"]?.ToString(),
                        LastName = reader["LastName"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        DepartmentId = reader["DepartmentId"] == DBNull.Value ? (byte?)null : (byte)reader["DepartmentId"],
                        RoleId = (byte)reader["RoleId"],
                        ManagerId = reader["ManagerId"] == DBNull.Value ? (short?)null : (short)reader["ManagerId"],
                    };
                };
                var result = await _dbCommand.ExecuteSelectQueryAsync(GET_USER_DATA_QUERY, parameters, mapFunction);
                return result.FirstOrDefault();
            }
            catch (Exception) { throw; }
        }

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
