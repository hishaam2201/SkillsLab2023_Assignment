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
        public ApplicationStatusEnum Status { get; set; }
        public DateTime ApplicationDateTime { get; set; }

        // TODO
        public override string TableName => "Application";

        public override string PrimaryKeyColumn => "Id";

        public override string InsertSqlTemplate => $@"INSERT INTO {TableName} (UserId, TrainingId, Status, ApplicationDateTime)
                                                       VALUES (@UserId, @TrainingId, @Status, @ApplicationDateTime)";

        public override string UpdateSqlTemplate => $@"UPDATE {TableName} SET UserId=@UserId, TrainingId=@TrainingId, 
                                                    Status=@Status, ApplicationDateTime=@ApplicationDateTime WHERE Id=@Id";
    }
}
