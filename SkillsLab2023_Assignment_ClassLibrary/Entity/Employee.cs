using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Employee : BaseEntity
    {
        // REFERENCE CLASS
        public int Id { get; private set; }
        public string Name { get; set; }
        public override string TableName => "Employee";

        public override string PrimaryKeyColumn => "Id";

        public override string InsertSqlTemplate => $@"INSERT INTO {TableName} (Name) VALUES (@Name)";

        public override string UpdateSqlTemplate => $@"UPDATE {TableName} SET Name=@Name WHERE Id=@Id";
    }
}
