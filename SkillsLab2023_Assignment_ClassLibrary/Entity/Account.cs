using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Account : BaseEntity
    {
        public int Id { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public override string TableName => "Account";

        public override string PrimaryKeyColumn => "Id";

        public override string InsertSqlTemplate => $"INSERT INTO {TableName} (Email, Password) VALUES (@Email, @Password)";

        public override string UpdateSqlTemplate => $"UPDATE {TableName} SET Email=@Email, Password=@Password WHERE Id=@Id";
    }
}
