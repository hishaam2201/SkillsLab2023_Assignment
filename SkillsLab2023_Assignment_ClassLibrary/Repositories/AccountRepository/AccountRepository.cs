using SkillsLab2023_Assignment_ClassLibrary.Entity;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                const string AUTHENTICATE_LOGIN_CREDENTIALS_QUERY = @"SELECT Email, Password FROM Account WHERE Email=@Email 
                                                                      And Password=@Password";
                using (SqlCommand sqlCommand = new SqlCommand(AUTHENTICATE_LOGIN_CREDENTIALS_QUERY, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Email", email);
                    sqlCommand.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        public bool EmailExists(string email)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                const string EMAIL_EXISTS_QUERY = @"SELECT Email FROM [User] WHERE Email=@Email";

                using (SqlCommand sqlCommand = new SqlCommand(EMAIL_EXISTS_QUERY, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        public bool Register(User user)
        {
            using (SqlConnection sqlConnection = _dataAccessLayer.CreateConnection())
            {
                const string CREATE_USER_TRANSACTION_QUERY =
                @"BEGIN TRANSACTION
                    DECLARE @RoleId INT
                    SELECT @RoleId = Id FROM [Role]
                    WHERE [Description] = 'Employee'

                    INSERT INTO [User] (FirstName, LastName, NIC, MobileNumber, DepartmentId, RoleId, ManagerName, Email, [Password])
                    VALUES (@FirstName, @LastName, @NIC, @MobileNumber, @DepartmentId, @RoleId, @ManagerName, @Email, @Password)

                    Declare @UserId int
	                SET @UserId = SCOPE_IDENTITY()

	                Insert into Account (Email, [Password], UserId) 
	                SELECT Email, [Password], Id from [User] where Id = @UserId
                COMMIT;";

                using (SqlCommand sqlCommand = new SqlCommand(CREATE_USER_TRANSACTION_QUERY, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                    sqlCommand.Parameters.AddWithValue("@LastName", user.LastName);
                    sqlCommand.Parameters.AddWithValue("@NIC", user.NationalIdentityCard);
                    sqlCommand.Parameters.AddWithValue("@MobileNumber", user.MobileNumber);
                    sqlCommand.Parameters.AddWithValue("@DepartmentId", user.DepartmentId);
                    sqlCommand.Parameters.AddWithValue("@ManagerName", user.ManagerName);
                    sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                    sqlCommand.Parameters.AddWithValue("@Password", user.Password);

                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
