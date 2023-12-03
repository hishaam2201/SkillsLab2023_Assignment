using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand
{
    public interface IDbCommand
    {
        int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null);
        object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters);
        void ExecuteTransaction(out bool isSuccessful, SqlCommand command, SqlParameter[] parameters);
        SqlParameter[] GetSqlParametersFromObject(object obj);
        SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedProperties);
        bool RecordExists(string query, SqlParameter[] parameters);
    }
}
