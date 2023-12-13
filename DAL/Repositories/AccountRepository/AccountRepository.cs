using System.Collections.Generic;
using System.Data.SqlClient;
using Framework.DatabaseCommand.DatabaseCommand;
using System;
using DAL.Models;
using System.Linq;
using DAL.DTO;

namespace DAL.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDatabaseCommand<Account> _dbCommand;
        public AccountRepository(IDatabaseCommand<Account> dbCommand)
        {
            _dbCommand = dbCommand;
        }

        public bool AuthenticateLoginCredentials(string email, string password)
        {
            try
            {
                const string AUTHENTICATE_LOGIN_CREDENTIALS_QUERY = @"SELECT Email, Password FROM Account WHERE Email=@Email 
                                                                    And Password=@Password";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Email = email, Password = password });

                return _dbCommand.RecordExists(AUTHENTICATE_LOGIN_CREDENTIALS_QUERY, parameters);
            }
            catch (Exception) { throw; }
        }

        public bool EmailExists(string email)
        {
            try
            {
                const string EMAIL_EXISTS_QUERY = @"SELECT 1 FROM Account WHERE Email=@Email";
                SqlParameter[] emailExistsParams = _dbCommand.GetSqlParametersFromObject(new { Email = email });

                return _dbCommand.RecordExists(EMAIL_EXISTS_QUERY, emailExistsParams);
            }
            catch (Exception) { throw; }
        }

        public IEnumerable<DepartmentDTO> GetAllDepartments()
        {
            try
            {
                return _dbCommand.ExecuteSelectQuery<DepartmentDTO>(query: $@"SELECT * FROM Department;");
            }
            catch(Exception) { throw; }
        }

        public IEnumerable<ManagerDTO> GetAllManagersFromDepartment()
        {
            throw new NotImplementedException();
        }

        // TODO: Need to rework on that, make it retrieve RoleName
        public int GetRoleId(string role)
        {
            try
            {
                const string GET_ROLE_ID_QUERY = @"SELECT Id FROM [Role] WHERE RoleName = @Role";
                SqlParameter[] parameters = _dbCommand.GetSqlParametersFromObject(new { Role = role });
                return (int)_dbCommand.ReturnFirstColumnOfFirstRow(GET_ROLE_ID_QUERY, parameters);
            }
            catch (Exception) { throw; }
        }

        public bool Register(User user, string email, string password)
        {
            try
            {
                string INSERT_INTO_USER_AND_ACCOUNT_QUERY =
                      $@"INSERT INTO [User] 
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

                _dbCommand.ExecuteTransaction(out bool isSuccessful,
                    new SqlCommand(INSERT_INTO_USER_AND_ACCOUNT_QUERY), queryParams);

                return isSuccessful;
            }
            catch (Exception) { throw; }
        }
    }
}
