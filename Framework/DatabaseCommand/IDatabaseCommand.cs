using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Framework.DatabaseCommand.DatabaseCommand
{
    public interface IDatabaseCommand<T>
    {
        void ExecuteTransaction(out bool isSuccessful, SqlCommand command, SqlParameter[] parameters);
        SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedParameters = null);
        bool RecordExists(string query, SqlParameter[] parameters);
        object ReturnFirstColumnOfFirstRow(string query, SqlParameter[] parameters);
        int ReturnNumOfRowsAffected(string query, SqlParameter[] parameters = null);
        IEnumerable<TResult> ExecuteSelectQuery<TResult>(
            string query = null, SqlParameter[] parameters = null, Func<IDataReader, TResult> mapFunction = null);
        T GetById(int id);
    }
}