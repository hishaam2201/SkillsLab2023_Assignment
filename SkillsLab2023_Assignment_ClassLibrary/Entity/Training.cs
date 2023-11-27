using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Training : BaseEntity
    {
        public int Id { get; private set; }
        public int PreRequisiteId { get; private set; } // FK
        public string TrainingProgrammeName { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime StartingDate { get; set; }
        public int DepartmentId { get; private set; } // FK

        // TODO
        public override string TableName => throw new NotImplementedException();

        public override string PrimaryKeyColumn => throw new NotImplementedException();

        public override string InsertSqlTemplate => throw new NotImplementedException();

        public override string UpdateSqlTemplate => throw new NotImplementedException();
    }
}
