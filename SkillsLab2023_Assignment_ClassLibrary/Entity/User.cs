using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class User : BaseEntity
    {
        public int Id { get; private set; }
        public int DepartmentId { get; private set; } // FK
        public int RoleId { get; private set; } // FK
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNumber { get; set; }
        public string NIC { get; set; }
        public string ManagerName { get; set; } // Will be null at first
        public string Email { get; set; }
        public string Password { get; set; }

        // TODO
        public override string TableName => throw new NotImplementedException();

        public override string PrimaryKeyColumn => throw new NotImplementedException();

        public override string InsertSqlTemplate => throw new NotImplementedException();

        public override string UpdateSqlTemplate => throw new NotImplementedException();
    }
}
