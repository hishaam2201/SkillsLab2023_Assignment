using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Framework.DatabaseCommand.DatabaseCommand
{
    public interface IDatabaseCommand<T>
    {
        Task<bool> ExecuteTransactionAsync(SqlCommand command, SqlParameter[] parameters);
        Task<bool> IsRecordPresentAsync(string query, SqlParameter[] parameters);
        Task<object> GetScalerResultAsync(string query, SqlParameter[] parameters);
        Task<int> AffectedRowsCountAsync(string query, SqlParameter[] parameters = null);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<TResult>> ExecuteSelectQueryAsync<TResult>(
            string query = null, SqlParameter[] parameters = null, Func<IDataReader, TResult> mapFunction = null);
        SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedParameters = null);
    }
}