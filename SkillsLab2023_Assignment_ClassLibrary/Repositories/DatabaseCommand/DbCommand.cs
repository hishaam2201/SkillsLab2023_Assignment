using SkillsLab2023_Assignment_ClassLibrary.Repositories.DAL;
using SkillsLab2023_Assignment_ClassLibrary.Repositories.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand
{
    public class DbCommand : IDbCommand
    {
        private IDataAccessLayer _dataAccessLayer;
        public DbCommand(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer;
        }

        public void ExecuteTransaction(out bool isSuccessful, SqlCommand command, SqlParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public SqlParameter[] GetSqlParametersFromObject(object obj)
        {
            throw new NotImplementedException();
        }

        public SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedProperties)
        {
            throw new NotImplementedException();
        }

        public bool RecordExists(string query, SqlParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
