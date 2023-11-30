using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class DocumentUpload
    {
        public int ApplicationId { get; private set; } // FK
        public string Url { get; set; }

/*        public override string TableName => "DocumentUpload";

        public override string PrimaryKeyColumn => "Id";

        public override string InsertSqlTemplate => $@"INSERT INTO {TableName} (ApplicationId, Url) VALUES (@ApplicationId, @Url)";

        public override string UpdateSqlTemplate => $@"UPDATE {TableName} SET ApplicationId=@ApplicationId, Url=@Url WHERE Id=@Id";*/
    }
}
