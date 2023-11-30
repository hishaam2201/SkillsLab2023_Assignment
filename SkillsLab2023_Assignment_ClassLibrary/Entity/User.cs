using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class User
    {
        public int Id { get; private set; }
        public int DepartmentId { get; set; } // FK
        public int RoleId { get; private set; } // FK
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNumber { get; set; }
        public string NIC { get; set; }
        public string ManagerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

/*      public override string TableName => "User";

        public override string PrimaryKeyColumn => "Id";

        public override string InsertSqlTemplate => $@"INSERT INTO {TableName} (DepartmentId, RoleId, FirstName, LastName, 
                                                       MobileNumber, NIC, ManagerName, Email, Password) VALUES 
                                                       (@DepartmentId, @RoleId, @FirstName, @LastName, @MobileNumber, @NIC, 
                                                       @ManagerName, @Email, @Password)";

        public override string UpdateSqlTemplate => $@"UPDATE {TableName} SET DepartmentId=@DepartmentId, RoleId=@RoleId, 
                                                       FirstName=@FirstName, LastName=@LastName, MobileNumber=@MobileNumber,
                                                       NIC=@NIC, ManagerName=@ManagerName, Email=@Email, Password=@Password,
                                                       WHERE Id=@Id";*/
    }
}
