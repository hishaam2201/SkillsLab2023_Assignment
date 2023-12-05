using SkillsLab2023_Assignment_ClassLibrary.Entity;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace SkillsLab2023_Assignment_ClassLibrary.Repositories.DatabaseCommand
{
    public interface IDatabaseCommand<T>
    {
        void ExecuteTransaction(out bool isSuccessful, SqlCommand command, SqlParameter[] parameters);
        SqlParameter[] GetSqlParametersFromObject(object obj);
        SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedProperties);
        bool RecordExists(string query, SqlParameter[] parameters);
        object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters);
        int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null);
        IEnumerable<T> GetAll();
    }
}