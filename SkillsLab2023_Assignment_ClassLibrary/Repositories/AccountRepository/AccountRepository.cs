using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Enums;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataAccessLayer _dataAccessLayer;
        public AccountRepository(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer;
        }

        public bool AuthenticateLoginCredentials(string email, string password)
        {
            const string AUTHENTICATE_LOGIN_CREDENTIALS_QUERY = @"SELECT Email, Password FROM Account WHERE Email=@Email 
                                                                And Password=@Password";
            SqlParameter[] parameters = _dataAccessLayer.GetSqlParametersFromObject(new { Email = email, Password = password });

            return _dataAccessLayer.RecordExists(AUTHENTICATE_LOGIN_CREDENTIALS_QUERY, parameters);
        }

        public bool EmailExists(string email)
        {
            const string EMAIL_EXISTS_QUERY = @"SELECT 1 FROM [User] WHERE Email=@Email";
            SqlParameter[] emailExistsParams = _dataAccessLayer.GetSqlParametersFromObject(new { Email = email });

            return _dataAccessLayer.RecordExists(EMAIL_EXISTS_QUERY, emailExistsParams);
        }

        public int GetRoleId(string role)
        {
            const string GET_ROLE_ID_QUERY = @"SELECT Id FROM [Role] WHERE [Description] = @Role";
            SqlParameter[] parameters = _dataAccessLayer.GetSqlParametersFromObject(new { Role = role });
            return (int)_dataAccessLayer.ReturnFirstColumnOfFirstRow(GET_ROLE_ID_QUERY, parameters);
        }

        public bool Register(User user)
        {
            string INSERT_INTO_USER_AND_ACCOUNT_QUERY =
               $@"INSERT INTO [User] 
                  (FirstName, LastName, NIC, MobileNumber, DepartmentId, RoleId, ManagerName, Email, [Password])
                  VALUES (@FirstName, @LastName, @NationalIdentityCard, @MobileNumber, @DepartmentId, 
                  {GetRoleId("Employee")}, @ManagerName, @Email, @Password); 
                  
                  DECLARE @UserId INT
                  SET @UserId = SCOPE_IDENTITY()

                  INSERT INTO Account (Email, [Password], UserId) 
                  SELECT Email, [Password], Id FROM [User] WHERE Id = @UserId";

            List<string> excludedUserProperties = new List<string> { "UserId", "RoleId" };
            SqlParameter[] userQueryParams = _dataAccessLayer.GetSqlParametersFromObject(user, excludedUserProperties);
            _dataAccessLayer.ExecuteTransaction(out bool isSuccessful, 
                new SqlCommand(INSERT_INTO_USER_AND_ACCOUNT_QUERY), userQueryParams);

            return isSuccessful;
        }
    }
}
