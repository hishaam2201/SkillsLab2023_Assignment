using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class Training
    {
        public int Id { get; private set; }
        public string ProgrammeName { get; set; }
        public int Capacity { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime StartingDate { get; set; }
        public int DepartmentId { get; private set; } // FK

        /*        public override string TableName => "Training";

                public override string PrimaryKeyColumn => "Id";

                public override string InsertSqlTemplate => $@"INSERT INTO {TableName} (PreRequisiteId, ProgrammeName, 
                                                               Description, Capacity, Deadline, StartingDate, DepartmentId) VALUES
                                                               (@PreRequisiteId, @ProgrammeName, @Description, @Capacity, 
                                                                @Deadline, @StartingDate, @DepartmentId)";

                public override string UpdateSqlTemplate => $@"UPDATE {TableName} SET PreRequisiteId=@PreRequisiteId, 
                                                               ProgrammeName=@ProgrammeName, Description=@Description, 
                                                               Capacity=@Capacity, Deadline=@Deadline, 
                                                               StartingDate=@StartingDate, DepartmentId=@DepartmentId
                                                               WHERE Id=@Id";*/
    }
}
