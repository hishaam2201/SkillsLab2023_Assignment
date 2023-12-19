
using Framework.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.DatabaseCommand.DatabaseCommand
{
    public class DatabaseCommand<T> : IDatabaseCommand<T>
    {
        private readonly IDataAccessLayer _dataAccessLayer;
        public DatabaseCommand(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer;
        }

        public async Task<bool> ExecuteTransactionAsync(SqlCommand command, SqlParameter[] parameters)
        {
            bool isSuccessful;

            using (SqlConnection sqlConnection = await _dataAccessLayer.CreateConnectionAsync())
            {
                using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        command.Connection = sqlConnection;
                        command.Transaction = sqlTransaction;

                        if (parameters != null && parameters.Any())
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        await command.ExecuteNonQueryAsync();

                        sqlTransaction.Commit();
                        isSuccessful = true;
                    }
                    catch (Exception)
                    {
                        sqlTransaction.Rollback();
                        isSuccessful = false;
                        throw;
                    }
                }
            }
            return isSuccessful;
        }

        public async Task<bool> IsRecordPresentAsync(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = await _dataAccessLayer.CreateConnectionAsync())
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        if (parameters != null && parameters.Any())
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        using(SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            return reader.HasRows;
                        }
                    }
                    catch (Exception) { throw; } 
                }
            }
        }

        public async Task<object> GetScalerResultAsync(string query, SqlParameter[] parameters)
        {
            using (SqlConnection sqlConnection = await _dataAccessLayer.CreateConnectionAsync())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        if (parameters != null && parameters.Any())
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }
                        return await sqlCommand.ExecuteScalarAsync();
                    }
                    catch (Exception) { throw; }
                }
            }
        }

        public async Task<int> AffectedRowsCountAsync(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection sqlConnection = await _dataAccessLayer.CreateConnectionAsync())
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    try
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }

                        return await sqlCommand.ExecuteNonQueryAsync();
                    }
                    catch (Exception) { throw; }
                }
            }
        }

        public async Task<IEnumerable<TResult>> ExecuteSelectQueryAsync<TResult>(string query = null, SqlParameter[] parameters = null,
            Func<IDataReader, TResult> mapFunction = null)
        {
            List<TResult> results = new List<TResult>();
            using(SqlConnection sqlConnection = await _dataAccessLayer.CreateConnectionAsync())
            {
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(query ?? $@"SELECT * FROM [{typeof(TResult).Name}]", sqlConnection))
                    {
                        if (parameters != null && parameters.Any())
                        {
                            sqlCommand.Parameters.AddRange(parameters);
                        }

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    TResult item;
                                    if (mapFunction != null)
                                    {
                                        item = mapFunction(reader);
                                    }
                                    else
                                    {
                                        item = Activator.CreateInstance<TResult>();
                                        MapColumnsToProperties(item, reader);
                                    }
                                    results.Add(item);
                                }
                            }
                        }
                    }
                }
                catch(Exception) { throw; }
            }
            return results;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using (SqlConnection sqlConnection = await _dataAccessLayer.CreateConnectionAsync())
            {
                string GET_BY_ID_QUERY = $@"SELECT * FROM [{typeof(T).Name}] WHERE Id = @Id";
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(GET_BY_ID_QUERY, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                T item = Activator.CreateInstance<T>();
                                MapColumnsToProperties(item, reader);
                                return item;
                            }
                            return default; // return null
                        }
                    }
                }
                catch (Exception) { throw; }
            }
        }

        // PUBLIC HELPER METHODS
        public SqlParameter[] GetSqlParametersFromObject(object obj, List<string> excludedProperties = null)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (PropertyInfo property in properties)
            {
                if (excludedProperties == null || !excludedProperties.Contains(property.Name))
                {
                    parameters.Add(new SqlParameter($"@{property.Name}", property.GetValue(obj)));
                }
            }
            return parameters.ToArray();
        }

        // PRIAVTE HELPER METHODS
        private static void MapColumnsToProperties<TResult>(TResult item, IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                PropertyInfo property = typeof(TResult).GetProperty(columnName);

                if (property != null && !reader.IsDBNull(i))
                {
                    var value = reader[i] == DBNull.Value ? null : reader[i];
                    property.SetValue(item, value);
                }
            }
        }
    }
}

