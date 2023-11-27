using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Entity
{
    public class DocumentUpload : BaseEntity
    {
        public int ApplicationId { get; private set; } // FK
        public string Url { get; set; }

        // TODO
        public override string TableName => throw new NotImplementedException();

        public override string PrimaryKeyColumn => throw new NotImplementedException();

        public override string InsertSqlTemplate => throw new NotImplementedException();

        public override string UpdateSqlTemplate => throw new NotImplementedException();
    }
}
