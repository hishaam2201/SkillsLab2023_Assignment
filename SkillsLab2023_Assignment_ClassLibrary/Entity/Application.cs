using SkillsLab2023_Assignment_ClassLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Application : BaseEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; } // FK
        public int TrainingId { get; private set; } // FK
        public ApplicationStatusEnum StatusEnum { get; set; }
        public DateTime ApplicationDateTime { get; set; }

        // TODO
        public override string TableName => throw new NotImplementedException();

        public override string PrimaryKeyColumn => throw new NotImplementedException();

        public override string InsertSqlTemplate => throw new NotImplementedException();

        public override string UpdateSqlTemplate => throw new NotImplementedException();
    }
}
